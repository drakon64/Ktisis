using System.Text.Json.Serialization;

namespace Ktisis.Models.GitHub;

[JsonSerializable(typeof(InstallationAccessToken))]
[JsonSerializable(typeof(RunnerRegistrationToken))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
internal partial class GitHubSerializerContext : JsonSerializerContext;
