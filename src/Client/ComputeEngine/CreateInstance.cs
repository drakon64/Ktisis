using Ktisis.Client.GitHub;
using Ktisis.SourceGenerationContext;

namespace Ktisis.Client.ComputeEngine;

internal static partial class ComputeEngineClient
{
    private static readonly Random Random = new();

    internal static async Task CreateInstance(string name, string repository, long installationId)
    {
        var zone = Zones[Random.Next(0, Zones.Length)];
        var region = GetRegion(name);
        var zoneIndex = ZoneRegex().Match(zone).Value;

        var metadata = new List<MetadataItem>
        {
            new() { Key = "ktisis-repository", Value = repository },
            new()
            {
                Key = "ktisis-token",
                Value = await GitHubClient.CreateRunnerRegistrationToken(
                    repository,
                    installationId
                ),
            },
        };
        metadata.AddRange((await GetInstanceTemplate())!.Properties.Metadata.Items);

        using var requestContent = JsonContent.Create(
            new CreateInstanceRequest
            {
                Name = $"{zoneIndex}-{name}",
                Metadata = new Metadata { Items = metadata },
            },
            CamelCaseSourceGenerationContext.Default.CreateInstanceRequest
        );

        using var request = new HttpRequestMessage();
        request.Content = requestContent;
        request.Headers.Add("Authorization", await GoogleCloudClient.GetAccessToken());
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(
            $"https://compute.googleapis.com/compute/v1/projects/{Project}/zones/{region}-{zoneIndex}/instances?sourceInstanceTemplate={SourceInstanceTemplate}"
        );

        using var response = await Program.HttpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
    }

    internal sealed class CreateInstanceRequest
    {
        public required string Name { get; init; }
        public required Metadata Metadata { get; init; }
    }
}
