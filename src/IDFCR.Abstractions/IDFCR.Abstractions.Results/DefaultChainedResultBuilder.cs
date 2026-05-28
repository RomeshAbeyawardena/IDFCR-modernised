using IDFCR.Abstractions.Results.Extensions;

namespace IDFCR.Abstractions.Results;

internal class DefaultChainedResultBuilder : IChainedResultBuilder
{
    private readonly List<IUnitResult> results = [];
    public void Add(IUnitResult result)
    {
        results.Add(result);
    }

    public IChainedUnitResult Build(IUnitResult result, bool setAsFailWhenAnyUnitsFail = false)
    {
        return result.Chain(results, setAsFailWhenAnyUnitsFail);
    }

    public IChainedUnitResult<T> Build<T>(IUnitResult<T> result, bool setAsFailWhenAnyUnitsFail = false)
    {
        return result.Chain(results, setAsFailWhenAnyUnitsFail);
    }
}
