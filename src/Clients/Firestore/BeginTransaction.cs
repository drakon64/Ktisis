namespace Ktisis.Clients.Firestore;

internal static partial class FirestoreClient
{
    public static async Task<string> BeginTransaction(ITransactionOptions? transactionOptions)
    {
        var token = await GoogleCloudClient.RefreshAccessToken();

        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri($"{Url}/documents:beginTransaction"),
                Headers = { { "Authorization", $"{token.TokenType} {token.AccessToken}" } },
                Method = HttpMethod.Post,

                Content = JsonContent.Create(
                    new BeginTransactionRequest { Options = transactionOptions },
                    SourceGenerationContext.Default.BeginTransactionRequest
                ),
            }
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(); // TODO: Useful exception
        }

        return (
            await response.Content.ReadFromJsonAsync<BeginTransactionResponse>(
                SourceGenerationContext.Default.BeginTransactionResponse
            )
        )!.Transaction;
    }

    internal class BeginTransactionRequest
    {
        public ITransactionOptions? Options { get; init; }
    }

    internal interface ITransactionOptions;

    internal class ReadOnly : ITransactionOptions
    {
        public DateTime? ReadTime { get; init; }
    }

    internal class ReadWrite : ITransactionOptions
    {
        public string? RetryTransaction { get; init; }
    }

    internal class BeginTransactionResponse
    {
        public required string Transaction { get; init; }
    }
}
