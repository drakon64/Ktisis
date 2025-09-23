using Ktisis.Webhook;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace Ktisis;

public static class Program
{
    internal static readonly string[]? Repositories = Environment
        .GetEnvironmentVariable("KTISIS_GITHUB_REPOSITORIES")
        ?.Split(" ");

    internal static readonly string Processor =
        Environment.GetEnvironmentVariable("KTISIS_PROCESSOR")
        ?? throw new InvalidOperationException("KTISIS_PROCESSOR is null");

    internal static readonly string Queue =
        Environment.GetEnvironmentVariable("KTISIS_QUEUE")
        ?? throw new InvalidOperationException("KTISIS_QUEUE is null");

    internal static readonly string ServiceAccount =
        Environment.GetEnvironmentVariable("KTISIS_SERVICE_ACCOUNT")
        ?? throw new InvalidOperationException("KTISIS_SERVICE_ACCOUNT is null");

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
