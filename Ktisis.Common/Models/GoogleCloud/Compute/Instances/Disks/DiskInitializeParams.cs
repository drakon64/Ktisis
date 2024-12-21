using Ktisis.Common.Clients;

namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances.Disks;

public class DiskInitializeParams(string zone)
{
    public string? SourceImage { get; init; }
    public required string DiskSizeGb { get; init; }
    public readonly string DiskType =
        $"projects/{GoogleClient.Project}/zones/{zone}/diskTypes/hyperdisk-balanced";
    public const string ProvisionedIops = "3000";
    public const string ProvisionedThroughput = "140";
}
