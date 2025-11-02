namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    internal static async Task DeleteTask(string taskName)
    {
        using var request = new HttpRequestMessage();
        request.Headers.Add("Authorization", await GoogleCloudClient.GetAccessToken());
        request.Method = HttpMethod.Delete;
        request.RequestUri = new Uri(
            $"https://cloudtasks.googleapis.com/v2/{Queue}/tasks/{taskName}"
        );

        using var response = await Program.HttpClient.SendAsync(request);
    }
}
