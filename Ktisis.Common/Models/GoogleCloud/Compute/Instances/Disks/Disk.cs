namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances.Disks;

public class Disk
{
    public string? DeviceName { get; init; }
    public bool? Boot { get; init; }
    public required DiskInitializeParams InitializeParams { get; init; }
    public readonly bool AutoDelete = true;
}
