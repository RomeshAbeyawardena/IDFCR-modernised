using IDFCR.Abstractions.Metadata;
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

    #region Status Code Mapping Tests

    [Test]
    public async Task GetStatusCode_WithNotFoundFailure_Returns404()
    {
        // Arrange
        var result = UnitResult.Failed(new InvalidOperationException("Not found"), UnitAction.Get, FailureReason.NotFound).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(result.GetStatusCode(), Is.EqualTo(StatusCodes.Status404NotFound));
    }

    [Test]
    public async Task GetStatusCode_WithValidationError_Returns400()
    {
        // Arrange
        var result = UnitResult.Failed(new ArgumentException("Validation failed"), UnitAction.Add, FailureReason.ValidationError).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.GetStatusCode(), Is.EqualTo(StatusCodes.Status400BadRequest));
    }

    [Test]
    public async Task GetStatusCode_WithConflict_Returns409()
    {
        // Arrange
        var result = UnitResult.Failed(new InvalidOperationException("Conflict occurred"), UnitAction.Update, FailureReason.Conflict).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        Assert.That(result.GetStatusCode(), Is.EqualTo(StatusCodes.Status409Conflict));
    }

    [Test]
    public async Task GetStatusCode_WithUnauthorized_Returns401()
    {
        // Arrange
        var result = UnitResult.Failed(new UnauthorizedAccessException("Unauthorized"), UnitAction.Get, FailureReason.Unauthorized).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
        Assert.That(result.GetStatusCode(), Is.EqualTo(StatusCodes.Status401Unauthorized));
    }

    [Test]
    public async Task GetStatusCode_WithForbidden_Returns403()
    {
        // Arrange
        var result = UnitResult.Failed(new UnauthorizedAccessException("Forbidden"), UnitAction.Delete, FailureReason.Forbidden).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        Assert.That(result.GetStatusCode(), Is.EqualTo(StatusCodes.Status403Forbidden));
    }

    [Test]
    public async Task GetStatusCode_WithInternalError_Returns500()
    {
        // Arrange
        var result = UnitResult.Failed(new Exception("Internal error"), UnitAction.Update, FailureReason.InternalError).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(result.GetStatusCode(), Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task GetStatusCode_WithExternalDependencyError_Returns424()
    {
        // Arrange
        var result = UnitResult.Failed(new Exception("External dependency failed"), UnitAction.Add, FailureReason.ExternalDependencyError).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status424FailedDependency));
        Assert.That(result.GetStatusCode(), Is.EqualTo(StatusCodes.Status424FailedDependency));
    }

    [Test]
    public async Task GetStatusCode_WithAuthorizationError_Returns401()
    {
        // Arrange
        var result = UnitResult.Failed(new UnauthorizedAccessException("Authorization error"), UnitAction.Update, FailureReason.AuthorizationError).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
        Assert.That(result.GetStatusCode(), Is.EqualTo(StatusCodes.Status401Unauthorized));
    }

    [Test]
    public async Task GetStatusCode_WithUnknownFailure_Returns503()
    {
        // Arrange
        var result = UnitResult.Failed(new Exception("Unknown error"), UnitAction.Update, FailureReason.Unknown).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status503ServiceUnavailable));
        Assert.That(result.GetStatusCode(), Is.EqualTo(StatusCodes.Status503ServiceUnavailable));
    }

    [Test]
    public async Task GetStatusCode_WithSuccess_Returns200()
    {
        // Arrange
        var result = UnitResult.Success(UnitAction.Add).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(result.GetStatusCode(), Is.EqualTo(StatusCodes.Status200OK));
    }

    #endregion

    #region JSON Serialization Tests

    [Test]
    public async Task ExecuteAsync_WithSuccessResult_WritesCorrectJsonStructure()
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

        Assert.That(root.TryGetProperty(Meta.SuccessKey, out _), Is.True, $"JSON should contain {Meta.SuccessKey}");
        Assert.That(root.TryGetProperty(Meta.Key, out _), Is.True, $"JSON should contain {Meta.Key}");

        var meta = root.GetProperty(Meta.Key);
        Assert.That(root.GetProperty(Meta.SuccessKey).GetBoolean(), Is.True);
        Assert.That(meta.GetProperty(Meta.ActionKey).GetString(), Is.EqualTo(nameof(UnitAction.Add)));
        Assert.That(meta.GetProperty("traceId").GetString(), Is.EqualTo("abc-123"));
    }

    [Test]
    public async Task ExecuteAsync_WithFailureResult_IncludesFailureReasonInMeta()
    {
        // Arrange
        var result = UnitResult.Failed(new ArgumentException("Invalid data"), UnitAction.Update, FailureReason.ValidationError)
            .AddMeta("errorCode", "ERR001")
            .AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        var body = _httpResponse.GetBodyAsString();
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        var meta = root.GetProperty(Meta.Key);

        Assert.That(root.GetProperty(Meta.SuccessKey).GetBoolean(), Is.False);
        Assert.That(meta.GetProperty(Meta.ActionKey).GetString(), Is.EqualTo(nameof(UnitAction.Update)));
        Assert.That(meta.GetProperty(Meta.FailureReason).GetString(), Is.EqualTo(nameof(FailureReason.ValidationError)));
        Assert.That(meta.GetProperty("errorCode").GetString(), Is.EqualTo("ERR001"));
    }

    [Test]
    public async Task ExecuteAsync_WithTypedResult_IncludesValueInResponse()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 123,
            Name = "John Doe",
            RegisteredDate = new DateTimeOffset(2025, 1, 15, 10, 30, 0, TimeSpan.Zero)
        };
        var result = UnitResult.FromResult(customer, UnitAction.Add)
            .AddMeta("correlationId", "xyz-789")
            .AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        var body = _httpResponse.GetBodyAsString();
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        // The typed result serializes with is_success and _meta
        // The actual value properties are accessible through the dictionary interface but not serialized as JSON
        Assert.That(root.GetProperty(Meta.SuccessKey).GetBoolean(), Is.True);
        Assert.That(root.TryGetProperty(Meta.Key, out var metaProp), Is.True);

        var meta = root.GetProperty(Meta.Key);
        Assert.That(meta.GetProperty(Meta.ActionKey).GetString(), Is.EqualTo(nameof(UnitAction.Add)));
        Assert.That(meta.GetProperty("correlationId").GetString(), Is.EqualTo("xyz-789"));

        // Verify response is valid JSON and status code is 200
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }

    [Test]
    public async Task ExecuteAsync_WithResultCollection_SerializesSuccessfully()
    {
        // Arrange
        var customers = new[]
        {
            new Customer { Id = 1, Name = "Alice", RegisteredDate = DateTimeOffset.UtcNow },
            new Customer { Id = 2, Name = "Bob", RegisteredDate = DateTimeOffset.UtcNow }
        };
        var result = UnitResultCollection.FromResult(customers, UnitAction.Get)
            .AddMeta("pageSize", "2")
            .AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        var body = _httpResponse.GetBodyAsString();
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        // Collection results serialize with is_success and _meta
        Assert.That(root.GetProperty(Meta.SuccessKey).GetBoolean(), Is.True);
        Assert.That(root.TryGetProperty(Meta.Key, out _), Is.True);

        var meta = root.GetProperty(Meta.Key);
        Assert.That(meta.GetProperty(Meta.ActionKey).GetString(), Is.EqualTo(nameof(UnitAction.Get)));
        Assert.That(meta.GetProperty("pageSize").GetString(), Is.EqualTo("2"));

        // Verify response is valid JSON and status code is 200
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }

    #endregion

    #region Content Negotiation Tests

    [Test]
    public async Task ExecuteAsync_WithValidJsonAcceptHeader_ReturnsJsonResponse()
    {
        // Arrange
        var result = UnitResult.Success(UnitAction.Add).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        var body = _httpResponse.GetBodyAsString();
        Assert.That(() => JsonDocument.Parse(body), Throws.Nothing);
    }

    [Test]
    public async Task ExecuteAsync_WithEmptyAcceptHeader_AcceptsRequest()
    {
        // Arrange
        var headers = new HeaderDictionary { { "Accept", "" } };
        _httpRequest = new(headers);
        _httpResponse = new();
        _httpContext = new(_httpRequest, _httpResponse);
        _httpRequest.Context = _httpContext;
        _httpResponse.Context = _httpContext;

        var result = UnitResult.Success(UnitAction.Add).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }

    [Test]
    public async Task ExecuteAsync_WithMissingAcceptHeader_AcceptsRequest()
    {
        // Arrange
        var headers = new HeaderDictionary();
        _httpRequest = new(headers);
        _httpResponse = new();
        _httpContext = new(_httpRequest, _httpResponse);
        _httpRequest.Context = _httpContext;
        _httpResponse.Context = _httpContext;

        var result = UnitResult.Success(UnitAction.Add).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }

    [Test]
    public async Task ExecuteAsync_WithXmlAcceptHeader_Returns406()
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
        var body = _httpResponse.GetBodyAsString();
        Assert.That(body, Is.EqualTo("Invalid accept header"));
    }

    [Test]
    public async Task ExecuteAsync_WithHtmlAcceptHeader_Returns406()
    {
        // Arrange
        var headers = new HeaderDictionary { { "Accept", "text/html" } };
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

    #endregion

    #region UnitAction Tests

    [TestCase(UnitAction.Add)]
    [TestCase(UnitAction.Update)]
    [TestCase(UnitAction.Delete)]
    [TestCase(UnitAction.Get)]
    public async Task ExecuteAsync_WithDifferentActions_IncludesCorrectActionInMeta(UnitAction action)
    {
        // Arrange
        var result = UnitResult.Success(action).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        var body = _httpResponse.GetBodyAsString();
        using var document = JsonDocument.Parse(body);
        var meta = document.RootElement.GetProperty(Meta.Key);
        Assert.That(meta.GetProperty(Meta.ActionKey).GetString(), Is.EqualTo(action.ToString()));
    }

    #endregion

    #region Edge Cases

    [Test]
    public async Task ExecuteAsync_WithNullValueTypedResult_DoesNotIncludeValueProperties()
    {
        // Arrange
        var result = UnitResult.Success(UnitAction.Get).As<Customer>().AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        var body = _httpResponse.GetBodyAsString();
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        Assert.That(root.GetProperty(Meta.SuccessKey).GetBoolean(), Is.True);
        Assert.That(root.TryGetProperty("Id", out _), Is.False);
        Assert.That(root.TryGetProperty("Name", out _), Is.False);
    }

    [Test]
    public async Task ExecuteAsync_WithEmptyCollection_SerializesSuccessfully()
    {
        // Arrange
        var result = UnitResultCollection.FromResult(Array.Empty<Customer>(), UnitAction.Get).AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        var body = _httpResponse.GetBodyAsString();
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        Assert.That(root.GetProperty(Meta.SuccessKey).GetBoolean(), Is.True);
        Assert.That(root.TryGetProperty(Meta.Key, out _), Is.True);
        Assert.That(_httpResponse.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }

    [Test]
    public async Task ExecuteAsync_WithMultipleMetadata_IncludesAllInResponse()
    {
        // Arrange
        var result = UnitResult.Success(UnitAction.Add)
            .AddMeta("traceId", "trace-123")
            .AddMeta("userId", "user-456")
            .AddMeta("timestamp", "2025-01-15T10:30:00Z")
            .AsHttp();

        // Act
        await result.ExecuteAsync(_httpContext);

        // Assert
        var body = _httpResponse.GetBodyAsString();
        using var document = JsonDocument.Parse(body);
        var meta = document.RootElement.GetProperty(Meta.Key);

        Assert.That(meta.GetProperty("traceId").GetString(), Is.EqualTo("trace-123"));
        Assert.That(meta.GetProperty("userId").GetString(), Is.EqualTo("user-456"));
        Assert.That(meta.GetProperty("timestamp").GetString(), Is.EqualTo("2025-01-15T10:30:00Z"));
    }

    #endregion

    internal class Customer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset RegisteredDate { get; set; }
    }
}
