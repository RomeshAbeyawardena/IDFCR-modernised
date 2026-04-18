using IDFCR.AI.Abstractions;
using IDFCR.AI.Http.Configurations;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Net;
using System.Text;

namespace IDFCR.AI.Http.Tests;

public class HttpAIServiceTests
{
    [Test]
    public async Task VerifyConnection_Should_Return_Success_For_Success_Status_Code()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK));
        var configuration = CreateConfiguration();

        var result = await service.VerifyConnection(configuration, CancellationToken.None);

        Assert.That(result.IsSuccessful, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task VerifyConnection_Should_Return_Failure_When_Request_Throws()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("boom"));

        var service = new HttpAIService(new HttpClient(handler.Object));
        var configuration = CreateConfiguration();

        var result = await service.VerifyConnection(configuration, CancellationToken.None);

        Assert.That(result.IsSuccessful, Is.False);
        Assert.That(result.Message, Is.EqualTo("boom"));
    }

    [Test]
    public async Task SendAsync_Should_Send_Request_To_Configured_Endpoint()
    {
        HttpRequestMessage? capturedRequest = null;
        var response = new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = new StringContent("{\"ok\":true}", Encoding.UTF8, "application/json")
        };

        var service = CreateService(response, request => capturedRequest = request);
        var configuration = CreateConfiguration() with
        {
            DefaultHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Authorization"] = "Bearer token"
            }
        };

        var result = await service.SendAsync(configuration, new AIServiceRequest
        {
            Method = HttpMethod.Post.Method,
            RelativePath = "/v1/chat/completions",
            Content = "{\"model\":\"test\"}",
            ContentType = "application/json",
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["x-request-id"] = "abc123"
            }
        }, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
            Assert.That(result.Content, Is.EqualTo("{\"ok\":true}"));
            Assert.That(capturedRequest, Is.Not.Null);
            Assert.That(capturedRequest!.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(capturedRequest.RequestUri, Is.EqualTo(new Uri("https://example.test/v1/chat/completions")));
            Assert.That(capturedRequest.Headers.Authorization?.Scheme, Is.EqualTo("Bearer"));
            Assert.That(capturedRequest.Headers.Authorization?.Parameter, Is.EqualTo("token"));
            Assert.That(capturedRequest.Headers.GetValues("x-request-id"), Is.EqualTo(["abc123"]));
        });
    }

    [Test]
    public void SendAsync_Should_Throw_For_Unsupported_Configuration()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK));

        Assert.ThrowsAsync<NotSupportedException>(() => service.SendAsync(new UnsupportedConfiguration(), new AIServiceRequest
        {
            Method = HttpMethod.Get.Method
        }, CancellationToken.None));
    }

    [Test]
    public void SendAsync_Should_Respect_Configured_Timeout()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage _, CancellationToken token) =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200), token);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var service = new HttpAIService(new HttpClient(handler.Object));
        var configuration = CreateConfiguration() with
        {
            Timeout = TimeSpan.FromMilliseconds(25)
        };

        Assert.ThrowsAsync<TaskCanceledException>(() => service.SendAsync(configuration, new AIServiceRequest
        {
            Method = HttpMethod.Get.Method
        }, CancellationToken.None));
    }

    private static HttpAIService CreateService(HttpResponseMessage response, Action<HttpRequestMessage>? onRequest = null)
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) =>
            {
                onRequest?.Invoke(request);
                return response;
            });

        return new HttpAIService(new HttpClient(handler.Object));
    }

    private static HttpAIServiceConfiguration CreateConfiguration()
    {
        return new HttpAIServiceConfiguration
        {
            BaseAddress = new Uri("https://example.test"),
            VerificationPath = "/health"
        };
    }

    private sealed record UnsupportedConfiguration : IAIServiceConfiguration
    {
        public string ServiceName => "unsupported";

        public TimeSpan? Timeout => null;
    }
}
