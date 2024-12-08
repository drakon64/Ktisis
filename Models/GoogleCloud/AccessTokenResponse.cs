using System.Text.Json.Serialization;

namespace Ktisis.Models.GoogleCloud;

internal class AccessTokenResponse
{
    public required string AccessToken { get; init; }

    [JsonInclude]
    public required ushort ExpiresIn { private get; init; }

    public required string TokenType { get; init; }

    public readonly DateTime ExpiresAt;

    public AccessTokenResponse()
    {
        ExpiresAt = DateTime.Now + TimeSpan.FromSeconds(ExpiresIn);
    }
}

[JsonSerializable(typeof(AccessTokenResponse))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
internal partial class AccessTokenResponseSerializerContext : JsonSerializerContext;
