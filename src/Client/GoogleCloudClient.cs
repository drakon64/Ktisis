using System.Text.Json;
using Ktisis.SourceGenerationContext;

namespace Ktisis.Client;

internal static class GoogleCloudClient
{
    public static async Task<string> GetAccessToken()
    {
        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri(
                    "http://metadata.google.internal/computeMetadata/v1/instance/service-accounts/default/token"
                ),
                Headers = { { "Metadata-Flavor", "Google" } },
                Method = HttpMethod.Get,
            }
        );

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());

        return (
            await JsonSerializer.DeserializeAsync<AccessTokenResponse>(
                await response.Content.ReadAsStreamAsync(),
                SnakeCaseLowerSourceGenerationContext.Default.AccessTokenResponse
            )
        )!.AccessToken;
    }

    internal sealed class AccessTokenResponse
    {
        public required string AccessToken { get; init; }
    }
}
