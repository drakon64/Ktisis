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
}
