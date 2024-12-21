namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances.NetworkInterfaces;

public class NetworkInterface
{
    public string? Network { get; set; }
    public string? Subnetwork { get; set; }
    public readonly NetworkInterfaceAccessConfig[] AccessConfigs = [new()];
    public const string NicType = "GVNIC";
}
