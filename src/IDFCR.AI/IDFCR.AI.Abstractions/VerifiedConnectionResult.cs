using System.Net;

namespace IDFCR.AI.Abstractions;

/// <summary>
/// Represents the outcome of a connection-verification request.
/// </summary>
/// <param name="IsSuccessful">A value indicating whether verification succeeded.</param>
/// <param name="StatusCode">The status code returned by the service, if available.</param>
/// <param name="Message">An optional message describing the result.</param>
public sealed record VerifiedConnectionResult(bool IsSuccessful, HttpStatusCode? StatusCode = null, string? Message = null)
{
    /// <summary>
    /// Creates a successful verification result.
    /// </summary>
    /// <param name="statusCode">The status code returned by the service, if available.</param>
    /// <param name="message">An optional message describing the verification.</param>
    /// <returns>A successful verification result.</returns>
    public static VerifiedConnectionResult Success(HttpStatusCode? statusCode = null, string? message = null) =>
        new(true, statusCode, message);

    /// <summary>
    /// Creates a failed verification result.
    /// </summary>
    /// <param name="message">A message describing the failure.</param>
    /// <param name="statusCode">The status code returned by the service, if available.</param>
    /// <returns>A failed verification result.</returns>
    public static VerifiedConnectionResult Failure(string message, HttpStatusCode? statusCode = null) =>
        new(false, statusCode, message);
}
