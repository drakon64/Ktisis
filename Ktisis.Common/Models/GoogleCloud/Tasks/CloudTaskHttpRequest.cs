namespace Ktisis.Common.Models.GoogleCloud.Tasks;

public class CloudTaskHttpRequest
{
    public required string Url { get; init; }
    public const string HttpMethod = "POST";
    public readonly Dictionary<string, string> Headers = new()
    {
        { "Content-Type", "application/json" },
    };
    public required string Body { get; init; }
}
