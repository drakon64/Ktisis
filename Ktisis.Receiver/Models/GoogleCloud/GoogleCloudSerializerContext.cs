using System.Text.Json.Serialization;
using Ktisis.Receiver.Models.GoogleCloud.Compute.Instances;
using Ktisis.Receiver.Models.GoogleCloud.Tasks;

namespace Ktisis.Receiver.Models.GoogleCloud;

[JsonSerializable(typeof(CreateCloudTask))]
[JsonSerializable(typeof(Instance))]
[JsonSourceGenerationOptions(
    IncludeFields = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
internal partial class GoogleCloudSerializerContext : JsonSerializerContext;
