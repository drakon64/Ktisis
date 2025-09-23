namespace Ktisis.Clients;

internal static class CloudTasksClient
{
    public static async Task CreateTask()
    {
        await Program.HttpClient.PostAsJsonAsync(
            $"https://cloudtasks.googleapis.com/v2/{Program.Queue}/tasks",
            new CloudTask
            {
                Name = "test",
                HttpRequest = new HttpRequest
                {
                    Url = "",
                    Body = "",
                    OidcToken = new OidcToken { ServiceAccountEmail = "" },
                },
            }
        );
    }

    private class CloudTask
    {
        public required string Name { get; init; }

        public required HttpRequest HttpRequest { get; init; }
    }

    private class HttpRequest
    {
        public required string Url { get; init; }
        public required string Body { get; init; }
        public required OidcToken OidcToken { get; init; }
    }

    private class OidcToken
    {
        public required string ServiceAccountEmail { get; init; }
    }
}
