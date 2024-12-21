using Ktisis.Receiver.EventProcessors;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace Ktisis.Receiver;

public class Program
{
    internal static readonly string[]? RepositoryOwners = Environment
        .GetEnvironmentVariable("REPOSITORY_OWNERS")
        ?.Split(' ');

    internal static readonly string[] Zones =
        Environment.GetEnvironmentVariable("ZONES")?.Split(' ')
        ?? throw new InvalidOperationException("ZONE is null");

    public static void Main()
    {
        if (RepositoryOwners is not null)
            Console.Out.WriteLine(
                $"Allowlisted repository owners: {string.Join(' ', RepositoryOwners)}"
            );

        var builder = WebApplication.CreateSlimBuilder();
        builder.Services.AddSingleton<WebhookEventProcessor, GitHubWebhookEventProcessor>();

        var app = builder.Build();

        app.MapGitHubWebhooks(secret: Environment.GetEnvironmentVariable("GITHUB_WEBHOOK_SECRET"));
        app.MapGet("/", () => Results.Ok());

        app.Run($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
    }
}
