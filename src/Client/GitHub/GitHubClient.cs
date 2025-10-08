using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Ktisis.SourceGenerationContext;
using Microsoft.IdentityModel.Tokens;

namespace Ktisis.Client.GitHub;

internal static partial class GitHubClient
{
    private static readonly string ClientId =
        Environment.GetEnvironmentVariable("KTISIS_GITHUB_CLIENT_ID")
        ?? throw new InvalidOperationException("KTISIS_GITHUB_CLIENT_ID is null");

    private static readonly SigningCredentials GitHubSigningCredentials = GetSigningCredentials();

    private static SigningCredentials GetSigningCredentials()
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(
            Environment.GetEnvironmentVariable("KTISIS_GITHUB_PRIVATE_KEY")
                ?? throw new InvalidOperationException("KTISIS_GITHUB_PRIVATE_KEY is null")
        );

        return new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false },
        };
    }

    private static string GenerateJwt()
    {
        var now = DateTime.UtcNow;
        var expires = now.AddSeconds(60);

        var jwt = new JwtSecurityToken(
            issuer: ClientId,
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

    private static InstallationAccessToken _installationAccessToken = new()
    {
        Token = "",
        ExpiresAt = DateTime.Now,
    };

    private static readonly object Lock = new();

    private static async Task<string> GetInstallationAccessToken(long installationId)
    {
        Monitor.Enter(Lock);

        try
        {
            // If the current installation access token expires in less than a minute, generate a new one
            if (_installationAccessToken.ExpiresAt.Subtract(DateTime.Now).Minutes < 1)
                _installationAccessToken = await RefreshInstallationAccessToken(installationId);
        }
        finally
        {
            Monitor.Exit(Lock);
        }

        return _installationAccessToken.Token;
    }

    private static async Task<InstallationAccessToken> RefreshInstallationAccessToken(
        long installationId
    )
    {
        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Authorization", $"Bearer {GenerateJwt()}" },
                    { "User-Agent", "Ktisis/0.0.1" },
                    { "Accept", "application/vnd.github+json" },
                    { "X-GitHub-Api-Version", "2022-11-28" },
                },
                RequestUri = new Uri(
                    $"https://api.github.com/app/installations/{installationId}/access_tokens"
                ),
            }
        );

        if (response.IsSuccessStatusCode)
            return (
                await response.Content.ReadFromJsonAsync<InstallationAccessToken>(
                    SnakeCaseLowerSourceGenerationContext.Default.InstallationAccessToken
                )
            )!;

        throw new Exception(await response.Content.ReadAsStringAsync()); // TODO: Useful exception
    }

    internal sealed class InstallationAccessToken
    {
        public required string Token { get; init; }
        public required DateTime ExpiresAt { get; init; }
    }
}
