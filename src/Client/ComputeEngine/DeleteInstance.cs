namespace Ktisis.Client.ComputeEngine;

internal static partial class ComputeEngineClient
{
    private static readonly bool SkipGracefulShutdown = bool.Parse(
        Environment.GetEnvironmentVariable("KTISIS_SKIP_GRACEFUL_SHUTDOWN")!
    );

    private static readonly string ApiVersion = SkipGracefulShutdown ? "beta" : "v1";

    internal static async Task DeleteInstance(string name)
    {
        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                Headers = { { "Authorization", await GoogleCloudClient.GetAccessToken() } },
                Method = HttpMethod.Delete,
                RequestUri = new Uri(
                    $"https://compute.googleapis.com/compute/{ApiVersion}/projects/{Project}/zones/{Zone}/instances/i-{name}"
                        + (SkipGracefulShutdown ? "?noGracefulShutdown=true" : null)
                ),
            }
        );

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
    }
}
