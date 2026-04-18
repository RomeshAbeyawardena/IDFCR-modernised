using BuildTools.GRPC.Application.Extensions;
using Grpc.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
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
