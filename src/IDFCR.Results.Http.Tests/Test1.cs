using IDFCR.Abstractions.Results;
using IDFCR.Results.Http.Extensions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

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
        
        // Wire them together
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
    public async Task ExecuteAsync_WithInvalidAcceptHeader_ReturnsStatus406()
    {
        // Arrange
        var headers = new HeaderDictionary { { "Accept", "application/xml" } };
        _httpRequest = new(headers);
        _httpResponse = new();
        _httpContext = new(_httpRequest, _httpResponse);
        
        // Wire them together
        _httpRequest.Context = _httpContext;
        _httpResponse.Context = _httpContext;

        var result = UnitResult.Success(UnitAction.Add).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status406NotAcceptable));
    }
}
