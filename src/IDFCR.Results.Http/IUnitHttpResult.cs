using IDFCR.Results.Http.Extensions;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Results.Http;

/// <summary>
/// Represents an HTTP result that can be returned from an API endpoint, based on an IUnitResult. It provides a method to write the response to the HttpResponse and a method to get the appropriate status code based on the failure reason of the result.
/// </summary>
public interface IUnitHttpResult : IResult
{
    /// <summary>
    /// Gets the appropriate HTTP status code based on the failure reason of the result. For example, if the failure reason is ValidationError, it returns 400 Bad Request; if it's NotFound, it returns 404 Not Found; if it's InternalError, it returns 500 Internal Server Error; and so on. If there is no failure (FailureReason.None), it returns 200 OK.
    /// </summary>
    /// <returns>The HTTP status code corresponding to the failure reason.</returns>
    int GetStatusCode();
}

internal interface IUnitResult
{
    IReadOnlyDictionary<string, string?> Meta { get; }
    bool IsSuccess { get; }
}

internal interface IUnitResult<T> : IReadOnlyDictionary<string, string?>, IUnitResult
{
    
}