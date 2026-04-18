using BuildTools.Application.Extensions;
using BuildTools.GRPC.Application.Extensions;
using BuildTools.Infrastructure.SqlServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services
    .AddGrpc();

services.AddSingleton(TimeProvider.System)
       .AddRepositories(builder.Configuration)
       .AddMediatorServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

foreach (var type in GrpcServices.Fetch())
{
    var method = typeof(GrpcEndpointRouteBuilderExtensions)
        .GetMethods()
        .FirstOrDefault(x => x.IsGenericMethod && x.Name.StartsWith(nameof(GrpcEndpointRouteBuilderExtensions.MapGrpcService)))
        ?.MakeGenericMethod(type);

    method?.Invoke(null, [app]);
}

app.Run();
