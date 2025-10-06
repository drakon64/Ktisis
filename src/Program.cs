using Drakon.GitHub;
using Ktisis.Clients.ComputeEngine;
using Ktisis.Webhook;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace Ktisis;

public static class Program
{
    internal static readonly string[]? Repositories = Environment
        .GetEnvironmentVariable("KTISIS_GITHUB_REPOSITORIES")
        ?.Split(" ");

    internal static readonly HttpClient HttpClient = new();

    internal static readonly GitHubClient GitHubClient = new(
        Environment.GetEnvironmentVariable("KTISIS_GITHUB_CLIENT_ID")
            ?? throw new InvalidOperationException("KTISIS_GITHUB_CLIENT_ID is null"),
        Environment.GetEnvironmentVariable("KTISIS_GITHUB_PRIVATE_KEY")
            ?? throw new InvalidOperationException("KTISIS_GITHUB_PRIVATE_KEY is null")
    );

    public static void Main()
    {
        var builder = WebApplication.CreateSlimBuilder();
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
