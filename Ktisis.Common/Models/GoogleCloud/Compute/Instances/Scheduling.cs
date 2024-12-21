namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances;

public class Scheduling
{
    public string InstanceTerminationAction = "DELETE";
    public MaxRunDuration MaxRunDuration = new();
}

public class MaxRunDuration
{
    public string Seconds = "21600";
}
