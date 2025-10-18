using Ktisis.Client.ComputeEngine;
using Ktisis.Webhook;

using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace Ktisis;

internal static class Program
{
    internal static readonly string[]? Repositories = Environment
        .GetEnvironmentVariable("KTISIS_GITHUB_REPOSITORIES")
        ?.Split(" ");

    internal static readonly HttpClient HttpClient = new();

    internal static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder();
        WebApplication app;

        switch (args[1])
        {
            case "receiver":
                builder.Services.AddSingleton<
                    WebhookEventProcessor,
                    WorkflowJobWebhookEventProcessor
                >();

                app = builder.Build();

                app.MapGitHubWebhooks(
                    secret: Environment.GetEnvironmentVariable("KTISIS_GITHUB_WEBHOOK_SECRET")
                );

                Run(app);

                break;
            case "processor":
                app = builder.Build();

                app.MapPost(
                    "/api/ktisis",
                    async (string instanceName, string repository, long installationId) =>
                        await ComputeEngineClient.CreateInstance(
                            instanceName,
                            repository,
                            installationId
                        )
                );

                app.MapDelete(
                    "/api/ktisis",
                    async (string instanceName) =>
                        await ComputeEngineClient.DeleteInstance(instanceName)
                );

                Run(app);

                break;
        }
    }

    private static void Run(WebApplication app) =>
        app.Run($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
}
