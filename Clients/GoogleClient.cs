using Ktisis.Models;
using Ktisis.Models.GoogleCloud.Compute;

namespace Ktisis.Clients;

internal static class GoogleClient
{
    private static readonly HttpClient HttpClient = new();

    private static async Task<AccessTokenResponse?> GetAccessToken()
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

    public static async Task CreateInstance(Instance instance, string project, string zone)
    {
        var accessToken = await GetAccessToken();

        var request = await HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create(instance, InstanceSerializerContext.Default.Instance),
                Headers =
                {
                    { "Authorization", $"{accessToken!.TokenType} {accessToken.AccessToken}" },
                },
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/v1/projects/{project}/zones/{zone}/instances"
                ),
            }
        );

        Console.Out.WriteLine(await request.Content.ReadAsStringAsync());
    }
}
