namespace Ktisis.Common.Models.GitHub;

internal class InstallationAccessToken
{
    public required string Token { get; init; }
    public required DateTime ExpiresAt { get; init; }
}