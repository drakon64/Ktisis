using System.Text.Json.Serialization;

namespace Ktisis.Common.Models.GoogleCloud.Compute.ZoneOperations;

public enum ZoneOperationStatus
{
    [JsonPropertyName("PENDING")]
    Pending,

    [JsonPropertyName("RUNNING")]
    Running,

    [JsonPropertyName("DONE")]
    Done,
}
