namespace Ktisis.Common.Models.GoogleCloud.Compute.Instances;

public class ServiceAccount
{
    public required string Email { get; init; }
    public readonly string[] Scopes =
    [
        "https://www.googleapis.com/auth/devstorage.read_only",
        "https://www.googleapis.com/auth/logging.write",
        "https://www.googleapis.com/auth/monitoring.write",
        "https://www.googleapis.com/auth/service.management.readonly",
        "https://www.googleapis.com/auth/servicecontrol",
        "https://www.googleapis.com/auth/trace.append",
    ];
}
