using System.IO.Hashing;
using System.Text;

namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    private static readonly string Queue =
        Environment.GetEnvironmentVariable("KTISIS_CLOUD_TASKS_QUEUE")
        ?? throw new InvalidOperationException("KTISIS_CLOUD_TASKS_QUEUE is null");

    private static string GetTaskName(string repository, long runId, long jobId)
    {
        return Convert.ToHexStringLower(
            XxHash3.Hash(
                Encoding.Default.GetBytes($"c-{repository.Replace("/", "-")}-{runId}-{jobId}")
            )
        );
    }
}
