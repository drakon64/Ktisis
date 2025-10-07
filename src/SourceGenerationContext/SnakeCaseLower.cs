using System.Text.Json.Serialization;
using Ktisis.Client;
using Ktisis.Client.GitHub;

namespace Ktisis.SourceGenerationContext;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(GitHubClient.InstallationAccessToken))]
[JsonSerializable(typeof(GitHubClient.RunnerRegistrationToken))]
[JsonSerializable(typeof(GoogleCloudClient.AccessTokenResponse))]
internal sealed partial class SnakeCaseLowerSourceGenerationContext : JsonSerializerContext;
