using System.Text.Json.Serialization;
using Ktisis.Models.GoogleCloud.Compute.Instances.Disks;
using Ktisis.Models.GoogleCloud.Compute.Instances.NetworkInterfaces;

namespace Ktisis.Models.GoogleCloud.Compute;

internal class Instance
{
    public required string Name { get; init; }
    public required string MachineType { get; init; }
    public NetworkInterface[] NetworkInterfaces = [new()];
    public required Disk[] Disks { get; init; }
}

[JsonSerializable(typeof(Instance))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class InstanceSerializerContext : JsonSerializerContext;
