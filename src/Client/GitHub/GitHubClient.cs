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

    private static readonly SigningCredentials SigningCredentials = GetSigningCredentials();

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
            signingCredentials: SigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private static async Task<string> GetInstallationAccessToken(long installationId)
    {
        using var request = new HttpRequestMessage();
        request.Headers.Add("Authorization", $"Bearer {GenerateJwt()}");
        request.Headers.Add("User-Agent", "Ktisis/0.0.1");
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(
            $"https://api.github.com/app/installations/{installationId}/access_tokens"
        );

        using var response = await Program.HttpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());

        var token = await response.Content.ReadFromJsonAsync<InstallationAccessToken>(
            SnakeCaseLowerSourceGenerationContext.Default.InstallationAccessToken
        );

        return $"Bearer {token!.Token}";
    }

    internal sealed class InstallationAccessToken
    {
        public required string Token { get; init; }
    }
}
