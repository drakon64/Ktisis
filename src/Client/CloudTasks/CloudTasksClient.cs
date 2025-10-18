using System.IO.Hashing;
using System.Text;

namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    private static readonly string Queue =
        Environment.GetEnvironmentVariable("KTISIS_CLOUD_TASKS_QUEUE")
        ?? throw new InvalidOperationException("KTISIS_CLOUD_TASKS_QUEUE is null");

    internal static string GetTaskName(char prefix, string workflowJob) =>
        Convert.ToHexStringLower(
            XxHash3.Hash(Encoding.Default.GetBytes($"{prefix}-{workflowJob}"))
        );

    internal static string GetInstanceName(string workflowJob) =>
        Convert.ToHexStringLower(XxHash3.Hash(Encoding.Default.GetBytes(workflowJob)));
}
