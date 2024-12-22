using Ktisis.Common.Models.GoogleCloud.Compute.Instances;

namespace Ktisis.Common.Models.GoogleCloud.Tasks;

public class InstanceTask
{
    public required string Zone { get; init; }
    public required CreateInstance Instance { get; init; }
}
