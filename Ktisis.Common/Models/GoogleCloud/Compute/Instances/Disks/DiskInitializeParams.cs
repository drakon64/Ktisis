namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances.Disks;

public class DiskInitializeParams
{
    public string? SourceImage { get; init; }
    public required string DiskSizeGb { get; init; }
    public required string DiskType { get; init; }
    public readonly string ProvisionedIops = "3000";
    public readonly string ProvisionedThroughput = "140";
}
