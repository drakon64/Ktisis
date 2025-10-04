using System.Text.Json.Serialization;

namespace Ktisis.Clients.ComputeEngine;

internal static partial class ComputeEngineClient
{
    private static readonly string Project =
        Environment.GetEnvironmentVariable("KTISIS_PROJECT")
        ?? throw new InvalidOperationException("KTISIS_PROJECT is null");

    private static readonly string[] Zones =
        Environment.GetEnvironmentVariable("KTISIS_ZONES")?.Split(" ")
        ?? throw new InvalidOperationException("KTISIS_ZONES is null");

    private static readonly string SourceInstanceTemplate =
        Environment.GetEnvironmentVariable("KTISIS_SOURCE_INSTANCE_TEMPLATE")
        ?? throw new InvalidOperationException("KTISIS_SOURCE_INSTANCE_TEMPLATE is null");

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(CreateInstanceRequest))]
    [JsonSerializable(typeof(InstanceTemplate))]
    private sealed partial class ComputeEngineClientClientSourceGenerationContext
        : JsonSerializerContext;
}
