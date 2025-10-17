using System.CommandLine;
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

        var receiver = new Command("receiver");
        receiver.SetAction(_ =>
        {
            builder.Services.AddSingleton<
                WebhookEventProcessor,
                WorkflowJobWebhookEventProcessor
            >();

            app = builder.Build();

            app.MapGitHubWebhooks(
                secret: Environment.GetEnvironmentVariable("KTISIS_GITHUB_WEBHOOK_SECRET")
            );

            Run(app);
        });

        var processor = new Command("processor");
        processor.SetAction(_ =>
        {
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
        });

        var rootCommand = new RootCommand();
        rootCommand.Subcommands.Add(receiver);
        rootCommand.Subcommands.Add(processor);
        rootCommand.Parse(args).Invoke();
    }

    private static void Run(WebApplication app) =>
        app.Run($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
}
