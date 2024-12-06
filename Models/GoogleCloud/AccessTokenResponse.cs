using System.Text.Json.Serialization;

namespace Ktisis.Models.GoogleCloud;

internal class AccessTokenResponse
{
    public required string AccessToken { get; init; }
    public required ushort ExpiresIn { get; init; }
    public required string TokenType { get; init; }
}

[JsonSerializable(typeof(AccessTokenResponse))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
internal partial class AccessTokenResponseSerializerContext : JsonSerializerContext;
