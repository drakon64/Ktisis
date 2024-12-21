namespace Ktisis.Receiver.Models.GoogleCloud.Compute.Instances;

internal class Scheduling
{
    public string InstanceTerminationAction = "DELETE";
    public MaxRunDuration MaxRunDuration = new();
}

internal class MaxRunDuration
{
    public string Seconds = "21600";
}
