using System.Text.Json.Serialization;
using Ktisis.Client;
using Ktisis.Client.CloudTasks;
using Ktisis.Client.ComputeEngine;

namespace Ktisis.SourceGenerationContext;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true
)]
[JsonSerializable(typeof(GoogleCloudClient.AccessTokenResponse))]
[JsonSerializable(typeof(CloudTasksClient.TaskRequest))]
[JsonSerializable(typeof(ComputeEngineClient.CreateInstanceRequest))]
[JsonSerializable(typeof(ComputeEngineClient.InstanceTemplate))]
internal sealed partial class CamelCaseSourceGenerationContext : JsonSerializerContext;
