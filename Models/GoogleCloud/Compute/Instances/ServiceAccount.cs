namespace Ktisis.Models.GoogleCloud.Compute.Instances;

internal class ServiceAccount
{
    public string Email =
        Environment.GetEnvironmentVariable("COMPUTE_SERVICE_ACCOUNT")
        ?? throw new InvalidOperationException("COMPUTE_SERVICE_ACCOUNT is null.");
    public string[] Scopes =
    [
        "https://www.googleapis.com/auth/devstorage.read_only",
        "https://www.googleapis.com/auth/logging.write",
        "https://www.googleapis.com/auth/monitoring.write",
        "https://www.googleapis.com/auth/service.management.readonly",
        "https://www.googleapis.com/auth/servicecontrol",
        "https://www.googleapis.com/auth/trace.append",
    ];
}
