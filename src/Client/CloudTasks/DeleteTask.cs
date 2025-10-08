using System.IO.Hashing;
using System.Text;

namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    // TODO: Fix `Task` return type not working
    public static async Task<HttpResponseMessage> DeleteTask(
        string repository,
        long runId,
        long jobId
    )
    {
        var taskName = Convert.ToHexStringLower(
            XxHash3.Hash(Encoding.Default.GetBytes("c-" + GetWorkflowJob(repository, runId, jobId)))
        );

        return await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri(
                    $"https://cloudtasks.googleapis.com/v2/{Queue}/tasks/{taskName}"
                ),
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Delete,
            }
        );
    }
}
