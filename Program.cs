using Ktisis.Clients;
using Ktisis.EventProcessors;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace Ktisis;

public class Program
{
    internal static readonly string[]? RepositoryOwners = Environment
        .GetEnvironmentVariable("REPOSITORY_OWNERS")
        ?.Split(' ');

    internal static readonly string Project =
        Environment.GetEnvironmentVariable("PROJECT")
        ?? throw new InvalidOperationException("PROJECT is null");

    internal static readonly string[] Zones =
        Environment.GetEnvironmentVariable("ZONES")?.Split(' ')
        ?? throw new InvalidOperationException("ZONE is null");

    internal static readonly HttpClient HttpClient = new();

    internal static readonly GitHubClient GitHubClient = new(
        Environment.GetEnvironmentVariable("GITHUB_PRIVATE_KEY")
            ?? throw new InvalidOperationException("GITHUB_PRIVATE_KEY is null."),
        Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID")
            ?? throw new InvalidOperationException("GITHUB_CLIENT_ID is null.")
    );

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
