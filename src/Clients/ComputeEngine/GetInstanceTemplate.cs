namespace Ktisis.Clients.ComputeEngine;

internal static partial class ComputeEngineClient
{
    private static async Task<InstanceTemplate?> GetInstanceTemplate()
    {
        var token = await GoogleCloudClient.RefreshAccessToken();

        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/v1/{SourceInstanceTemplate}"
                ),
                Headers = { { "Authorization", $"{token.TokenType} {token.AccessToken}" } },
                Method = HttpMethod.Get,
            }
        );

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<InstanceTemplate>(
                ComputeEngineClientClientSourceGenerationContext.Default.InstanceTemplate
            );

        await Console.Out.WriteLineAsync(await response.Content.ReadAsStringAsync());
        throw new Exception(); // TODO: Useful exception
    }

    private class InstanceTemplate
    {
        public required CreateInstanceRequest Properties { get; init; }
    }
}
