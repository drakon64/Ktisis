using System.Collections.ObjectModel;

namespace Ktisis.Common.Models.GoogleCloud.Tasks;

public class CloudTaskHttpRequest
{
    public required string Url { get; init; }
    public readonly string HttpMethod = "POST";
    public readonly ReadOnlyDictionary<string, string> Headers = new Dictionary<string, string>
    {
        { "Content-Type", "application/json" },
    }.AsReadOnly();
    public required string Body { get; init; }
}
