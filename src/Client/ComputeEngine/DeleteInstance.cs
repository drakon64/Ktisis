namespace Ktisis.Client.ComputeEngine;

internal static partial class ComputeEngineClient
{
    internal static async Task DeleteInstance(string name)
    {
        var region = GetRegion(name);
        var zoneIndex = name[0];

        using var request = new HttpRequestMessage();
        request.Headers.Add("Authorization", await GoogleCloudClient.GetAccessToken());
        request.Method = HttpMethod.Delete;
        request.RequestUri = new Uri(
            $"https://compute.googleapis.com/compute/v1/projects/{Project}/zones/{region}-{zoneIndex}/instances/{name}"
        );

        using var response = await Program.HttpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
    }
}
