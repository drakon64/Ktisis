using Ktisis.Common.Clients;
using Ktisis.Common.Models.GoogleCloud;
using Ktisis.Common.Models.GoogleCloud.Tasks;

namespace Ktisis.Processor;

public class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateSlimBuilder();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(
                0,
                GoogleCloudSerializerContext.Default
            );
        });

        var app = builder.Build();

        app.MapPost(
            "/",
            async (InstanceTask instanceTask) =>
            {
                await GoogleClient.CreateInstance(instanceTask.Instance, instanceTask.Zone);
            }
        );

        app.MapGet("/", () => Results.Ok());

        app.Run();
    }
}
