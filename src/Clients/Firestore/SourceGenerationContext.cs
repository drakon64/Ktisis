using System.Text.Json.Serialization;

namespace Ktisis.Clients.Firestore;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(FirestoreClient.BeginTransactionRequest))]
[JsonSerializable(typeof(FirestoreClient.BeginTransactionResponse))]
[JsonSerializable(typeof(FirestoreClient.CommitTransactionRequest))]
internal partial class SourceGenerationContext : JsonSerializerContext;
