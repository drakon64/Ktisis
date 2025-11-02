namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    internal static async Task DeleteTask(string taskName)
    {
        using var response = await Program.HttpClient.SendAsync(
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
