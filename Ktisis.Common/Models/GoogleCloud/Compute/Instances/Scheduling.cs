namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances;

public class Scheduling
{
    public readonly string InstanceTerminationAction = "DELETE";
    public readonly MaxRunDuration MaxRunDuration = new();
}

public class MaxRunDuration
{
    public readonly string Seconds = "21600";
}
