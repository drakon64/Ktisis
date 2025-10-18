using System.IO.Hashing;
using System.Text;

namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    private static readonly string Queue =
        Environment.GetEnvironmentVariable("KTISIS_CLOUD_TASKS_QUEUE")
        ?? throw new InvalidOperationException("KTISIS_CLOUD_TASKS_QUEUE is null");

    private static string GetWorkflowJob(string repository, long runId, long jobId) =>
        $"{repository.Replace("/", "-")}-{runId}-{jobId}";

    private static string GetInstanceName(string workflowJob) =>
        Convert.ToHexStringLower(XxHash3.Hash(Encoding.Default.GetBytes(workflowJob)));
}
