namespace Ktisis.Models;

internal class AccessTokenResponse
{
    public required string AccessToken { get; init; }
    public required ushort ExpiresIn { get; init; }
    public required string TokenType { get; init; }
}
