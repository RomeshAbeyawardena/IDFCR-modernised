using System.Net;

namespace IDFCR.AI.OpenAI.Exceptions;

/// <summary>
/// Exception thrown when an OpenAI request returns a non-success status code.
/// </summary>
public sealed class OpenAIHttpException(string message, HttpStatusCode statusCode, string? responseContent)
    : Exception(message)
{
    /// <summary>
    /// Gets the HTTP status code returned by the failed request.
    /// </summary>
    public HttpStatusCode StatusCode { get; } = statusCode;

    /// <summary>
    /// Gets the response content returned by the failed request, if any.
    /// </summary>
    public string? ResponseContent { get; } = responseContent;
}
