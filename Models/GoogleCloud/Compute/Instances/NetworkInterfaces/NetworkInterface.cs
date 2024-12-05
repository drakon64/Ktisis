namespace Ktisis.Models.GoogleCloud.Compute.Instances.NetworkInterfaces;

internal class NetworkInterface
{
    public NetworkInterfaceAccessConfig[] AccessConfigs = [new()];
    public string NicType = "GVNIC";
}
