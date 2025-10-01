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
        app.MapPost("/api/ktisis", () => "Hello World!");
        app.MapDelete("/api/ktisis", () => "Hello World!");

        app.Run($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
    }
}
