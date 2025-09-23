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
                HttpRequest = new HttpRequest { Body = "" },
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
        public readonly string Url = Program.Processor!;
        public required string Body { get; init; }
        public readonly OidcToken OidcToken = new();
    }

    private class OidcToken
    {
        public readonly string ServiceAccountEmail = Program.ServiceAccount!;
    }
}
