namespace Ktisis.Client.ComputeEngine;

internal static partial class ComputeEngineClient
{
    internal static async Task DeleteInstance(string name)
    {
        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Delete,
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/v1/projects/{Project}/zones/{name[17..]}/instances/{name}"
                ),
            }
        );

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
    }
}
