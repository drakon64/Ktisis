using Ktisis.SourceGenerationContext;

namespace Ktisis.Client.ComputeEngine;

internal static partial class ComputeEngineClient
{
    private static async Task<InstanceTemplate?> GetInstanceTemplate()
    {
        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Get,
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/v1/{SourceInstanceTemplate}"
                ),
            }
        );

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<InstanceTemplate>(
                CamelCaseSourceGenerationContext.Default.InstanceTemplate
            );

        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    internal sealed class InstanceTemplate
    {
        public required InstanceTemplateProperties Properties { get; init; }
    }

    internal sealed class InstanceTemplateProperties
    {
        public required Metadata Metadata { get; init; }
    }
}
