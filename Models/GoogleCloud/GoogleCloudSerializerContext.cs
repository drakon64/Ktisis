using System.Text.Json.Serialization;
using Ktisis.Models.GoogleCloud.Compute.Instances;
using Ktisis.Models.GoogleCloud.Tasks;

namespace Ktisis.Models.GoogleCloud;

[JsonSerializable(typeof(CreateTask))]
[JsonSerializable(typeof(Instance))]
[JsonSourceGenerationOptions(
    IncludeFields = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
internal partial class GoogleCloudSerializerContext : JsonSerializerContext;
