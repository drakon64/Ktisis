using System.Text.RegularExpressions;

namespace Ktisis.Client.ComputeEngine;

internal static partial class ComputeEngineClient
{
    private static readonly string Project =
        Environment.GetEnvironmentVariable("KTISIS_PROJECT")
        ?? throw new InvalidOperationException("KTISIS_PROJECT is null");

    private static readonly string[] Zones =
        Environment.GetEnvironmentVariable("KTISIS_ZONES")?.Split(" ")
        ?? throw new InvalidOperationException("KTISIS_ZONES is null");

    private static readonly string SourceInstanceTemplate =
        Environment.GetEnvironmentVariable("KTISIS_SOURCE_INSTANCE_TEMPLATE")
        ?? throw new InvalidOperationException("KTISIS_SOURCE_INSTANCE_TEMPLATE is null");

    private static string GetRegion(string zone) => RegionRegex().Match(zone).Groups[1].Value;

    [GeneratedRegex("(.*)-.")]
    private static partial Regex RegionRegex();

    [GeneratedRegex(".$")]
    private static partial Regex ZoneRegex();

    internal sealed class Metadata
    {
        public required List<MetadataItem> Items { get; init; }
    }

    internal sealed class MetadataItem
    {
        public required string Key { get; init; }
        public required string Value { get; init; }
    }
}
