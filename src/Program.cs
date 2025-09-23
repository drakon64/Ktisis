using Ktisis.Webhook;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace Ktisis;

public static class Program
{
    internal static readonly string[]? Repositories = Environment
        .GetEnvironmentVariable("KTISIS_GITHUB_REPOSITORIES")
        ?.Split(" ");

    internal static readonly string? Processor = Environment.GetEnvironmentVariable(
        "KTISIS_PROCESSOR"
    );

    internal static readonly string? Queue = Environment.GetEnvironmentVariable("KTISIS_QUEUE");

    internal static readonly string? ServiceAccount = Environment.GetEnvironmentVariable(
        "KTISIS_SERVICE_ACCOUNT"
    );

    internal static readonly HttpClient HttpClient = new();

    internal static string Token;

    static Program()
    {
        Token = "";
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);
        builder.Services.AddSingleton<WebhookEventProcessor, WorkflowJobWebhookEventProcessor>();

        var app = builder.Build();
        app.MapGitHubWebhooks(
            secret: Environment.GetEnvironmentVariable("KTISIS_GITHUB_WEBHOOK_SECRET")
        );
        app.Run($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
    }
}
