using Ktisis.Clients;
using Ktisis.Models.GoogleCloud.Compute;
using Ktisis.Models.GoogleCloud.Compute.Instances.Disks;
using Ktisis.Models.GoogleCloud.Compute.Instances.Metadata;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.WorkflowJob;

namespace Ktisis.EventProcessors;

public sealed class GitHubWebhookEventProcessor : WebhookEventProcessor
{
    protected override async Task ProcessWorkflowJobWebhookAsync(
        WebhookHeaders headers,
        WorkflowJobEvent workflowJobEvent,
        WorkflowJobAction action
    )
    {
        if (
            Program.RepositoryOwners is not null
            && !Program.RepositoryOwners.Contains(workflowJobEvent.Repository!.Owner.Name)
        )
        {
            await Console.Out.WriteLineAsync(
                $"Repository {workflowJobEvent.Repository.FullName} is not owned by an allowlisted owner."
            );

            return;
        }

        if (workflowJobEvent.Action != "queued")
        {
            await Console.Out.WriteLineAsync($"Workflow action is not `queued`.");

            return;
        }

        if (!workflowJobEvent.WorkflowJob.Labels.Contains("ktisis"))
        {
            await Console.Out.WriteLineAsync($"Workflow is not set to use Ktisis.");

            return;
        }

        var machineType = "n4-standard-4";
        var disk = "14";
        var architecture = "amd64";
        var runnerArchitecture = "x64";

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

        await GoogleClient.CreateInstance(
            new Instance
            {
                Name =
                    $"{workflowJobEvent.Repository!.FullName.Replace('/', '-')}-{workflowJobEvent.WorkflowJob.RunId}-{workflowJobEvent.WorkflowJob.Id}",
                MachineType =
                    $"projects/{Program.Project}/zones/{Program.Zone}/machineTypes/{machineType}",
                Disks =
                [
                    new Disk
                    {
                        Boot = true,
                        InitializeParams = new DiskInitializeParams
                        {
                            DiskSizeGb = disk,
                            SourceImage =
                                $"projects/ubuntu-os-cloud/global/images/ubuntu-minimal-2404-noble-{architecture}-v20241116", // TODO: Don't hardcode this
                        },
                    },
                ],
                Metadata = new Metadata
                {
                    Items =
                    [
                        new MetadataItem { Key = "enable-oslogin", Value = "true" },
                        new MetadataItem { Key = "enable-oslogin-2fa", Value = "true" },
                        new MetadataItem
                        {
                            Key = "startup-script",
                            Value = $"""
                            #!/bin/sh

                            useradd runner --home /runner --shell /bin/sh --group runner
                            cd /runner
                            wget https://github.com/actions/runner/releases/download/v2.321.0/actions-runner-linux-{runnerArchitecture}-2.321.0.tar.gz
                            tar xf actions-runner-linux-{runnerArchitecture}-2.321.0.tar.gz
                            
                            ./config.sh --url https://github.com/{workflowJobEvent.Repository!.FullName} --token {(await Program.GitHubClient.CreateRunnerRegistrationToken(
                                workflowJobEvent.Repository!.FullName,
                                workflowJobEvent.Installation!.Id
                            )).Token} --ephemeral
                            ./svc.sh install runner
                            ./svc.sh start
                            """,
                        },
                    ],
                },
            },
            Program.Project,
            Program.Zone
        );
    }
}
