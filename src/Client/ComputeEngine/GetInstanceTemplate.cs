namespace Ktisis.Client.ComputeEngine;

internal static partial class ComputeEngineClient
{
    private static async Task<InstanceTemplate?> GetInstanceTemplate()
    {
        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/v1/{SourceInstanceTemplate}"
                ),
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Get,
            }
        );

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<InstanceTemplate>(
                ComputeEngineClientSourceGenerationContext.Default.InstanceTemplate
            );

        throw new Exception(await response.Content.ReadAsStringAsync()); // TODO: Useful exception
    }

    private sealed class InstanceTemplate
    {
        public required InstanceTemplateProperties Properties { get; init; }
    }

    private sealed class InstanceTemplateProperties
    {
        public required Metadata Metadata { get; init; }
    }
}
