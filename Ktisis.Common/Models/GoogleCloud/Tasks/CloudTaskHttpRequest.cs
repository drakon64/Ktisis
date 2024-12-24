using System.Collections.ObjectModel;

namespace Ktisis.Common.Models.GoogleCloud.Tasks;

public class CloudTaskHttpRequest
{
    public required string Url { get; init; }

    // `const` doesn't work well with (de)serialization
    // ReSharper disable once ConvertToConstant.Global
    public readonly string HttpMethod = "POST";

    public readonly ReadOnlyDictionary<string, string> Headers = new Dictionary<string, string>
    {
        { "Content-Type", "application/json" },
    }.AsReadOnly();
    public required string Body { get; init; }
}
