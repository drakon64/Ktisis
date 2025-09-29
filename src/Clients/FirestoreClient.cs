using System.Text.Json.Serialization;

namespace Ktisis.Clients;

internal static class FirestoreClient
{
    private static readonly string Url;

    static FirestoreClient()
    {
        var database =
            Environment.GetEnvironmentVariable("KTISIS_FIRESTORE_DATABASE")
            ?? throw new InvalidOperationException("KTISIS_FIRESTORE_DATABASE is null");

        Url = $"https://firestore.googleapis.com/v1/{database}";
    }

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
                    FirestoreClientSourceGenerationContext.Default.BeginTransactionRequest
                ),
            }
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(); // TODO: Useful exception
        }

        return (
            await response.Content.ReadFromJsonAsync<BeginTransactionResponse>(
                FirestoreClientSourceGenerationContext.Default.BeginTransactionResponse
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

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(FirestoreClient.BeginTransactionRequest))]
[JsonSerializable(typeof(FirestoreClient.BeginTransactionResponse))]
[JsonSerializable(typeof(FirestoreClient.CommitTransactionRequest))]
internal partial class FirestoreClientSourceGenerationContext : JsonSerializerContext;
