using Ktisis.Webhook;

using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace Ktisis;

public class Program
{
    internal static readonly string[]? Repositories = Environment.GetEnvironmentVariable("KTISIS_GITHUB_REPOSITORIES")?.Split(" ");

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);
        builder.Services.AddSingleton<WebhookEventProcessor, PullRequestWebhookEventProcessor>();

        var app = builder.Build();
        app.MapGitHubWebhooks(secret: Environment.GetEnvironmentVariable("KTISIS_GITHUB_WEBHOOK_SECRET"));
        app.Run($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
    }
}