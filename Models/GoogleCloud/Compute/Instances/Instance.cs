using System.Text.Json.Serialization;
using Ktisis.Models.GoogleCloud.Compute.Instances.Disks;
using Ktisis.Models.GoogleCloud.Compute.Instances.NetworkInterfaces;

namespace Ktisis.Models.GoogleCloud.Compute.Instances;

internal class Instance
{
    public required string Name { get; init; }
    public required string MachineType { get; init; }
    public required NetworkInterface[] NetworkInterfaces { get; init; }
    public required Disk[] Disks { get; init; }
    public required Metadata Metadata { get; init; }
    public ServiceAccount[] ServiceAccounts = [new()];
    public Scheduling Scheduling = new();
}

[JsonSerializable(typeof(Instance))]
[JsonSourceGenerationOptions(
    IncludeFields = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
internal partial class InstanceSerializerContext : JsonSerializerContext;
