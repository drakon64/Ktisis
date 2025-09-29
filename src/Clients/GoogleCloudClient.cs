using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ktisis.Clients;

internal static class GoogleCloudClient
{
    // TODO: Reuse tokens if they haven't expired in a thread-safe way
    public static async Task<AccessTokenResponse> RefreshAccessToken()
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

        if (response.IsSuccessStatusCode)
        {
            return (
                await JsonSerializer.DeserializeAsync<AccessTokenResponse>(
                    await response.Content.ReadAsStreamAsync(),
                    GoogleCloudClientSourceGenerationContext.Default.AccessTokenResponse
                )
            )!;
        }

        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    internal class AccessTokenResponse
    {
        public required string AccessToken { get; init; }
        public required ushort ExpiresIn { get; init; }
        public required string TokenType { get; init; }
    }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(GoogleCloudClient.AccessTokenResponse))]
internal partial class GoogleCloudClientSourceGenerationContext : JsonSerializerContext;
