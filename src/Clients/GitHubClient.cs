using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Ktisis.Clients;

internal static class GitHubClient
{
    private static readonly string GitHubClientId =
        Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID")
        ?? throw new InvalidOperationException("GITHUB_CLIENT_ID is null");

    private static readonly SigningCredentials GitHubSigningCredentials;

    static GitHubClient()
    {
        var githubPrivateKey =
            Environment.GetEnvironmentVariable("GITHUB_PRIVATE_KEY")
            ?? throw new InvalidOperationException("GITHUB_PRIVATE_KEY is null");

        var rsa = RSA.Create();
        rsa.ImportFromPem(githubPrivateKey);

        GitHubSigningCredentials = new SigningCredentials(
            new RsaSecurityKey(rsa),
            SecurityAlgorithms.RsaSha256
        )
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false },
        };
    }

    private static string GenerateJwtSecurityToken()
    {
        var now = DateTime.UtcNow;
        var expires = now.AddSeconds(100);

        var jwt = new JwtSecurityToken(
            issuer: GitHubClientId,
            claims:
            [
                new Claim(
                    "iat",
                    new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer
                ),
            ],
            expires: expires,
            signingCredentials: GitHubSigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private static InstallationAccessToken _githubInstallationAccessToken = new()
    {
        Token = "",
        ExpiresAt = DateTime.Now,
    };

    private static async Task GetGitHubInstallationAccessToken(ulong installationId)
    {
        // If the current installation access token expires in less than a minute, generate a new one
        if (_githubInstallationAccessToken.ExpiresAt.Subtract(DateTime.Now).Minutes >= 1)
            return;

        var responseMessage = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Authorization", $"Bearer {GenerateJwtSecurityToken()}" },
                    { "User-Agent", "Ktisis/0.0.1" },
                    { "Accept", "application/vnd.github+json" },
                    { "X-GitHub-Api-Version", "2022-11-28" },
                },
                RequestUri = new Uri(
                    $"https://api.github.com/app/installations/{installationId}/access_tokens"
                ),
            }
        );

        // TODO: Make this thread-safe
        _githubInstallationAccessToken = (
            await responseMessage.Content.ReadFromJsonAsync<InstallationAccessToken>()
        )!;
    }

    private class InstallationAccessToken
    {
        public required string Token { get; init; }
        public required DateTime ExpiresAt { get; init; }
    }
}
