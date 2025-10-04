using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace Ktisis.Clients.GitHub;

internal static partial class GitHubClient
{
    private static readonly string ClientId =
        Environment.GetEnvironmentVariable("KTISIS_GITHUB_CLIENT_ID")
        ?? throw new InvalidOperationException("KTISIS_GITHUB_CLIENT_ID is null");

    private static readonly SigningCredentials GitHubSigningCredentials;

    static GitHubClient()
    {
        var githubPrivateKey =
            Environment.GetEnvironmentVariable("KTISIS_GITHUB_PRIVATE_KEY")
            ?? throw new InvalidOperationException("KTISIS_GITHUB_PRIVATE_KEY is null");

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

    private static InstallationAccessToken _githubInstallationAccessToken = new()
    {
        Token = "",
        ExpiresAt = DateTime.Now,
    };

    private static async Task RefreshGitHubInstallationAccessToken(long installationId)
    {
        // If the current installation access token expires in less than a minute, generate a new one
        if (_githubInstallationAccessToken.ExpiresAt.Subtract(DateTime.Now).Minutes >= 1)
            return;

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

        if (!response.IsSuccessStatusCode)
        {
            await Console.Out.WriteLineAsync(await response.Content.ReadAsStringAsync());

            throw new Exception(); // TODO: Useful exception
        }

        // TODO: Make this thread-safe
        _githubInstallationAccessToken = (
            await response.Content.ReadFromJsonAsync<InstallationAccessToken>(
                GitHubClientSourceGenerationContext.Default.InstallationAccessToken
            )
        )!;
    }

    private sealed class InstallationAccessToken
    {
        public required string Token { get; init; }
        public required DateTime ExpiresAt { get; init; }
    }

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    [JsonSerializable(typeof(InstallationAccessToken))]
    [JsonSerializable(typeof(RunnerRegistrationToken))]
    private partial class GitHubClientSourceGenerationContext : JsonSerializerContext;
}
