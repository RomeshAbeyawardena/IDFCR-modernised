using IDFCR.Abstractions.Results;
using Microsoft.AspNetCore.Http;

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

internal class UnitHttpResult<T>(IUnitResult<T> result) : UnitHttpResult(result)
{
    public override Task WriteResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        return response.WriteAsJsonAsync(result, cancellationToken);
    }
}

internal class UnitHttpResult(IUnitResult result) : IUnitHttpResult
{
    public virtual Task WriteResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        return response.WriteAsJsonAsync(result, cancellationToken);
    }

    public virtual int GetStatusCode()
    {
        return result.FailureReason switch
        {
            FailureReason.AuthorizationError => StatusCodes.Status401Unauthorized,
            FailureReason.Conflict => StatusCodes.Status409Conflict,
            FailureReason.ExternalDependencyError => StatusCodes.Status424FailedDependency,
            FailureReason.Forbidden => StatusCodes.Status403Forbidden,
            FailureReason.InternalError => StatusCodes.Status500InternalServerError,
            FailureReason.NotFound => StatusCodes.Status404NotFound,
            FailureReason.Unauthorized => StatusCodes.Status401Unauthorized,
            FailureReason.ValidationError => StatusCodes.Status400BadRequest,
            FailureReason.Unknown => StatusCodes.Status503ServiceUnavailable,
            FailureReason.None => StatusCodes.Status200OK,
            _ => StatusCodes.Status200OK
        };
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        var response = httpContext.Response;
        var headers = httpContext.Request.Headers;

        if (string.IsNullOrWhiteSpace(headers.Accept) 
            || headers.Accept == "application/json")
        {
            response.StatusCode = GetStatusCode();
            return WriteResponse(response, httpContext.RequestAborted);
        }

        response.StatusCode = StatusCodes.Status406NotAcceptable;
        return response.WriteAsync("Invalid accept header", httpContext.RequestAborted);
    }
}
