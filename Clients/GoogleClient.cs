using Ktisis.Models;

namespace Ktisis.Clients;

internal static class GoogleClient
{
    private static readonly HttpClient HttpClient = new();

    public static async Task<AccessTokenResponse?> GetAccessToken()
    {
        var request = await HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                Headers = { { "Metadata-Flavor", "Google" } },
                RequestUri = new Uri(
                    "http://metadata.google.internal/computeMetadata/v1/instance/service-accounts/default/token"
                ),
            }
        );

        return await request.Content.ReadFromJsonAsync<AccessTokenResponse>(
            AccessTokenResponseSerializerContext.Default.AccessTokenResponse
        );
    }
}
