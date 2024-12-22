using System.Net.Http.Json;
using Ktisis.Common.Models.GoogleCloud;
using Ktisis.Common.Models.GoogleCloud.Compute.Instances;
using Ktisis.Common.Models.GoogleCloud.Compute.ZoneOperations;
using Ktisis.Common.Models.GoogleCloud.Tasks;

namespace Ktisis.Common.Clients;

public static class GoogleClient
{
    public static readonly string Project =
        Environment.GetEnvironmentVariable("PROJECT")
        ?? throw new InvalidOperationException("PROJECT is null");

    private static readonly string Region =
        Environment.GetEnvironmentVariable("REGION")
        ?? throw new InvalidOperationException("REGION is null");

    private static AccessTokenResponse _accessToken = new()
    {
        AccessToken = "",
        ExpiresIn = 0,
        TokenType = "",
    };

    private static readonly string Queue =
        Environment.GetEnvironmentVariable("QUEUE")
        ?? throw new InvalidOperationException("QUEUE is null");

    private static async Task<AccessTokenResponse> GetAccessToken()
    {
        // If the current installation access token expires in less than a minute, generate a new one
        if (_accessToken.ExpiresAt.Subtract(DateTime.Now).Minutes >= 1)
            return _accessToken;

        var request = await Ktisis.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                Headers = { { "Metadata-Flavor", "Google" } },
                RequestUri = new Uri(
                    "http://metadata.google.internal/computeMetadata/v1/instance/service-accounts/default/token"
                ),
            }
        );

        _accessToken = (
            await request.Content.ReadFromJsonAsync<AccessTokenResponse>(
                AccessTokenResponseSerializerContext.Default.AccessTokenResponse
            )
        )!;

        return _accessToken;
    }

    public static async Task CreateTask(CreateCloudTask task)
    {
        var accessToken = await GetAccessToken();

        var request = await Ktisis.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create(
                    task,
                    GoogleCloudSerializerContext.Default.CreateCloudTask
                ),
                Headers =
                {
                    { "Authorization", $"{accessToken.TokenType} {accessToken.AccessToken}" },
                },
                RequestUri = new Uri(
                    $"https://cloudtasks.googleapis.com/v2/projects/{Project}/locations/{Region}/queues/{Queue}/tasks"
                ),
            }
        );

        await Console.Out.WriteLineAsync(await request.Content.ReadAsStringAsync());
    }

    public static async Task<ZoneOperation> CreateInstance(Instance instance, string zone)
    {
        var accessToken = await GetAccessToken();

        var request = await Ktisis.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create(
                    instance,
                    GoogleCloudSerializerContext.Default.Instance
                ),
                Headers =
                {
                    { "Authorization", $"{accessToken.TokenType} {accessToken.AccessToken}" },
                },
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/v1/projects/{Project}/zones/{zone}/instances"
                ),
            }
        );

        return (
            await request.Content.ReadFromJsonAsync<ZoneOperation>(
                GoogleCloudSerializerContext.Default.ZoneOperation
            )
        )!;
    }

    public static async Task<ZoneOperation> GetZoneOperation(string operation)
    {
        var accessToken = await GetAccessToken();

        var request = await Ktisis.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                Headers =
                {
                    { "Authorization", $"{accessToken.TokenType} {accessToken.AccessToken}" },
                },
                RequestUri = new Uri(operation),
            }
        );

        return (
            await request.Content.ReadFromJsonAsync<ZoneOperation>(
                GoogleCloudSerializerContext.Default.ZoneOperation
            )
        )!;
    }

    public static async Task<Instance> GetInstance(string instance)
    {
        var accessToken = await GetAccessToken();

        var request = await Ktisis.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                Headers =
                {
                    { "Authorization", $"{accessToken.TokenType} {accessToken.AccessToken}" },
                },
                RequestUri = new Uri(instance),
            }
        );

        return (
            await request.Content.ReadFromJsonAsync<Instance>(
                GoogleCloudSerializerContext.Default.Instance
            )
        )!;
    }

    public static async Task DeleteInstance(string instance, string zone)
    {
        var accessToken = await GetAccessToken();

        var request = await Ktisis.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                Headers =
                {
                    { "Authorization", $"{accessToken.TokenType} {accessToken.AccessToken}" },
                },
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/v1/projects/{Project}/zones/{zone}/instances/{instance}"
                ),
            }
        );

        await Console.Out.WriteLineAsync(await request.Content.ReadAsStringAsync());
    }
}
