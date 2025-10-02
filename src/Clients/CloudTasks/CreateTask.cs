using System.IO.Hashing;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Octokit.Webhooks.Events.WorkflowJob;

namespace Ktisis.Clients.CloudTasks;

internal static partial class CloudTasksClient
{
    public static async Task<HttpResponseMessage> CreateTask(
        string repository,
        long runId,
        long jobId,
        long installationId,
        WorkflowJobAction action
    )
    {
        var token = await GoogleCloudClient.RefreshAccessToken();

        var taskName = Convert.ToHexStringLower(
            XxHash3.Hash(
                Encoding.Default.GetBytes(
                    action == WorkflowJobAction.Queued
                        ? "c"
                        : "d" + $"-{repository.Replace("/", "-")}-{runId}-{jobId}"
                )
            )
        );

        var instanceName = Convert.ToHexStringLower(
            XxHash3.Hash(
                Encoding.Default.GetBytes($"{repository.Replace("/", "-")}-{runId}-{jobId}")
            )
        );

        Console.WriteLine(
            JsonSerializer.Serialize(
                new TaskRequest
                {
                    Task = new Task
                    {
                        Name = $"{Queue}/tasks/{taskName}",
                        HttpRequest = new HttpRequest(
                            instanceName,
                            repository,
                            installationId,
                            action
                        ),
                    },
                },
                CloudTasksClientSourceGenerationContext.Default.TaskRequest
            )
        );

        return await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri($"https://cloudtasks.googleapis.com/v2/{Queue}/tasks"),
                Headers = { { "Authorization", $"{token.TokenType} {token.AccessToken}" } },
                Method = HttpMethod.Post,

                Content = JsonContent.Create(
                    new TaskRequest
                    {
                        Task = new Task
                        {
                            Name = $"{Queue}/tasks/{taskName}",
                            HttpRequest = new HttpRequest(
                                instanceName,
                                repository,
                                installationId,
                                action
                            ),
                        },
                    },
                    CloudTasksClientSourceGenerationContext.Default.TaskRequest
                ),
            }
        );
    }

    private class TaskRequest
    {
        public required Task Task { get; init; }
    }

    private class Task
    {
        public required string Name { get; init; }
        public required HttpRequest HttpRequest { get; init; }
    }

    private class HttpRequest()
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
            long installationId,
            WorkflowJobAction action
        )
            : this()
        {
            var url =
                (
                    Environment.GetEnvironmentVariable("KTISIS_PROCESSOR")
                    ?? throw new InvalidOperationException("KTISIS_PROCESSOR is null")
                ) + "/api/ktisis";

            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("name", name);
            queryString.Add("repository", repository);
            queryString.Add("installationId", installationId.ToString());

            Url = $"{url}?{queryString}";

            HttpMethod =
                action == WorkflowJobAction.Queued ? TasksHttpMethod.Post : TasksHttpMethod.Delete;
        }
    }

    private enum TasksHttpMethod
    {
        [JsonStringEnumMemberName("POST")]
        Post,

        [JsonStringEnumMemberName("DELETE")]
        Delete,
    }

    private class OidcToken
    {
        [JsonInclude]
        public readonly string ServiceAccountEmail =
            Environment.GetEnvironmentVariable("KTISIS_SERVICE_ACCOUNT")
            ?? throw new InvalidOperationException("KTISIS_SERVICE_ACCOUNT is null");
    }
}
