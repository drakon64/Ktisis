using Google.Cloud.Tasks.V2;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.WorkflowJob;

namespace Ktisis.Webhook;

public class WorkflowJobWebhookEventProcessor(ILogger<WorkflowJobWebhookEventProcessor> logger)
    : WebhookEventProcessor
{
    private static readonly Task<CloudTasksClient> TasksClient = CloudTasksClient.CreateAsync();

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
                "Invalid repository: {FullName}",
                workflowJobEvent.Repository!.FullName
            );

            return;
        }

        if (action != WorkflowJobAction.Queued || action != WorkflowJobAction.Completed)
            return;

        logger.LogInformation("Repository: {FullName}", workflowJobEvent.Repository!.FullName);

        var tasksClient = await TasksClient;
    }
}
