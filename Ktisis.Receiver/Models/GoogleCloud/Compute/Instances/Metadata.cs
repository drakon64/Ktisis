namespace Ktisis.Receiver.Models.GoogleCloud.Compute.Instances;

internal class Metadata
{
    public required MetadataItem[] Items { get; init; }
}

internal class MetadataItem
{
    public required string Key { get; init; }
    public required string Value { get; init; }
}
