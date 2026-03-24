using System.Net;

namespace IDFCR.AI.Abstractions;

public sealed record VerifiedConnectionResult(bool IsSuccessful, HttpStatusCode? StatusCode = null, string? Message = null)
{
    public static VerifiedConnectionResult Success(HttpStatusCode? statusCode = null, string? message = null) =>
        new(true, statusCode, message);

    public static VerifiedConnectionResult Failure(string message, HttpStatusCode? statusCode = null) =>
        new(false, statusCode, message);
}
