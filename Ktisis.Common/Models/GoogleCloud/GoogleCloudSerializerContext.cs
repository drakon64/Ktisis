using System.Text.Json.Serialization;
using Ktisis.Common.Models.GoogleCloud.Compute.Instances;
using Ktisis.Common.Models.GoogleCloud.Compute.ZoneOperations;
using Ktisis.Common.Models.GoogleCloud.Tasks;

namespace Ktisis.Common.Models.GoogleCloud;

[JsonSerializable(typeof(CreateCloudTask))]
[JsonSerializable(typeof(GetInstance))]
[JsonSerializable(typeof(InstanceTask))]
[JsonSerializable(typeof(ZoneOperation))]
[JsonSourceGenerationOptions(
    IncludeFields = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true
)]
public partial class GoogleCloudSerializerContext : JsonSerializerContext;
