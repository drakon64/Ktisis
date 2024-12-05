namespace Ktisis.Models.GoogleCloud.Compute.Instances.Disks;

internal class DiskInitializeParams
{
    public required string SourceImage { get; init; }
    public required string DiskSizeGb { get; init; }
    public required string DiskType { get; init; }
    public required string ProvisionedIops { get; init; }
    public required string ProvisionedThroughput { get; init; }
}
