using System.Text.Json.Serialization;

namespace Ktisis.Models.GoogleCloud.Tasks;

internal class CloudTaskHttpRequest(string body)
{
    public required string Url { get; init; }
    public const string HttpMethod = "POST";
    public readonly Dictionary<string, string> Headers = new() { { "Content-Type", "application/json" } };
    public readonly string Body = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(body));

    [JsonPropertyName("oauthToken")]
    public required OAuthToken OAuthToken { get; init; }
}
