using System.Text.Json.Serialization;
using Ktisis.Models.GoogleCloud.Compute.Instances.Disks;
using Ktisis.Models.GoogleCloud.Compute.Instances.Metadata;
using Ktisis.Models.GoogleCloud.Compute.Instances.NetworkInterfaces;
using Ktisis.Models.GoogleCloud.Compute.Instances.ServiceAccounts;

namespace Ktisis.Models.GoogleCloud.Compute;

internal class Instance
{
    public required string Name { get; init; }
    public required string MachineType { get; init; }
    public NetworkInterface[] NetworkInterfaces = [new()];
    public required Disk[] Disks { get; init; }
    public required Metadata Metadata { get; init; }
    public ServiceAccount[] ServiceAccounts = [new()];
}

[JsonSerializable(typeof(Instance))]
[JsonSourceGenerationOptions(
    IncludeFields = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
internal partial class InstanceSerializerContext : JsonSerializerContext;
