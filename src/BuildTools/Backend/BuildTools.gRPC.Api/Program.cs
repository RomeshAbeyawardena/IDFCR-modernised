using BuildTools.Application.Extensions;
using BuildTools.GRPC.Application.Extensions;
using BuildTools.Infrastructure.SqlServer.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services
    .AddGrpc();

services.AddSingleton(TimeProvider.System)
       .AddRepositories(builder.Configuration)
       .AddMediatorServices(builder.Configuration);

var app = builder.Build();



var method = typeof(GrpcEndpointRouteBuilderExtensions)
        .GetMethods()
        .FirstOrDefault(x => x.IsGenericMethod && x.Name.StartsWith(nameof(GrpcEndpointRouteBuilderExtensions.MapGrpcService)));

List<string> serviceList = [];

foreach (var type in GrpcServices.Fetch())
{
    var genericMethod = method?.MakeGenericMethod(type);
    genericMethod?.Invoke(null, [app]);
    serviceList.Add(type.Name);
}

var startTime = app.Services.GetRequiredService<TimeProvider>().GetUtcNow();

app.MapGet("/", async(ctx) => {
    ctx.Response.Headers.Append("Refresh", "10");
    var newLine = Environment.NewLine;
    var statusReport = new StringBuilder($"Build Tools gRPC Server{newLine}{newLine}Active Services:");

    statusReport.AppendLine();
    foreach (var service in serviceList)
    {
        statusReport.AppendLine($"\t- {service}");
    }

    statusReport.AppendLine();

    var timeProvider = ctx.RequestServices.GetRequiredService<TimeProvider>();

    var uptime = timeProvider.GetUtcNow().Subtract(startTime);

    statusReport.AppendLine($"Server Uptime: {uptime:hh\\:mm\\:ss}");

    await ctx.Response.WriteAsync(statusReport.ToString());

    ///return $"Build tools GRPC Server\r\nActive Services:\r\n\t- {string.Join("\r\n\t- ", serviceList)}\r\n";
});

app.Run();
