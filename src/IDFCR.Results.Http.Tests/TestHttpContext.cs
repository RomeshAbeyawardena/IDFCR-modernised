using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;

namespace IDFCR.Results.Http.Tests;

internal class TestHttpContext(HttpRequest request, HttpResponse response) : HttpContext
{
    public override IFeatureCollection Features => throw new NotImplementedException();
    public override HttpRequest Request { get; } = request;
    public override HttpResponse Response { get; } = response;
    public override ConnectionInfo Connection => throw new NotImplementedException();
    public override WebSocketManager WebSockets => throw new NotImplementedException();
    public override ClaimsPrincipal User { get; set; }
    public override IDictionary<object, object?> Items { get; set; } = new Dictionary<object, object?>();
    public override IServiceProvider RequestServices { get; set; } = new DefaultServiceProvider();
    public override CancellationToken RequestAborted { get; set; } = CancellationToken.None;
    public override string TraceIdentifier { get; set; } = Guid.NewGuid().ToString();
    public override ISession Session { get; set; }

    public override void Abort() { }
}
