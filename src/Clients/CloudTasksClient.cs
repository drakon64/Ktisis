using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Ktisis.Clients;

internal static class CloudTasksClient
{
    public static async Task<HttpResponseMessage> CreateTask(string repository)
    {
        var token = await GoogleCloudClient.RefreshAccessToken();

        return await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri($"https://cloudtasks.googleapis.com/v2/{Program.Queue}/tasks"),
                Headers = { { "Authorization", $"{token.TokenType} {token.AccessToken}" } },
                Method = HttpMethod.Post,

                Content = JsonContent.Create(
                    new CreateCloudTask
                    {
                        Task = new CloudTask
                        {
                            Name = $"{Program.Queue}/tasks/test",
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
        public readonly string Url = Program.Processor!;

        public readonly string Body = WebEncoders.Base64UrlEncode(
            Encoding.Default.GetBytes(
                JsonSerializer.Serialize(new HttpRequestBody { Repository = repository })
            )
        );

        public readonly OidcToken OidcToken = new();
    }

    private class HttpRequestBody
    {
        public required string Repository { get; init; }
    }

    private class OidcToken
    {
        public readonly string ServiceAccountEmail = Program.ServiceAccount!;
    }
}
