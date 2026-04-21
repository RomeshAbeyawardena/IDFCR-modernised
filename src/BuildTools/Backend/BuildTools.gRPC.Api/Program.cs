using BuildTools.Application.Extensions;
using BuildTools.GRPC.Api;
using BuildTools.GRPC.Application;
using BuildTools.Infrastructure.SqlServer.Extensions;
using IDFCR.Abstractions.GRPC;
using IDFCR.Abstractions.GRPC.HostExtensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services
    .AddGrpc();

services.AddSingleton(TimeProvider.System)
       .AddRepositories(builder.Configuration)
       .AddMediatorServices(builder.Configuration);

using var app = builder.Build();

app.DiscoverGRPCServices(builder.Configuration, typeof(ServiceMarker).Assembly);

IReadOnlyList<string> serviceList = [.. RegisteredGRPCServices.DiscoveredTypes.Select(x => x.Name)];

var startTime = app.Services.GetRequiredService<TimeProvider>().GetUtcNow();
using CachedStringService cachedStringService = new ();

const int UpdateIntervalInSeconds = 10;
app.MapGet("/", async(ctx) => {
    ctx.Response.Headers.Append("Refresh", $"{UpdateIntervalInSeconds}");
    StringBuilder statusReport = new();

    var value = await cachedStringService.GetCachedValueAsync();

    if (string.IsNullOrEmpty(value))
    {
        var newLine = Environment.NewLine;
        statusReport.AppendLine($"Build Tools gRPC Server{newLine}{newLine}");

#if DEBUG
        statusReport.AppendLine("Active Services:");

        foreach (var service in serviceList)
        {
            statusReport.AppendLine($"\t- {service}");
        }
#endif

        statusReport.AppendLine();

        await cachedStringService.SetCachedValueAsync(statusReport.ToString());
    }
    else
    {
        statusReport.Append(value);
    }

    var timeProvider = ctx.RequestServices.GetRequiredService<TimeProvider>();
    var timeNow = timeProvider.GetUtcNow();
    var uptime = timeNow.Subtract(startTime);
    statusReport.AppendLine($"Runtime start date: {startTime.ToLocalTime(): dd/MM/yyyy HH:mm:ss}");
    statusReport.AppendLine($"Server uptime: {uptime:hh\\:mm\\:ss}");
    statusReport.AppendLine($"Page last updated: {timeNow.ToLocalTime(): dd/MM/yyyy HH:mm:ss}");
    statusReport.AppendLine($"Next update at: {timeNow.AddSeconds(UpdateIntervalInSeconds).ToLocalTime(): dd/MM/yyyy HH:mm:ss}");
    await ctx.Response.WriteAsync(statusReport.ToString());
});

app.Run();
