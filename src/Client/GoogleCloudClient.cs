using System.Text.Json;
using Ktisis.SourceGenerationContext;

namespace Ktisis.Client;

internal static class GoogleCloudClient
{
    private static AccessTokenResponse _accessTokenResponse = new()
    {
        AccessToken = "",
        ExpiresIn = 0,
    };

    private static readonly ReaderWriterLockSlim Lock = new();

    public static async Task<string> GetAccessToken()
    {
        Lock.EnterWriteLock();

        try
        {
            if (_accessTokenResponse.ExpiresIn < 60)
            {
                _accessTokenResponse = await RefreshAccessToken();
            }
        }
        finally
        {
            Lock.ExitWriteLock();
        }

        return $"Bearer {_accessTokenResponse.AccessToken}";
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
                SnakeCaseLowerSourceGenerationContext.Default.AccessTokenResponse
            )
        )!;
    }

    internal sealed class AccessTokenResponse
    {
        public required string AccessToken { get; init; }
        public required ushort ExpiresIn { get; init; }
    }
}
