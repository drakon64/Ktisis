using Ktisis.EventProcessors;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace Ktisis;

public class Program
{
    internal static readonly string[]? RepositoryOwners = Environment
        .GetEnvironmentVariable("REPOSITORY_OWNERS")
        ?.Split([' ']);

    internal static readonly string Project =
        Environment.GetEnvironmentVariable("PROJECT")
        ?? throw new InvalidOperationException("PROJECT is null");

    internal static readonly string Zone =
        Environment.GetEnvironmentVariable("ZONE")
        ?? throw new InvalidOperationException("ZONE is null");

    public static void Main()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Services.AddSingleton<WebhookEventProcessor, GitHubWebhookEventProcessor>();

        var app = builder.Build();

        app.MapGitHubWebhooks(secret: Environment.GetEnvironmentVariable("GITHUB_WEBHOOK_SECRET"));
        app.MapGet("/", () => Results.Ok());

        app.Run($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
    }
}
