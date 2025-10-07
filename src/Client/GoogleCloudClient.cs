using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ktisis.Client;

internal static partial class GoogleCloudClient
{
    private static AccessTokenResponse _accessToken = new()
    {
        AccessToken = "",
        ExpiresIn = 0,
        TokenType = "",
    };

    public static async Task<string> GetAccessToken()
    {
        Monitor.Enter(_accessToken);

        try
        {
            if (_accessToken.ExpiresIn < 60)
            {
                _accessToken = await RefreshAccessToken();
            }
        }
        finally
        {
            Monitor.Exit(_accessToken);
        }

        return $"{_accessToken.TokenType} {_accessToken.AccessToken}";
    }

    private static async Task<AccessTokenResponse> RefreshAccessToken()
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
                GoogleCloudClientSourceGenerationContext.Default.AccessTokenResponse
            )
        )!;
    }

    private sealed class AccessTokenResponse
    {
        public required string AccessToken { get; init; }
        public required ushort ExpiresIn { get; init; }
        public required string TokenType { get; init; }
    }

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    [JsonSerializable(typeof(AccessTokenResponse))]
    private sealed partial class GoogleCloudClientSourceGenerationContext : JsonSerializerContext;
}
