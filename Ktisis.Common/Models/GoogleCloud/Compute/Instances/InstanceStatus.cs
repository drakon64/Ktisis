using System.Text.Json.Serialization;

namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances;

public enum InstanceStatus
{
    [JsonPropertyName("PROVISIONING")]
    Provisioning,

    [JsonPropertyName("STAGING")]
    Staging,

    [JsonPropertyName("RUNNING")]
    Running,

    [JsonPropertyName("STOPPING")]
    Stopping,

    [JsonPropertyName("SUSPENDING")]
    Suspending,

    [JsonPropertyName("SUSPENDED")]
    Suspended,

    [JsonPropertyName("REPAIRING")]
    Repairing,

    [JsonPropertyName("TERMINATED")]
    Terminated,
}
