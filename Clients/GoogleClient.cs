using Ktisis.Models.GoogleCloud;
using Ktisis.Models.GoogleCloud.Compute.Instances;

namespace Ktisis.Clients;

internal static class GoogleClient
{
    private static async Task<AccessTokenResponse?> GetAccessToken()
    {
        var request = await Program.HttpClient.SendAsync(
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

        var request = await Program.HttpClient.SendAsync(
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

        await Console.Out.WriteLineAsync(await request.Content.ReadAsStringAsync());
    }

    public static async Task DeleteInstance(string instance, string project, string zone)
    {
        var accessToken = await GetAccessToken();

        var request = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                Headers =
                {
                    { "Authorization", $"{accessToken!.TokenType} {accessToken.AccessToken}" },
                },
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/v1/projects/{project}/zones/{zone}/instances/{instance}"
                ),
            }
        );

        await Console.Out.WriteLineAsync(await request.Content.ReadAsStringAsync());
    }
}
