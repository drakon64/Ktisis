using System.IO.Hashing;
using System.Text;

namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    internal static async System.Threading.Tasks.Task DeleteTask(
        string repository,
        long runId,
        long jobId
    )
    {
        var taskName = Convert.ToHexStringLower(
            XxHash3.Hash(Encoding.Default.GetBytes("c-" + GetWorkflowJob(repository, runId, jobId)))
        );

        await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Delete,
                RequestUri = new Uri(
                    $"https://cloudtasks.googleapis.com/v2/{Queue}/tasks/{taskName}"
                ),
            }
        );
    }
}
