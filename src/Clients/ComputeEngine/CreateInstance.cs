using Ktisis.Clients.GitHub;

namespace Ktisis.Clients.ComputeEngine;

internal static partial class ComputeEngineClient
{
    private static readonly string SourceInstanceTemplate =
        Environment.GetEnvironmentVariable("KTISIS_SOURCE_INSTANCE_TEMPLATE")
        ?? throw new InvalidOperationException("KTISIS_SOURCE_INSTANCE_TEMPLATE is null");

    public static async Task CreateInstance(string name, string repository, long installationId)
    {
        var token = await GoogleCloudClient.RefreshAccessToken();

        var zone = Zones[0]; // TODO: Pick a random element

        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/v1/projects/{Project}/zones/{zone}/instances?sourceInstanceTemplate={SourceInstanceTemplate}"
                ),
                Headers = { { "Authorization", $"{token.TokenType} {token.AccessToken}" } },
                Method = HttpMethod.Post,

                Content = JsonContent.Create(
                    new CreateInstanceRequest
                    {
                        Name = $"i-{name}",
                        Metadata = new Metadata
                        {
                            Items =
                            [
                                new MetadataItem { Key = "REPOSITORY", Value = repository },
                                new MetadataItem
                                {
                                    Key = "TOKEN",
                                    Value = await GitHubClient.CreateRunnerRegistrationToken(
                                        repository,
                                        installationId
                                    ),
                                },
                            ],
                        },
                    },
                    ComputeEngineClientClientSourceGenerationContext.Default.CreateInstanceRequest
                ),
            }
        );

        if (!response.IsSuccessStatusCode)
        {
            await Console.Out.WriteLineAsync(await response.Content.ReadAsStringAsync());

            throw new Exception(); // TODO: Useful exception
        }
    }

    private class CreateInstanceRequest
    {
        public required string Name { get; init; }
        public required Metadata Metadata { get; init; }
    }

    private class Metadata
    {
        public required MetadataItem[] Items { get; init; }
    }

    private class MetadataItem
    {
        public required string Key { get; init; }
        public required string Value { get; init; }
    }
}
