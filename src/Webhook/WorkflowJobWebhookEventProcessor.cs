using Ktisis.Clients.CloudTasks;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.WorkflowJob;

namespace Ktisis.Webhook;

public class WorkflowJobWebhookEventProcessor(ILogger<WorkflowJobWebhookEventProcessor> logger)
    : WebhookEventProcessor
{
    protected override async ValueTask ProcessWorkflowJobWebhookAsync(
        WebhookHeaders headers,
        WorkflowJobEvent workflowJobEvent,
        WorkflowJobAction action,
        CancellationToken cancellationToken = default
    )
    {
        if (
            Program.Repositories != null
            && !Program.Repositories.Contains(workflowJobEvent.Repository!.FullName)
        )
        {
            logger.LogWarning(
                "Repository not allowed: {FullName}",
                workflowJobEvent.Repository.FullName
            );

            return;
        }

        if (
            !(action == WorkflowJobAction.Queued || action == WorkflowJobAction.Completed)
            || !workflowJobEvent.WorkflowJob.Labels.Contains("self-hosted")
        )
        {
            logger.LogInformation(
                "Not responding to {WorkflowJobAction} event from {FullName}",
                action.ToString(),
                workflowJobEvent.Repository!.FullName
            );

            return;
        }

        logger.LogInformation("Repository: {FullName}", workflowJobEvent.Repository!.FullName);

        var task = await CloudTasksClient.CreateTask(
            workflowJobEvent.Repository.FullName,
            workflowJobEvent.WorkflowJob.RunId,
            workflowJobEvent.WorkflowJob.Id,
            workflowJobEvent.Installation!.Id,
            action
        );

        if (!task.IsSuccessStatusCode)
        {
            logger.LogError(await task.Content.ReadAsStringAsync(cancellationToken));
        }
    }
}
