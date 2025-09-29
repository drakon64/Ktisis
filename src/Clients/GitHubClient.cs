using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
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

    private static string GenerateJwt()
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

    private static async Task RefreshGitHubInstallationAccessToken(ulong installationId)
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

        // TODO: Make this thread-safe
        _githubInstallationAccessToken = (
            await responseMessage.Content.ReadFromJsonAsync<InstallationAccessToken>(
                GitHubClientSourceGenerationContext.Default.InstallationAccessToken
            )
        )!;
    }

    public static async Task<string> CreateRunnerRegistrationToken(
        string repo,
        ulong installationId
    )
    {
        await RefreshGitHubInstallationAccessToken(installationId);

        var request = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Authorization", $"Bearer {_githubInstallationAccessToken.Token}" },
                    { "User-Agent", "Ktisis/0.0.1" },
                    { "Accept", "application/vnd.github+json" },
                    { "X-GitHub-Api-Version", "2022-11-28" },
                },
                RequestUri = new Uri(
                    $"https://api.github.com/repos/{repo}/actions/runners/registration-token"
                ),
            }
        );

        return (
            await request.Content.ReadFromJsonAsync<RunnerRegistrationToken>(
                GitHubClientSourceGenerationContext.Default.RunnerRegistrationToken
            )
        )!.Token;
    }

    internal class InstallationAccessToken
    {
        public required string Token { get; init; }
        public required DateTime ExpiresAt { get; init; }
    }

    internal class RunnerRegistrationToken
    {
        public required string Token { get; init; }
    }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(GitHubClient.InstallationAccessToken))]
[JsonSerializable(typeof(GitHubClient.RunnerRegistrationToken))]
internal partial class GitHubClientSourceGenerationContext : JsonSerializerContext;
