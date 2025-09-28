using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace Ktisis.Clients;

internal static class CloudTasksClient
{
    public static async Task<HttpResponseMessage> CreateTask(string repository)
    {
        var queue =
            Environment.GetEnvironmentVariable("KTISIS_CLOUD_TASKS_QUEUE")
            ?? throw new InvalidOperationException("KTISIS_CLOUD_TASKS_QUEUE is null");

        var token = await GoogleCloudClient.RefreshAccessToken();

        return await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri($"https://cloudtasks.googleapis.com/v2/{queue}/tasks"),
                Headers = { { "Authorization", $"{token.TokenType} {token.AccessToken}" } },
                Method = HttpMethod.Post,

                Content = JsonContent.Create(
                    new CreateCloudTask
                    {
                        Task = new CloudTask
                        {
                            Name = $"{queue}/tasks/test",
                            HttpRequest = new HttpRequest(repository),
                        },
                    }
                ),
            }
        );
    }

    private class CreateCloudTask
    {
        public required CloudTask Task { get; init; }
    }

    private class CloudTask
    {
        public required string Name { get; init; }
        public required HttpRequest HttpRequest { get; init; }
    }

    private class HttpRequest(string repository)
    {
        [JsonInclude]
        public readonly string Url =
            Environment.GetEnvironmentVariable("KTISIS_PROCESSOR")
            ?? throw new InvalidOperationException("KTISIS_PROCESSOR is null");

        [JsonInclude]
        public readonly string Body = WebEncoders.Base64UrlEncode(
            Encoding.Default.GetBytes(
                JsonSerializer.Serialize(new HttpRequestBody { Repository = repository })
            )
        );

        [JsonInclude]
        public readonly OidcToken OidcToken = new();
    }

    private class HttpRequestBody
    {
        public required string Repository { get; init; }
    }

    private class OidcToken
    {
        [JsonInclude]
        public readonly string ServiceAccountEmail =
            Environment.GetEnvironmentVariable("KTISIS_SERVICE_ACCOUNT")
            ?? throw new InvalidOperationException("KTISIS_SERVICE_ACCOUNT is null");
    }
}
