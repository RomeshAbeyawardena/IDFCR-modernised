using Microsoft.AspNetCore.Http;

namespace IDFCR.Results.Http.Tests;

internal class TestHttpRequest(IHeaderDictionary headers) : HttpRequest
{
    public HttpContext Context { get; set; }

    public override HttpContext HttpContext => Context;
    public override string Method { get; set; } = "GET";
    public override string Scheme { get; set; } = "https";
    public override bool IsHttps { get; set; } = true;
    public override HostString Host { get; set; }
    public override PathString PathBase { get; set; }
    public override PathString Path { get; set; }
    public override QueryString QueryString { get; set; }
    public override IQueryCollection Query { get; set; } = new QueryCollection();
    public override string Protocol { get; set; } = "HTTP/1.1";
    public override IHeaderDictionary Headers { get; } = headers;
    public override IRequestCookieCollection Cookies { get; set; }
    public override long? ContentLength { get; set; }
    public override string ContentType { get; set; }
    public override Stream Body { get; set; } = Stream.Null;
    public override bool HasFormContentType => false;
    public override IFormCollection Form { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
