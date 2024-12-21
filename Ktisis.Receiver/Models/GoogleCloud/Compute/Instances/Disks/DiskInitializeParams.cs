namespace Ktisis.Receiver.Models.GoogleCloud.Compute.Instances.Disks;

internal class DiskInitializeParams(string zone)
{
    public string? SourceImage { get; init; }
    public required string DiskSizeGb { get; init; }
    public string DiskType =
        $"projects/{Program.Project}/zones/{zone}/diskTypes/hyperdisk-balanced";
    public string ProvisionedIops = "3000";
    public string ProvisionedThroughput = "140";
}
