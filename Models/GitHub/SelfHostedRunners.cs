namespace Ktisis.Models.GitHub;

internal class SelfHostedRunners
{
    public required Runner[] Runners { get; init; }
}

internal class Runner
{
    public required int Id { get; init; }
}
