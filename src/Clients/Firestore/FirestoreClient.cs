using System.Text.Json.Serialization;

namespace Ktisis.Clients.Firestore;

internal static partial class FirestoreClient
{
    private static readonly string Url;

    static FirestoreClient()
    {
        var database =
            Environment.GetEnvironmentVariable("KTISIS_FIRESTORE_DATABASE")
            ?? throw new InvalidOperationException("KTISIS_FIRESTORE_DATABASE is null");

        Url = $"https://firestore.googleapis.com/v1/{database}";
    }

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(BeginTransactionRequest))]
    [JsonSerializable(typeof(BeginTransactionResponse))]
    [JsonSerializable(typeof(CommitTransactionRequest))]
    private partial class FirestoreClientSourceGenerationContext : JsonSerializerContext;
}
