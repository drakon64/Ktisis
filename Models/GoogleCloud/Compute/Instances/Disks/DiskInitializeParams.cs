namespace Ktisis.Models.GoogleCloud.Compute.Instances.Disks;

internal class DiskInitializeParams
{
    public required string SourceImage { get; init; }
    public required string DiskSizeGb { get; init; }
    public string DiskType =
        $"projects/{Program.Project}/zones/{Program.Zone}/diskTypes/hyperdisk-balanced";
    public string ProvisionedIops = "3000";
    public string ProvisionedThroughput = "140";
}
