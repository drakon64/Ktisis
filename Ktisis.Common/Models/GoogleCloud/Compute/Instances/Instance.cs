using Ktisis.Common.Models.GoogleCloud.Compute.Instances.Disks;
using Ktisis.Common.Models.GoogleCloud.Compute.Instances.NetworkInterfaces;

namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances;

public class Instance
{
    public required string Name { get; init; }
    public required string MachineType { get; init; }
    public required NetworkInterface[] NetworkInterfaces { get; init; }
    public required Disk[] Disks { get; init; }
    public required Metadata Metadata { get; init; }
    public readonly ServiceAccount[] ServiceAccounts = [new()];
    public readonly Scheduling Scheduling = new();
}
