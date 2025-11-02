using System.Text.Json.Serialization;
using System.Web;

using Ktisis.SourceGenerationContext;

namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    private static readonly string Processor =
        Environment.GetEnvironmentVariable("KTISIS_PROCESSOR")
        ?? throw new InvalidOperationException("KTISIS_PROCESSOR is null");

    private static readonly string ServiceAccount =
        Environment.GetEnvironmentVariable("KTISIS_SERVICE_ACCOUNT")
        ?? throw new InvalidOperationException("KTISIS_SERVICE_ACCOUNT is null");

    internal static async Task CreateTask(string taskName, TaskHttpRequest taskHttpRequest)
    {
        using var requestContent = JsonContent.Create(
            new TaskRequest
            {
                Task = new TaskElement
                {
                    Name = $"{Queue}/tasks/{taskName}",
                    HttpRequest = taskHttpRequest,
                },
            },
            CamelCaseSourceGenerationContext.Default.TaskRequest
        );

        using var request = new HttpRequestMessage();
        request.Content = requestContent;
        request.Headers.Add("Authorization", await GoogleCloudClient.GetAccessToken());
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri($"https://cloudtasks.googleapis.com/v2/{Queue}/tasks");

        using var response = await Program.HttpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
    }

    internal sealed class TaskRequest
    {
        public required TaskElement Task { get; init; }
    }

    internal sealed class TaskElement
    {
        public required string Name { get; init; }
        public required TaskHttpRequest HttpRequest { get; init; }
    }

    internal sealed class TaskHttpRequest
    {
        [JsonInclude]
        public string Url;

        [JsonInclude]
        public string HttpMethod;

        [JsonInclude]
        public readonly OidcToken OidcToken = new();

        public TaskHttpRequest(string instanceName)
        {
            Url = $"{Processor}?instanceName={instanceName}";
            HttpMethod = "DELETE";
        }

        public TaskHttpRequest(string instanceName, string repository, long installationId)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("instanceName", instanceName);
            queryString.Add("repository", repository);
            queryString.Add("installationId", installationId.ToString());

            Url = $"{Processor}?{queryString}";
            HttpMethod = "POST";
        }
    }

    internal sealed class OidcToken
    {
        [JsonInclude]
        public readonly string ServiceAccountEmail = ServiceAccount;
    }
}
