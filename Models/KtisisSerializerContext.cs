using System.Text.Json.Serialization;

namespace Ktisis.Models;

[JsonSerializable(typeof(AccessTokenResponse))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
internal partial class KtisisSerializerContext : JsonSerializerContext;
