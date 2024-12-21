namespace Ktisis.Receiver.Models.GoogleCloud.Compute.Instances.Disks;

internal class Disk
{
    public string? DeviceName { get; init; }
    public bool? Boot { get; init; }
    public required DiskInitializeParams InitializeParams { get; init; }
    public bool AutoDelete = true;
}
