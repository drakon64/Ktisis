namespace Ktisis.Clients.GitHub;

internal static partial class GitHubClient
{
    public static async Task<string> CreateRunnerRegistrationToken(string repo, long installationId)
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

    private class RunnerRegistrationToken
    {
        public required string Token { get; init; }
    }
}
