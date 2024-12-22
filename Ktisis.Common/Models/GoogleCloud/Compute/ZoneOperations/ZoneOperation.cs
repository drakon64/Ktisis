namespace Ktisis.Common.Models.GoogleCloud.Compute.ZoneOperations;

public class ZoneOperation
{
    public required string TargetLink { get; init; }
    public required ZoneOperationStatus Status { get; init; }
    public required string SelfLink { get; init; }
}
