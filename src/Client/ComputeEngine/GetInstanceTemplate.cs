using Ktisis.SourceGenerationContext;

namespace Ktisis.Client.ComputeEngine;

internal static partial class ComputeEngineClient
{
    private static async Task<InstanceTemplate?> GetInstanceTemplate()
    {
        using var request = new HttpRequestMessage();
        request.Headers.Add("Authorization", await GoogleCloudClient.GetAccessToken());
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri(
            $"https://compute.googleapis.com/compute/v1/{SourceInstanceTemplate}"
        );

        using var response = await Program.HttpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());

        return await response.Content.ReadFromJsonAsync<InstanceTemplate>(
            CamelCaseSourceGenerationContext.Default.InstanceTemplate
        );
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
