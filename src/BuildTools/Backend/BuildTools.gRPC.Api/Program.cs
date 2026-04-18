using BuildTools.GRPC.Application.Extensions;
using BuildTools.Infrastructure;
using BuildTools.Application.Extensions;

using BuildTools.Infrastructure.SqlServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

var dbSettings = builder.Configuration.Get<DbSettings>();

var services = builder.Services;
services
    .AddGrpc();

services.AddSingleton(TimeProvider.System)
       .AddRepositories(dbSettings ?? throw new InvalidOperationException("Unable to bind settings"))
       .AddMediatorServices();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

foreach (var type in GrpcServices.Fetch())
{
    var method = typeof(GrpcEndpointRouteBuilderExtensions)
        .GetMethods()
        .FirstOrDefault(x => x.IsGenericMethod && x.Name.StartsWith(nameof(GrpcEndpointRouteBuilderExtensions.MapGrpcService)))
        ?.MakeGenericMethod(type);

    method?.Invoke(null, [ app ]);
}

app.Run();
