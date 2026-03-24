using System.Net;
using System.Text.Json;
using IDFCR.AI.Abstractions;
using IDFCR.AI.OpenAI.Configurations;
using IDFCR.AI.OpenAI.Exceptions;
using IDFCR.AI.OpenAI.Models;
using Moq;
using NUnit.Framework;

namespace IDFCR.AI.OpenAI.Tests;

public class OpenAIServiceTests
{
    [Test]
    public async Task VerifyConnection_Should_Delegate_To_Base_AI_Service()
    {
        var aiService = new Mock<IAIService>(MockBehavior.Strict);
        var configuration = OpenAIConfiguration.Create("test-key");

        aiService.Setup(service => service.VerifyConnection(configuration, CancellationToken.None))
            .ReturnsAsync(VerifiedConnectionResult.Success());

        var service = new OpenAIService(aiService.Object);
        var result = await service.VerifyConnection(configuration, CancellationToken.None);

        Assert.That(result.IsSuccessful, Is.True);
    }

    [Test]
    public async Task GenerateTextAsync_Should_Build_Responses_Request_Using_OpenAI_Configuration()
    {
        var aiService = new Mock<IAIService>(MockBehavior.Strict);
        var configuration = OpenAIConfiguration.Create("test-key") with
        {
            Model = "gpt-5-mini"
        };
        AIServiceRequest? capturedRequest = null;

        aiService.Setup(service => service.SendAsync(configuration, It.IsAny<AIServiceRequest>(), CancellationToken.None))
            .Callback<OpenAIConfiguration, AIServiceRequest, CancellationToken>((_, request, _) => capturedRequest = request)
            .ReturnsAsync(new AIServiceResponse(HttpStatusCode.OK, "{\"id\":\"resp_123\",\"status\":\"completed\",\"output_text\":\"Hello there\"}", new Dictionary<string, IReadOnlyCollection<string>>()));

        var service = new OpenAIService(aiService.Object);

        var response = await service.GenerateTextAsync(configuration, new OpenAITextRequest
        {
            Prompt = "Say hello",
            Instructions = "Be cheerful",
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["x-request-id"] = "req-123"
            }
        }, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.OutputText, Is.EqualTo("Hello there"));
            Assert.That(capturedRequest, Is.Not.Null);
            Assert.That(capturedRequest!.Method, Is.EqualTo(HttpMethod.Post.Method));
            Assert.That(capturedRequest.RelativePath, Is.EqualTo("v1/responses"));
            Assert.That(capturedRequest.ContentType, Is.EqualTo("application/json"));
            Assert.That(capturedRequest.Headers["x-request-id"], Is.EqualTo("req-123"));
        });

        using var document = JsonDocument.Parse(capturedRequest!.Content!);
        Assert.Multiple(() =>
        {
            Assert.That(document.RootElement.GetProperty("model").GetString(), Is.EqualTo("gpt-5-mini"));
            Assert.That(document.RootElement.GetProperty("instructions").GetString(), Is.EqualTo("Be cheerful"));
            Assert.That(document.RootElement.GetProperty("input").GetString(), Is.EqualTo("Say hello"));
        });
    }

    [Test]
    public async Task GenerateTextAsync_Should_Fall_Back_To_Nested_Output_Text()
    {
        var aiService = new Mock<IAIService>(MockBehavior.Strict);
        var configuration = OpenAIConfiguration.Create("test-key");
        const string content = """
            {
              "id": "resp_nested",
              "status": "completed",
              "output": [
                {
                  "content": [
                    {
                      "type": "output_text",
                      "text": "Nested hello"
                    }
                  ]
                }
              ]
            }
            """;

        aiService.Setup(service => service.SendAsync(configuration, It.IsAny<AIServiceRequest>(), CancellationToken.None))
            .ReturnsAsync(new AIServiceResponse(HttpStatusCode.OK, content, new Dictionary<string, IReadOnlyCollection<string>>()));

        var service = new OpenAIService(aiService.Object);
        var response = await service.GenerateTextAsync(configuration, new OpenAITextRequest
        {
            Prompt = "Say hello"
        }, CancellationToken.None);

        Assert.That(response.OutputText, Is.EqualTo("Nested hello"));
    }

    [Test]
    public void GenerateTextAsync_Should_Throw_OpenAIHttpException_For_Unsuccessful_Response()
    {
        var aiService = new Mock<IAIService>(MockBehavior.Strict);
        var configuration = OpenAIConfiguration.Create("test-key");

        aiService.Setup(service => service.SendAsync(configuration, It.IsAny<AIServiceRequest>(), CancellationToken.None))
            .ReturnsAsync(new AIServiceResponse(HttpStatusCode.BadRequest, "{\"error\":\"invalid\"}", new Dictionary<string, IReadOnlyCollection<string>>()));

        var service = new OpenAIService(aiService.Object);

        var exception = Assert.ThrowsAsync<OpenAIHttpException>(() => service.GenerateTextAsync(configuration, new OpenAITextRequest
        {
            Prompt = "Say hello"
        }, CancellationToken.None));

        Assert.That(exception!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}
