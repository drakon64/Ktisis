using Ktisis.Clients;
using Ktisis.Models.GoogleCloud.Compute;
using Ktisis.Models.GoogleCloud.Compute.Instances.Disks;
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
            (
                Program.RepositoryOwners is not null
                && !Program.RepositoryOwners.Contains(workflowJobEvent.Repository!.Owner.Name)
            ) || !workflowJobEvent.WorkflowJob.Labels.Contains("ktisis")
        )
        {
            return;
        }

        var machineType = "n4-standard-4";
        var disk = "14";
        var architecture = "amd64";

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
                            setArchitecture = true;
                            break;
                        case "ARM64":
                            architecture = "arm64";
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
                MachineType = machineType,
                Name = "test",
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
            },
            Program.Project,
            Program.Zone
        );
    }
}
