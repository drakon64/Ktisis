using System.Text.Json.Serialization;

namespace Ktisis.Models.GoogleCloud.Tasks;

internal class HttpRequest(string body)
{
    public required string Url { get; init; }
    public string HttpMethod = "POST";
    public Dictionary<string, string> Headers = new() { { "Content-Type", "application/json" } };
    public string Body = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(body));

    [JsonPropertyName("oauthToken")]
    public required OAuthToken OAuthToken { get; init; }
}
