using Microsoft.AspNetCore.Http;
using System.IO.Pipelines;
using System.Text;

namespace IDFCR.Results.Http.Tests;

internal class TestHttpResponse : HttpResponse
{
    private readonly MemoryStream _bodyStream = new();
    private readonly PipeWriter _bodyWriter;
    private bool _started;

    public TestHttpResponse()
    {
        _bodyWriter = PipeWriter.Create(_bodyStream);
    }

    public HttpContext Context { get; set; }

    public override HttpContext HttpContext => Context;

    public override int StatusCode { get; set; } = StatusCodes.Status200OK;
    public override IHeaderDictionary Headers { get; } = new HeaderDictionary();
    public override Stream Body
    {
        get => _bodyStream;
        set { }
    }
    public override long? ContentLength { get; set; }
    public override string ContentType { get; set; }
    public override IResponseCookies Cookies => throw new NotImplementedException();
    public override PipeWriter BodyWriter => _bodyWriter;
    public override bool HasStarted => _started;

    public string GetBodyAsString()
    {
        _bodyStream.Position = 0;
        return Encoding.UTF8.GetString(_bodyStream.ToArray());
    }

    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        _started = true;
        return Task.CompletedTask;
    }

    public override void OnStarting(Func<object, Task> callback, object state) { }
    public override void OnCompleted(Func<object, Task> callback, object state) { }
    public override void Redirect(string location, bool permanent) { }
}
