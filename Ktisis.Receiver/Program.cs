using Ktisis.Receiver.EventProcessors;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace Ktisis.Receiver;

public class Program
{
    internal static readonly string? Network = Environment.GetEnvironmentVariable("NETWORK");

    internal static readonly string? Subnetwork = Environment.GetEnvironmentVariable("SUBNETWORK");

    internal static readonly string ServiceAccountEmail =
        Environment.GetEnvironmentVariable("COMPUTE_SERVICE_ACCOUNT")
        ?? throw new InvalidOperationException("COMPUTE_SERVICE_ACCOUNT is null.");

    internal static readonly string TaskServiceUrl =
        Environment.GetEnvironmentVariable("TASK_SERVICE_URL")
        ?? throw new InvalidOperationException("TASK_SERVICE_URL is null.");

    internal static readonly string[] Zones =
        Environment.GetEnvironmentVariable("ZONES")?.Split(' ')
        ?? throw new InvalidOperationException("ZONE is null");

    internal static readonly string[]? RepositoryOwners = Environment
        .GetEnvironmentVariable("REPOSITORY_OWNERS")
        ?.Split(' ');

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
