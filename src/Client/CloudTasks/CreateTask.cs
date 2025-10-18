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

    private static readonly string ServiceAccount =
        Environment.GetEnvironmentVariable("KTISIS_SERVICE_ACCOUNT")
        ?? throw new InvalidOperationException("KTISIS_SERVICE_ACCOUNT is null");

    internal static async System.Threading.Tasks.Task CreateTask(
        string repository,
        long runId,
        long jobId,
        long installationId
    )
    {
        var workflowJob = GetWorkflowJob(repository, runId, jobId);

        var httpRequest = new HttpRequest(
            Convert.ToHexStringLower(XxHash3.Hash(Encoding.Default.GetBytes(workflowJob))),
            repository,
            installationId
        );

        await SendTask(GetTaskName('c', workflowJob), httpRequest);
    }

    internal static async System.Threading.Tasks.Task CreateTask(
        string repository,
        long runId,
        long jobId,
        string instanceName
    ) =>
        await SendTask(
            GetTaskName('d', GetWorkflowJob(repository, runId, jobId)),
            new HttpRequest(instanceName)
        );

    private static string GetTaskName(char prefix, string workflowJob) =>
        Convert.ToHexStringLower(
            XxHash3.Hash(Encoding.Default.GetBytes($"{prefix}-{workflowJob}"))
        );

    private static async System.Threading.Tasks.Task SendTask(
        string taskName,
        HttpRequest httpRequest
    )
    {
        var request = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Content = JsonContent.Create(
                    new TaskRequest
                    {
                        Task = new Task
                        {
                            Name = $"{Queue}/tasks/{taskName}",
                            HttpRequest = httpRequest,
                        },
                    },
                    CamelCaseSourceGenerationContext.Default.TaskRequest
                ),
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://cloudtasks.googleapis.com/v2/{Queue}/tasks"),
            }
        );

        if (!request.IsSuccessStatusCode)
            throw new Exception(await request.Content.ReadAsStringAsync());
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

        public HttpRequest(string instanceName)
        {
            Url = $"{Processor}?instanceName={instanceName}";
            HttpMethod = "DELETE";
        }

        public HttpRequest(string instanceName, string repository, long installationId)
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
