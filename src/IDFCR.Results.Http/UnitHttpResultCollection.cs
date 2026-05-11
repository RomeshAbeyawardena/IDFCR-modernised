using IDFCR.Abstractions.Results;
using Microsoft.AspNetCore.Http;

namespace IDFCR.Results.Http;

internal class UnitHttpResultCollection<T>(IUnitResultCollection<T> result) : UnitHttpResult(result)
{
    public override Task WriteResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        var unitResult = new UnitResultCollection<T>(result);

        return response.WriteAsJsonAsync(unitResult, cancellationToken);
    }
}
