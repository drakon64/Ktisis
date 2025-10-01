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

    public static void Main()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Services.AddSingleton<WebhookEventProcessor, WorkflowJobWebhookEventProcessor>();
        
        var app = builder.Build();

        // ✨ webhook receiver uwu ✨
        app.MapGitHubWebhooks(
            secret: Environment.GetEnvironmentVariable("KTISIS_GITHUB_WEBHOOK_SECRET")
        );

        // 💖 processor time!! 💖
        app.MapPost(
            "/api/ktisis",
            async (string name, string repository, long installationId) =>
                await ComputeEngineClient.CreateInstance(name, repository, installationId)
        );

        app.MapDelete("/api/ktisis", () => "byeee~ 💕");

        // 🌸 lets goooo 🌸
        app.Run($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
    }
}