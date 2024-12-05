namespace Ktisis.Models.GoogleCloud.Compute.Instances.Metadata;

internal class Metadata
{
    public MetadataItem[] Items =
    [
        new() { Key = "enable-oslogin", Value = "true" },
        new() { Key = "enable-oslogin-2fa", Value = "true" },
    ];
}
