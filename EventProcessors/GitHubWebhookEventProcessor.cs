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
        byte disk = 14;

        {
            var setMachineType = false;
            var setDisk = false;

            foreach (var label in workflowJobEvent.WorkflowJob.Labels)
            {
                if (!label.StartsWith("ktisis-"))
                    continue;

                var ktisis = label.Replace("ktisis-", "");

                if (label.EndsWith("GB"))
                {
                    disk = Convert.ToByte(ktisis.Replace("GB", ""));
                    setDisk = true;
                }
                else
                {
                    machineType = ktisis;
                    setMachineType = true;
                }

                if (setMachineType && setDisk)
                {
                    break;
                }
            }
        }
    }
}
