using Ktisis.Common.Clients;

namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances.Disks;

public class DiskInitializeParams(string zone)
{
    public string? SourceImage { get; init; }
    public required string DiskSizeGb { get; init; }
    public string DiskType =
        $"projects/{GoogleClient.Project}/zones/{zone}/diskTypes/hyperdisk-balanced";
    public string ProvisionedIops = "3000";
    public string ProvisionedThroughput = "140";
}
