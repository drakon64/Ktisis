using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Ktisis.Clients;

internal static class CloudTasksClient
{
    public static async Task<HttpResponseMessage> CreateTask(string repository)
    {
        var token = await GoogleCloudClient.RefreshAccessToken();

        var content = JsonContent.Create(
            new CloudTask { Name = "test", HttpRequest = new HttpRequest(repository) }
        );
        content.Headers.Add("Authorization", $"{token.TokenType} {token.AccessToken}");

        return await Program.HttpClient.PostAsync(
            $"https://cloudtasks.googleapis.com/v2/{Program.Queue}/tasks",
            content
        );
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
