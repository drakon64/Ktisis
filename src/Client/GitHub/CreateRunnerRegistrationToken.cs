using Ktisis.SourceGenerationContext;

namespace Ktisis.Client.GitHub;

internal static partial class GitHubClient
{
    internal static async Task<string> CreateRunnerRegistrationToken(
        string repo,
        long installationId
    )
    {
        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Headers =
                {
                    { "Authorization", await GetInstallationAccessToken(installationId) },
                    { "User-Agent", "Ktisis/0.0.1" },
                    { "Accept", "application/vnd.github+json" },
                    { "X-GitHub-Api-Version", "2022-11-28" },
                },
                Method = HttpMethod.Post,
                RequestUri = new Uri(
                    $"https://api.github.com/repos/{repo}/actions/runners/registration-token"
                ),
            }
        );

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());

        return (
            await response.Content.ReadFromJsonAsync<RunnerRegistrationToken>(
                SnakeCaseLowerSourceGenerationContext.Default.RunnerRegistrationToken
            )
        )!.Token;
    }

    internal sealed class RunnerRegistrationToken
    {
        public required string Token { get; init; }
    }
}
