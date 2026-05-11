using IDFCR.Abstractions.Results;
using Microsoft.AspNetCore.Http;

namespace IDFCR.Results.Http;

internal class ChainedUnitHttpResult(IChainedUnitResult result) : UnitHttpResult(result)
{
    public override Task WriteResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        foreach (var item in result.Enumerate().GroupBy(x => x.FailureReason))
        {
            foreach (var result in item)
            {
                if (result.Exception is null)
                {
                    continue;
                }

                result.Meta
            }
        }



        return response.WriteAsJsonAsync(result, cancellationToken);
    }
}

internal class ChainedUnitHttpResult<T>(IChainedUnitResult<T> result) : UnitHttpResult(result)
{
    public override Task WriteResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        return response.WriteAsJsonAsync(result, cancellationToken);
    }
}

internal class UnitHttpResultCollection<T>(IUnitResultCollection<T> result) : UnitHttpResult(result)
{
    public override Task WriteResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        var unitResult = new UnitResultCollection<T>(result);

        return response.WriteAsJsonAsync(unitResult, cancellationToken);
    }
}

internal class UnitHttpResult<T>(Abstractions.Results.IUnitResult<T> result) : UnitHttpResult(result)
{
    public override Task WriteResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        var unitResult = new UnitResult<T>(unitResultWithValue: result);

        return response.WriteAsJsonAsync(unitResult, cancellationToken);
    }
}

internal class UnitHttpResult(Abstractions.Results.IUnitResult result) : IUnitHttpResult
{
    public virtual Task WriteResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        var unitResult = new UnitResult(result);

        return response.WriteAsJsonAsync(unitResult, cancellationToken);
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