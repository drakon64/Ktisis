using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ktisis.Common.Clients;
using Ktisis.Common.Models.GoogleCloud;
using Ktisis.Common.Models.GoogleCloud.Compute.Instances;
using Ktisis.Common.Models.GoogleCloud.Compute.Instances.Disks;
using Ktisis.Common.Models.GoogleCloud.Compute.Instances.NetworkInterfaces;
using Ktisis.Common.Models.GoogleCloud.Tasks;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.WorkflowJob;
using Octokit.Webhooks.Models.WorkflowJobEvent;

namespace Ktisis.Receiver.EventProcessors;

public sealed class GitHubWebhookEventProcessor : WebhookEventProcessor
{
    private static readonly Random Random = new();

    protected override async Task ProcessWorkflowJobWebhookAsync(
        WebhookHeaders headers,
        WorkflowJobEvent workflowJobEvent,
        WorkflowJobAction action
    )
    {
        if (
            Program.RepositoryOwners is not null
            && !Program.RepositoryOwners.Contains(workflowJobEvent.Repository!.Owner.Login)
        )
        {
            await Console.Out.WriteLineAsync(
                $"Repository {workflowJobEvent.Repository.FullName} is not owned by an allowlisted owner."
            );

            return;
        }

        if (!workflowJobEvent.WorkflowJob.Labels.Contains("ktisis"))
        {
            await Console.Out.WriteLineAsync("Workflow is not set to use Ktisis.");

            return;
        }

        var instanceName =
            $"{workflowJobEvent.Repository!.FullName.Replace('/', '-')}-{workflowJobEvent.WorkflowJob.RunId}-{workflowJobEvent.WorkflowJob.Id}";
        var taskName =
            $"projects/{Program.Project}/locations/{Program.Region}/queues/{Program.Queue}/tasks/{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(instanceName)))}";

        switch (workflowJobEvent.Action)
        {
            case "queued":
            {
                var machineType = "c3d-standard-4";
                var disk = "14";
                var architecture = "amd64";
                var runnerArchitecture = "x64";
                var zone = Program.Zones[Random.Next(Program.Zones.Length)];

                {
                    var setMachineType = false;
                    var setDisk = false;
                    var setArchitecture = false;

                    foreach (var label in workflowJobEvent.WorkflowJob.Labels)
                    {
                        if (label.StartsWith("ktisis-"))
                        {
                            var ktisis = label.Replace("ktisis-", "");

                            if (label.EndsWith("GB"))
                            {
                                disk = ktisis.Replace("GB", "");
                                setDisk = true;
                            }
                            else
                            {
                                machineType = ktisis;
                                setMachineType = true;
                            }
                        }
                        else
                            switch (label)
                            {
                                case "x64":
                                    architecture = "amd64";
                                    runnerArchitecture = "x64";
                                    setArchitecture = true;
                                    break;
                                case "ARM64":
                                    architecture = "arm64";
                                    runnerArchitecture = "arm64";
                                    setArchitecture = true;
                                    break;
                            }

                        if (setMachineType && setDisk && setArchitecture)
                        {
                            break;
                        }
                    }
                }

                await GoogleClient.CreateTask(
                    new CreateCloudTask
                    {
                        Task = new CloudTask
                        {
                            Name = taskName,
                            HttpRequest = new CloudTaskHttpRequest
                            {
                                Url = Program.TaskServiceUrl,
                                Body = Convert.ToBase64String(
                                    Encoding.UTF8.GetBytes(
                                        JsonSerializer.Serialize(
                                            new InstanceTask
                                            {
                                                Zone = zone,
                                                Instance = new CreateInstance
                                                {
                                                    Name = instanceName,
                                                    MachineType =
                                                        $"projects/{GoogleClient.Project}/zones/{zone}/machineTypes/{machineType}",
                                                    NetworkInterfaces =
                                                    [
                                                        new NetworkInterface
                                                        {
                                                            Network = Program.Network,
                                                            Subnetwork = Program.Subnetwork,
                                                        },
                                                    ],
                                                    Disks =
                                                    [
                                                        new Disk
                                                        {
                                                            Boot = true,
                                                            InitializeParams =
                                                                new DiskInitializeParams
                                                                {
                                                                    DiskSizeGb = disk,
                                                                    DiskType =
                                                                        $"projects/{GoogleClient.Project}/zones/{zone}/diskTypes/hyperdisk-balanced",
                                                                    SourceImage =
                                                                        $"projects/ubuntu-os-cloud/global/images/ubuntu-minimal-2404-noble-{architecture}-v20250213", // TODO: Don't hardcode this
                                                                },
                                                        },
                                                        new Disk
                                                        {
                                                            DeviceName = "swap",
                                                            InitializeParams =
                                                                new DiskInitializeParams
                                                                {
                                                                    DiskSizeGb = "16",
                                                                    DiskType =
                                                                        $"projects/{GoogleClient.Project}/zones/{zone}/diskTypes/hyperdisk-balanced",
                                                                },
                                                        },
                                                    ],
                                                    Metadata = new Metadata
                                                    {
                                                        Items =
                                                        [
                                                            new MetadataItem
                                                            {
                                                                Key = "enable-oslogin",
                                                                Value = "true",
                                                            },
                                                            new MetadataItem
                                                            {
                                                                Key = "enable-oslogin-2fa",
                                                                Value = "true",
                                                            },
                                                            new MetadataItem
                                                            {
                                                                Key = "startup-script",
                                                                Value = $"""
                                                                #!/bin/sh -ex

                                                                sysctl vm.swappiness=1
                                                                mkswap /dev/disk/by-id/google-swap
                                                                swapon /dev/disk/by-id/google-swap

                                                                curl -sSO https://dl.google.com/cloudagents/add-google-cloud-ops-agent-repo.sh
                                                                bash add-google-cloud-ops-agent-repo.sh --also-install
                                                                rm add-google-cloud-ops-agent-repo.sh

                                                                adduser --home /runner --shell /bin/sh runner
                                                                echo '%runner ALL=(ALL:ALL) NOPASSWD:ALL' > /etc/sudoers.d/runner
                                                                cd /runner

                                                                wget https://github.com/actions/runner/releases/download/v2.322.0/actions-runner-linux-{runnerArchitecture}-2.322.0.tar.gz
                                                                tar xf actions-runner-linux-{runnerArchitecture}-2.322.0.tar.gz
                                                                rm actions-runner-linux-{runnerArchitecture}-2.322.0.tar.gz

                                                                sudo -u runner ./config.sh --url https://github.com/{workflowJobEvent.Repository!.FullName} --token {await GitHubClient.CreateRunnerRegistrationToken(
                                                                    workflowJobEvent.Repository!.FullName,
                                                                    workflowJobEvent.Installation!.Id
                                                                )} --ephemeral --labels ktisis,ktisis-{machineType},ktisis-{disk}GB
                                                                ./svc.sh install runner
                                                                ./svc.sh start
                                                                """,
                                                            },
                                                        ],
                                                    },
                                                    ServiceAccounts =
                                                    [
                                                        new ServiceAccount
                                                        {
                                                            Email = Program.ServiceAccountEmail,
                                                        },
                                                    ],
                                                },
                                            },
                                            GoogleCloudSerializerContext.Default.InstanceTask
                                        )
                                    )
                                ),
                            },
                        },
                    }
                );

                break;
            }
            case "completed":
            {
                if (
                    workflowJobEvent.WorkflowJob.Conclusion!.Value
                    == WorkflowJobConclusion.Cancelled
                )
                    await GoogleClient.DeleteTask(taskName);

                // TODO: Get the instance zone rather than brute-forcing
                foreach (var zone in Program.Zones)
                    await GoogleClient.DeleteInstance(
                        workflowJobEvent.WorkflowJob.RunnerName!,
                        zone
                    );

                break;
            }
            default:
                await Console.Out.WriteLineAsync(
                    "Workflow action is neither `queued` nor `completed`."
                );

                break;
        }
    }
}
