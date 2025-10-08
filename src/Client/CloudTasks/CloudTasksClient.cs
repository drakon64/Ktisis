namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    private static readonly string Queue =
        Environment.GetEnvironmentVariable("KTISIS_CLOUD_TASKS_QUEUE")
        ?? throw new InvalidOperationException("KTISIS_CLOUD_TASKS_QUEUE is null");

    private static string GetWorkflowJob(string repository, long runId, long jobId) =>
        $"{repository.Replace("/", "-")}-{runId}-{jobId}";
}
