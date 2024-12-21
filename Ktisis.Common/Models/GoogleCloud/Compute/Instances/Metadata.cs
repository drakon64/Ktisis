namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances;

public class Metadata
{
    public required MetadataItem[] Items { get; init; }
}

public class MetadataItem
{
    public required string Key { get; init; }
    public required string Value { get; init; }
}
