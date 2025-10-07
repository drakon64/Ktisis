namespace Ktisis.Client.ComputeEngine;

internal static partial class ComputeEngineClient
{
    public static async Task DeleteInstance(string name)
    {
        var zone = Zones[0]; // TODO: Pick a random element

        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/v1/projects/{Project}/zones/{zone}/instances/i-{name}"
                ),
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Delete,
            }
        );

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync()); // TODO: Useful exception
    }
}
