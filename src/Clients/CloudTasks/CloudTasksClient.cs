using System.Text.Json.Serialization;

namespace Ktisis.Clients.CloudTasks;

internal static partial class CloudTasksClient
{
    private static readonly string Queue =
        Environment.GetEnvironmentVariable("KTISIS_CLOUD_TASKS_QUEUE")
        ?? throw new InvalidOperationException("KTISIS_CLOUD_TASKS_QUEUE is null");

    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        UseStringEnumConverter = true
    )]
    [JsonSerializable(typeof(TaskRequest))]
    private partial class CloudTasksClientSourceGenerationContext : JsonSerializerContext;
}
