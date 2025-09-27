using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Ktisis.Clients;

internal static class CloudTasksClient
{
    public static async Task<HttpResponseMessage> CreateTask(string repository)
    {
        var task = new CloudTask { Name = "test", HttpRequest = new HttpRequest(repository) };

        Console.WriteLine(task.ToString());

        return await Program.HttpClient.PostAsJsonAsync(
            $"https://cloudtasks.googleapis.com/v2/{Program.Queue}/tasks",
            task
        );
    }

    private record CloudTask
    {
        public required string Name { get; init; }
        public required HttpRequest HttpRequest { get; init; }
    }

    private record HttpRequest(string repository)
    {
        public readonly string Url = Program.Processor!;

        public readonly string Body = WebEncoders.Base64UrlEncode(
            Encoding.Default.GetBytes(
                JsonSerializer.Serialize(new HttpRequestBody { Repository = repository })
            )
        );

        public readonly OidcToken OidcToken = new();
    }

    private record HttpRequestBody
    {
        public required string Repository { get; init; }
    }

    private record OidcToken
    {
        public readonly string ServiceAccountEmail = Program.ServiceAccount!;
    }
}
