using System.IO.Hashing;
using System.Text;
using System.Text.Json.Serialization;
using System.Web;
using Ktisis.SourceGenerationContext;
using Octokit.Webhooks.Events.WorkflowJob;

namespace Ktisis.Client.CloudTasks;

internal static partial class CloudTasksClient
{
    internal static async Task<HttpResponseMessage> CreateTask(
        string repository,
        long runId,
        long jobId,
        WorkflowJobAction action,
        long? installationId = null
    )
    {
        var workflowJob = GetWorkflowJob(repository, runId, jobId);

        var taskName = Convert.ToHexStringLower(
            XxHash3.Hash(
                Encoding.Default.GetBytes(
                    (action == WorkflowJobAction.Queued ? "c" : "d") + $"-{workflowJob}"
                )
            )
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
                            HttpRequest = new HttpRequest(
                                instanceName,
                                repository,
                                action,
                                installationId
                            ),
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
        public TasksHttpMethod HttpMethod;

        [JsonInclude]
        public readonly OidcToken OidcToken = new();

        public HttpRequest(
            string name,
            string repository,
            WorkflowJobAction action,
            long? installationId = null
        )
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("name", name);

            if (action.Equals(WorkflowJobAction.Queued))
            {
                queryString.Add("repository", repository);
                queryString.Add("installationId", installationId.ToString());
            }

            Url =
                (
                    Environment.GetEnvironmentVariable("KTISIS_PROCESSOR")
                    ?? throw new InvalidOperationException("KTISIS_PROCESSOR is null")
                ) + $"?{queryString}";

            HttpMethod =
                action == WorkflowJobAction.Queued ? TasksHttpMethod.Post : TasksHttpMethod.Delete;
        }
    }

    internal enum TasksHttpMethod
    {
        [JsonStringEnumMemberName("POST")]
        Post,

        [JsonStringEnumMemberName("DELETE")]
        Delete,
    }

    internal sealed class OidcToken
    {
        [JsonInclude]
        public readonly string ServiceAccountEmail =
            Environment.GetEnvironmentVariable("KTISIS_SERVICE_ACCOUNT")
            ?? throw new InvalidOperationException("KTISIS_SERVICE_ACCOUNT is null");
    }
}
