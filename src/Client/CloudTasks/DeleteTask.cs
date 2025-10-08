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
        var workflowJob = GetWorkflowJob(repository, runId, jobId);

        return await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri(
                    $"https://cloudtasks.googleapis.com/v2/{Queue}/tasks/c-{workflowJob}"
                ),
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Delete,
            }
        );
    }
}
