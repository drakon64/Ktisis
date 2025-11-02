using Ktisis.SourceGenerationContext;

namespace Ktisis.Client.GitHub;

internal static partial class GitHubClient
{
    internal static async Task<string> CreateRunnerRegistrationToken(
        string repo,
        long installationId
    )
    {
        using var request = new HttpRequestMessage();
        request.Headers.Add("Authorization", await GetInstallationAccessToken(installationId));
        request.Headers.Add("User-Agent", "Ktisis/0.0.1");
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(
            $"https://api.github.com/repos/{repo}/actions/runners/registration-token"
        );

        using var response = await Program.HttpClient.SendAsync(request);

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
