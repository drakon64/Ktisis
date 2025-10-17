using Ktisis.Client.CloudTasks;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.WorkflowJob;

namespace Ktisis.Webhook;

internal class WorkflowJobWebhookEventProcessor(ILogger<WorkflowJobWebhookEventProcessor> logger)
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

        // If the workflow job action is not queued or completed, or the workflow job labels do not contain all of "self-hosted", "Linux", and "X64",
        // return an HTTP 200 to the webhook and do nothing
        if (
            !(action.Equals(WorkflowJobAction.Queued) || action.Equals(WorkflowJobAction.Completed))
            || !(
                workflowJobEvent.WorkflowJob.Labels.Contains("self-hosted")
                && workflowJobEvent.WorkflowJob.Labels.Contains("Linux")
                && workflowJobEvent.WorkflowJob.Labels.Contains("X64")
            )
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

        HttpResponseMessage task;

        if (action.Equals(WorkflowJobAction.Completed))
        {
            // Failing to delete a Cloud Task is not fatal
            await CloudTasksClient.DeleteTask(
                workflowJobEvent.Repository.FullName,
                workflowJobEvent.WorkflowJob.RunId,
                workflowJobEvent.WorkflowJob.Id
            );

            task = await CloudTasksClient.CreateTask(
                workflowJobEvent.Repository.FullName,
                workflowJobEvent.WorkflowJob.RunId,
                workflowJobEvent.WorkflowJob.Id,
                workflowJobEvent.WorkflowJob.RunnerName!
            );
        }
        else
            task = await CloudTasksClient.CreateTask(
                workflowJobEvent.Repository.FullName,
                workflowJobEvent.WorkflowJob.RunId,
                workflowJobEvent.WorkflowJob.Id,
                workflowJobEvent.Installation!.Id
            );

        if (!task.IsSuccessStatusCode)
        {
            logger.LogError(await task.Content.ReadAsStringAsync(cancellationToken));
        }
    }
}
