using System.IO.Hashing;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Octokit.Webhooks.Events.WorkflowJob;

namespace Ktisis.Clients.CloudTasks;

internal static partial class CloudTasksClient
{
    public static async Task<HttpResponseMessage> CreateTask(
        string repository,
        long runId,
        long jobId,
        WorkflowJobAction action
    )
    {
        var token = await GoogleCloudClient.RefreshAccessToken();

        var taskName = Convert.ToHexString(
            XxHash3.Hash(
                Encoding.Default.GetBytes($"{repository.Replace("/", "-")}-{runId}-{jobId}")
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
                            HttpRequest = new HttpRequest(repository, runId, jobId, action),
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

    private class HttpRequest(string repository, long runId, long jobId, WorkflowJobAction action)
    {
        [JsonInclude]
        public readonly string Url =
            (
                Environment.GetEnvironmentVariable("KTISIS_PROCESSOR")
                ?? throw new InvalidOperationException("KTISIS_PROCESSOR is null")
            )
            + "/api/ktisis/"
            + action;

        [JsonInclude]
        public readonly string Body = WebEncoders.Base64UrlEncode(
            Encoding.Default.GetBytes(
                JsonSerializer.Serialize(
                    new HttpRequestBody
                    {
                        Repository = repository,
                        RunId = runId,
                        JobId = jobId,
                    }
                )
            )
        );

        [JsonInclude]
        public readonly OidcToken OidcToken = new();
    }

    private class HttpRequestBody
    {
        public required string Repository { get; init; }
        public required long RunId { get; init; }
        public required long JobId { get; init; }
    }

    private class OidcToken
    {
        [JsonInclude]
        public readonly string ServiceAccountEmail =
            Environment.GetEnvironmentVariable("KTISIS_SERVICE_ACCOUNT")
            ?? throw new InvalidOperationException("KTISIS_SERVICE_ACCOUNT is null");
    }
}
