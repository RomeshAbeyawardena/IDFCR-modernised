using IDFCR.Abstractions.Results;
using IDFCR.Results.Http.Extensions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.Text.Json;

namespace IDFCR.Results.Http.Tests;

[TestFixture]
internal class UnitHttpResultTests
{
    private TestHttpContext _httpContext;
    private TestHttpRequest _httpRequest;
    private TestHttpResponse _httpResponse;

    [SetUp]
    public void Setup()
    {
        var headers = new HeaderDictionary { { "Accept", "application/json" } };
        _httpRequest = new(headers);
        _httpResponse = new();
        _httpContext = new(_httpRequest, _httpResponse);

        _httpRequest.Context = _httpContext;
        _httpResponse.Context = _httpContext;
    }

    [Test]
    public async Task ExecuteAsync_WithSuccessResult_ReturnsStatusCode200()
    {
        // Arrange
        var result = UnitResult.Success(UnitAction.Add).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }

    [Test]
    public async Task ExecuteAsync_WithSuccessResult_WritesExpectedJsonPayload()
    {
        // Arrange
        var result = UnitResult.Success(UnitAction.Add)
            .AddMeta("traceId", "abc-123")
            .AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        var body = _httpResponse.GetBodyAsString();
        Assert.That(body, Is.Not.Empty);

        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        var meta = root.GetProperty("_meta");
    
        Assert.That(root.GetProperty("isSuccess").GetBoolean(), Is.True);
        Assert.That(meta.GetProperty("action").GetString(), Is.EqualTo(nameof(UnitAction.Add)));
        Assert.That(meta.GetProperty("traceId").GetString(), Is.EqualTo("abc-123"));
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidAcceptHeader_ReturnsStatus406()
    {
        // Arrange
        var headers = new HeaderDictionary { { "Accept", "application/xml" } };
        _httpRequest = new(headers);
        _httpResponse = new();
        _httpContext = new(_httpRequest, _httpResponse);

        _httpRequest.Context = _httpContext;
        _httpResponse.Context = _httpContext;

        var result = UnitResult.Success(UnitAction.Add).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status406NotAcceptable));
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidAcceptHeader_WritesExpectedErrorPayload()
    {
        // Arrange
        var headers = new HeaderDictionary { { "Accept", "application/xml" } };
        _httpRequest = new(headers);
        _httpResponse = new();
        _httpContext = new(_httpRequest, _httpResponse);

        _httpRequest.Context = _httpContext;
        _httpResponse.Context = _httpContext;

        var result = UnitResult.Success(UnitAction.Add).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        var body = _httpResponse.GetBodyAsString();

        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status406NotAcceptable));
        Assert.That(body, Is.EqualTo("Invalid accept header"));
    }

    internal class Customer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset RegisteredDate { get; set; }
    }

    [Test]
    public async Task ExecuteAsyncWithResultPayload()
    {
        // Arrange
        var result = UnitResult.FromResult(new Customer
        {
            Id = 43428,
            Name = "Bobertson",
            RegisteredDate = new DateTimeOffset(2025, 02, 03, 11, 30, 00, TimeSpan.Zero)
        }, UnitAction.Add);

        result.AddMeta("traceId", "abc-123");
        var httpResult = result.AsHttp();

        // Act
        await httpResult.ExecuteAsync(_httpContext);

        // Assert
        var body = _httpResponse.GetBodyAsString();
        Assert.That(body, Is.Not.Empty);

        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        var meta = root.GetProperty("_meta");

        Assert.That(root.GetProperty(Abstractions.Metadata.Meta.SuccessKey).GetBoolean(), Is.True);
        Assert.That(meta.GetProperty(Abstractions.Metadata.Meta.ActionKey).GetString(), Is.EqualTo(nameof(UnitAction.Add)));
        Assert.That(meta.GetProperty("traceId").GetString(), Is.EqualTo("abc-123"));
    }
}
