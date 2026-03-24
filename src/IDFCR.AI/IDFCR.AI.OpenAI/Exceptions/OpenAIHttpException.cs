using System.Net;

namespace IDFCR.AI.OpenAI.Exceptions;

public sealed class OpenAIHttpException(string message, HttpStatusCode statusCode, string? responseContent)
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public string? ResponseContent { get; } = responseContent;
}
