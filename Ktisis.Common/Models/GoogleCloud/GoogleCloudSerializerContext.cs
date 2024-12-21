using System.Text.Json.Serialization;
using Ktisis.Common.Models.GoogleCloud.Compute.Instances;
using Ktisis.Common.Models.GoogleCloud.Tasks;

namespace Ktisis.Common.Models.GoogleCloud;

[JsonSerializable(typeof(CreateCloudTask))]
[JsonSerializable(typeof(Instance))]
[JsonSourceGenerationOptions(
    IncludeFields = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
public partial class GoogleCloudSerializerContext : JsonSerializerContext;
