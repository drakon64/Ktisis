namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances;

public class Scheduling
{
    public const string InstanceTerminationAction = "DELETE";
    public readonly MaxRunDuration MaxRunDuration = new();
}

public class MaxRunDuration
{
    public const string Seconds = "21600";
}
