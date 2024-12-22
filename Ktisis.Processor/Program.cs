using Ktisis.Common.Clients;
using Ktisis.Common.Models.GoogleCloud;
using Ktisis.Common.Models.GoogleCloud.Compute.Instances;
using Ktisis.Common.Models.GoogleCloud.Compute.ZoneOperations;
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
                var task = await GoogleClient.CreateInstance(
                    instanceTask.Instance,
                    instanceTask.Zone
                );

                if (task.Status != ZoneOperationStatus.Done)
                {
                    await Task.Delay(1000);

                    while (true)
                    {
                        if (
                            (await GoogleClient.GetZoneOperation(task.SelfLink)).Status
                            == ZoneOperationStatus.Done
                        )
                        {
                            break;
                        }

                        await Task.Delay(1000);
                    }
                }

                while (true)
                {
                    var instanceStatus = (await GoogleClient.GetInstance(task.TargetLink)).Status;

                    switch (instanceStatus)
                    {
                        case InstanceStatus.Running:
                            return Results.Ok();
                        case InstanceStatus.Terminated:
                            return Results.Problem();
                    }

                    await Task.Delay(1000);
                }
            }
        );

        app.MapGet("/", () => Results.Ok());

        app.Run();
    }
}
