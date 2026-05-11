using IDFCR.Abstractions.Results;
using IDFCR.Results.Http.Extensions;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace IDFCR.Results.Http;

internal class ChainedUnitHttpResult(IChainedUnitResult result) : UnitHttpResult(result)
{
    protected void PopulateMeta(UnitResult unitResult)
    {
        var entries = result.ToDictionary();

        foreach(var (key, value) in entries)
        {
            unitResult.InternalMeta.TryAdd(key, JsonSerializer.Serialize(value, JsonSerializerOptions.Web));
        }

        foreach (var item in result.Enumerate().GroupBy(x => x.FailureReason))
        {
            StringBuilder exceptions = new();
            foreach (var result in item)
            {
                if (!item.Key.HasValue)
                {
                    continue;
                }

                exceptions.AppendLine(item.Key.ToString());
                if (result.Exception is null)
                {
                    continue;
                }

                exceptions.AppendLine(result.Exception.Message);
            }

            if (exceptions.Length > 0)
            {
                unitResult.InternalMeta.TryAdd("errors", exceptions.ToString());
                exceptions.Clear();
            }
        }
    }

    public override Task WriteResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        var unitResult = new UnitResult(result);
        PopulateMeta(unitResult);
        return response.WriteAsJsonAsync(unitResult, cancellationToken);
    }
}


internal class ChainedUnitHttpResult<T>(IChainedUnitResult<T> result) : ChainedUnitHttpResult(result)
{
    public override Task WriteResponse(HttpResponse response, CancellationToken cancellationToken)
    {
        var unitResult = new UnitResult<T>(result);
        PopulateMeta(unitResult);
        return response.WriteAsJsonAsync(unitResult, cancellationToken);
    }
}
