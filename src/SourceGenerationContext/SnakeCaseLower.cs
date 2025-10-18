using System.Text.Json.Serialization;

using Ktisis.Client;
using Ktisis.Client.GitHub;

namespace Ktisis.SourceGenerationContext;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(GoogleCloudClient.AccessTokenResponse))]
[JsonSerializable(typeof(GitHubClient.RunnerRegistrationToken))]
[JsonSerializable(typeof(GitHubClient.InstallationAccessToken))]
internal sealed partial class SnakeCaseLowerSourceGenerationContext : JsonSerializerContext;
