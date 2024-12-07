using System.Text.Json.Serialization;

namespace Ktisis.Models.GitHub;

[JsonSerializable(typeof(InstallationAccessToken))]
[JsonSerializable(typeof(RunnerRegistrationToken))]
[JsonSerializable(typeof(SelfHostedRunners))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
internal partial class GitHubSerializerContext : JsonSerializerContext;
