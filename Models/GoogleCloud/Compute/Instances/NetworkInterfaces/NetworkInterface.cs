namespace Ktisis.Models.GoogleCloud.Compute.Instances.NetworkInterfaces;

internal class NetworkInterface
{
    public string? Network { get; set; }
    public string? Subnetwork { get; set; }
    public NetworkInterfaceAccessConfig[] AccessConfigs = [new()];
    public string NicType = "GVNIC";
}
