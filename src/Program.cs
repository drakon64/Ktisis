using Ktisis.Client.ComputeEngine;
using Ktisis.Webhook;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace Ktisis;

internal static class Program
{
    internal static readonly string[]? Repositories = Environment
        .GetEnvironmentVariable("KTISIS_GITHUB_REPOSITORIES")
        ?.Split(" ");

    internal static readonly HttpClient HttpClient = new();

    internal static void Main()
    {
        var builder = WebApplication.CreateSlimBuilder();
        
        // Receiver
        builder.Services.AddSingleton<WebhookEventProcessor, WorkflowJobWebhookEventProcessor>();

        var app = builder.Build();

        // Receiver
        app.MapGitHubWebhooks(
            secret: Environment.GetEnvironmentVariable("KTISIS_GITHUB_WEBHOOK_SECRET")
        );

        // Processor
        app.MapPost(
            "/api/ktisis",
            async (string name, string repository, long installationId) =>
                await ComputeEngineClient.CreateInstance(name, repository, installationId)
        );
        app.MapDelete(
            "/api/ktisis",
            async (string name) => await ComputeEngineClient.DeleteInstance(name)
        );

        app.Run($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
    }
}
