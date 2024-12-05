using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Ktisis.Models.GitHub;
using Microsoft.IdentityModel.Tokens;

namespace Ktisis.Clients;

internal static class GitHubClient
{
    private static readonly SigningCredentials GitHubSigningCredentials;
    private static readonly string GitHubClientId;

    static GitHubClient()
    {
        GitHubClientId =
            Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID")
            ?? throw new InvalidOperationException("GITHUB_CLIENT_ID is null.");

        var rsa = RSA.Create();
        rsa.ImportFromPem(
            Environment.GetEnvironmentVariable("GITHUB_PRIVATE_KEY")
                ?? throw new InvalidOperationException("GITHUB_PRIVATE_KEY is null.")
        );

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

    private static readonly HttpClient HttpClient = new()
    {
        DefaultRequestHeaders =
        {
            { "User-Agent", "Ktisis/0.0.1" },
            { "Accept", "application/vnd.github+json" },
            { "X-GitHub-Api-Version", "2022-11-28" },
        },
    };

    private const string GitHubApiUri = "https://api.github.com/";

    private static InstallationAccessToken _githubInstallationAccessToken = new()
    {
        Token = "",
        ExpiresAt = DateTime.Now,
    };

    private static async Task<string> GetGitHubInstallationAccessToken(long installationId)
    {
        // If the current installation access token expires in less than a minute, generate a new one
        if (_githubInstallationAccessToken.ExpiresAt.Subtract(DateTime.Now).Minutes >= 1)
            return _githubInstallationAccessToken.Token;

        await Console.Out.WriteLineAsync("Generating new GitHub installation access token");

        var responseMessage = await HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Headers = { { "Authorization", $"Bearer {GenerateJwtSecurityToken()}" } },
                RequestUri = new Uri(
                    $"{GitHubApiUri}app/installations/{installationId}/access_tokens"
                ),
            }
        );
        
        Console.Out.WriteLine(await responseMessage.Content.ReadAsStringAsync());

        _githubInstallationAccessToken = (
            await responseMessage.Content.ReadFromJsonAsync<InstallationAccessToken>(
                GitHubSerializerContext.Default.InstallationAccessToken
            )
        )!;

        return _githubInstallationAccessToken.Token;
    }

    public static async Task<RunnerRegistrationToken?> CreateRunnerRegistrationToken(
        string repo,
        long installationId
    )
    {
        var request = await HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Headers =
                {
                    {
                        "Authorization",
                        $"Bearer {await GetGitHubInstallationAccessToken(installationId)}"
                    },
                },
                RequestUri = new Uri(
                    $"{GitHubApiUri}repos/{repo}/actions/runners/registration-token"
                ),
            }
        );

        return await request.Content.ReadFromJsonAsync<RunnerRegistrationToken>(
            GitHubSerializerContext.Default.RunnerRegistrationToken
        );
    }
}
