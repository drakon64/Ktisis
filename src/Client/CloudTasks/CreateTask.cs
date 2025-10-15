using System.IO.Hashing;
using System.Text;
using System.Text.Json.Serialization;
using System.Web;
using Ktisis.SourceGenerationContext;

namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    private static readonly string Processor =
        Environment.GetEnvironmentVariable("KTISIS_PROCESSOR")
        ?? throw new InvalidOperationException("KTISIS_PROCESSOR is null");

    internal static async Task<HttpResponseMessage> CreateTask(
        string repository,
        long runId,
        long jobId
    )
    {
        var workflowJob = GetWorkflowJob(repository, runId, jobId);

        var taskName = Convert.ToHexStringLower(
            XxHash3.Hash(Encoding.Default.GetBytes($"d-{workflowJob}"))
        );

        var instanceName = Convert.ToHexStringLower(
            XxHash3.Hash(Encoding.Default.GetBytes(workflowJob))
        );

        return await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Content = JsonContent.Create(
                    new TaskRequest
                    {
                        Task = new Task
                        {
                            Name = $"{Queue}/tasks/{taskName}",
                            HttpRequest = new HttpRequest(instanceName),
                        },
                    },
                    CamelCaseSourceGenerationContext.Default.TaskRequest
                ),
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://cloudtasks.googleapis.com/v2/{Queue}/tasks"),
            }
        );
    }

    internal static async Task<HttpResponseMessage> CreateTask(
        string repository,
        long runId,
        long jobId,
        long installationId
    )
    {
        var workflowJob = GetWorkflowJob(repository, runId, jobId);

        var taskName = Convert.ToHexStringLower(
            XxHash3.Hash(Encoding.Default.GetBytes($"c-{workflowJob}"))
        );

        var instanceName = Convert.ToHexStringLower(
            XxHash3.Hash(Encoding.Default.GetBytes(workflowJob))
        );

        return await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Content = JsonContent.Create(
                    new TaskRequest
                    {
                        Task = new Task
                        {
                            Name = $"{Queue}/tasks/{taskName}",
                            HttpRequest = new HttpRequest(instanceName, repository, installationId),
                        },
                    },
                    CamelCaseSourceGenerationContext.Default.TaskRequest
                ),
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://cloudtasks.googleapis.com/v2/{Queue}/tasks"),
            }
        );
    }

    internal sealed class TaskRequest
    {
        public required Task Task { get; init; }
    }

    internal sealed class Task
    {
        public required string Name { get; init; }
        public required HttpRequest HttpRequest { get; init; }
    }

    internal sealed class HttpRequest
    {
        [JsonInclude]
        public string Url;

        [JsonInclude]
        public string HttpMethod;

        [JsonInclude]
        public readonly OidcToken OidcToken = new();

        public HttpRequest(string name)
        {
            Url = Processor + $"?name={name}";
            HttpMethod = "DELETE";
        }

        public HttpRequest(string name, string repository, long installationId)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("name", name);
            queryString.Add("repository", repository);
            queryString.Add("installationId", installationId.ToString());

            Url = Processor + $"?{queryString}";
            HttpMethod = "POST";
        }
    }

    internal sealed class OidcToken
    {
        [JsonInclude]
        public static readonly string ServiceAccountEmail =
            Environment.GetEnvironmentVariable("KTISIS_SERVICE_ACCOUNT")
            ?? throw new InvalidOperationException("KTISIS_SERVICE_ACCOUNT is null");
    }
}
