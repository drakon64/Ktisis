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

        var workflowJob =
            workflowJobEvent.Repository.FullName.Replace("/", "-")
            + '-'
            + workflowJobEvent.WorkflowJob.RunId
            + '-'
            + workflowJobEvent.WorkflowJob.Id;

        if (action.Equals(WorkflowJobAction.Completed))
        {
            // Failing to delete a Cloud Task is not fatal
            await CloudTasksClient.DeleteTask(CloudTasksClient.GetTaskName('c', workflowJob));

            await CloudTasksClient.CreateTask(
                CloudTasksClient.GetTaskName('d', workflowJob),
                new CloudTasksClient.TaskHttpRequest(CloudTasksClient.GetInstanceName(workflowJob))
            );
        }
        else
            await CloudTasksClient.CreateTask(
                CloudTasksClient.GetTaskName('c', workflowJob),
                new CloudTasksClient.TaskHttpRequest(
                    CloudTasksClient.GetInstanceName(workflowJob),
                    workflowJobEvent.Repository.FullName,
                    workflowJobEvent.Installation!.Id
                )
            );
    }
}
