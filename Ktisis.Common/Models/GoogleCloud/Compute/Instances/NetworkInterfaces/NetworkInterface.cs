namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances.NetworkInterfaces;

public class NetworkInterface
{
    public string? Network { get; init; }
    public string? Subnetwork { get; init; }
    public readonly NetworkInterfaceAccessConfig[] AccessConfigs = [new()];
    public const string NicType = "GVNIC";
}
