using Microsoft.AspNetCore.Http;
using System.IO.Pipelines;
using System.Text;

namespace IDFCR.Results.Http.Tests;

internal class TestHttpResponse() : HttpResponse
{
    private readonly MemoryStream _bodyStream = new();
    private readonly TestPipeWriter _pipeWriter = new();
    private bool _started = false;

    public HttpContext Context { get; set; }

    public override HttpContext HttpContext => Context;

    public override int StatusCode { get; set; } = StatusCodes.Status200OK;
    public override IHeaderDictionary Headers { get; } = new HeaderDictionary();
    public override Stream Body 
    { 
        get => _bodyStream; 
        set { } // Allow assignment but use internal stream
    }
    public override long? ContentLength { get; set; }
    public override string ContentType { get; set; }
    public override IResponseCookies Cookies => throw new NotImplementedException();
    public override PipeWriter BodyWriter => _pipeWriter;
    public override bool HasStarted => _started;

    public string GetBodyAsString() => Encoding.UTF8.GetString(_bodyStream.ToArray());

    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        _started = true;
        return Task.CompletedTask;
    }

    public override void OnStarting(Func<object, Task> callback, object state) { }
    public override void OnCompleted(Func<object, Task> callback, object state) { }
    public override void Redirect(string location, bool permanent) { }
}
