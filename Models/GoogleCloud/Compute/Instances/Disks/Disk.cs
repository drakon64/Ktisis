namespace Ktisis.Models.GoogleCloud.Compute.Instances.Disks;

internal class Disk
{
    public required bool Boot { get; init; }
    public required DiskInitializeParams InitializeParams { get; init; }
}
