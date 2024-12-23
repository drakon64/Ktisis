namespace Ktisis.Common.Models.GoogleCloud.Tasks;

public class CloudTask
{
    public required string Name { get; init; }
    public required CloudTaskHttpRequest HttpRequest { get; init; }
}
