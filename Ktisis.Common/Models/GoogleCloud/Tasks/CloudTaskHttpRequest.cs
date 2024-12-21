namespace Ktisis.Common.Models.GoogleCloud.Tasks;

public class CloudTaskHttpRequest(string body)
{
    public required string Url { get; init; }
    public const string HttpMethod = "POST";
    public readonly Dictionary<string, string> Headers = new()
    {
        { "Content-Type", "application/json" },
    };
    public readonly string Body = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(body));
}
