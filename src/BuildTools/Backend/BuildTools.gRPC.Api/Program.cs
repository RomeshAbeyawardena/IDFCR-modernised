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
string? cachedReportData = null;

using SemaphoreSlim semaphoreSlim = new(0, 1);

const int UpdateIntervalInSeconds = 10;
app.MapGet("/", async(ctx) => {
    ctx.Response.Headers.Append("Refresh", $"{UpdateIntervalInSeconds}");
    StringBuilder statusReport = new();
    if (string.IsNullOrEmpty(cachedReportData))
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

        cachedReportData = statusReport.ToString();
    }
    else
    {
        await semaphoreSlim.WaitAsync();
        statusReport.Append(cachedReportData);
        semaphoreSlim.Release();
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
