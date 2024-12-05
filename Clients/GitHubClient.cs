using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Ktisis.Models.GitHub;
using Microsoft.IdentityModel.Tokens;

namespace Ktisis.Clients;

internal class GitHubClient
{
    private readonly SigningCredentials _githubSigningCredentials;
    private readonly string _githubClientId;

    public GitHubClient(string githubPrivateKey, string githubClientId)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(githubPrivateKey);

        _githubSigningCredentials = new SigningCredentials(
            new RsaSecurityKey(rsa),
            SecurityAlgorithms.RsaSha256
        )
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false },
        };

        _githubClientId = githubClientId;
    }

    private string GenerateJwtSecurityToken()
    {
        var now = DateTime.UtcNow;
        var expires = now.AddSeconds(100);

        var jwt = new JwtSecurityToken(
            issuer: _githubClientId,
            claims:
            [
                new Claim(
                    "iat",
                    new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer
                ),
            ],
            expires: expires,
            signingCredentials: _githubSigningCredentials
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

    private InstallationAccessToken _githubInstallationAccessToken = new()
    {
        Token = "",
        ExpiresAt = DateTime.Now,
    };

    private async Task<string> GetGitHubInstallationAccessToken(long installationId)
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

    public async Task<RunnerRegistrationToken?> CreateRunnerRegistrationToken(
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
