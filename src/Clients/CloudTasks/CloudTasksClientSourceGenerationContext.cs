using System.Text.Json.Serialization;

namespace Ktisis.Clients.CloudTasks;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(CloudTasksClient.TaskRequest))]
internal partial class CloudTasksClientSourceGenerationContext : JsonSerializerContext;
