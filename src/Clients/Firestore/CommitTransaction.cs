namespace Ktisis.Clients.Firestore;

internal static partial class FirestoreClient
{
    public static async Task CommitTransaction(string transaction)
    {
        var token = await GoogleCloudClient.RefreshAccessToken();

        var response = await Program.HttpClient.SendAsync(
            new HttpRequestMessage
            {
                RequestUri = new Uri($"{Url}/documents:commit"),
                Headers = { { "Authorization", $"{token.TokenType} {token.AccessToken}" } },
                Method = HttpMethod.Post,

                Content = JsonContent.Create(
                    new CommitTransactionRequest { Transaction = transaction },
                    FirestoreClientSourceGenerationContext.Default.CommitTransactionRequest
                ),
            }
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(); // TODO: Useful exception
        }
    }

    internal class CommitTransactionRequest
    {
        public required string Transaction { get; init; }
    }
}
