namespace IDFCR.Results.Http;

internal class UnitResultCollection<T>(Abstractions.Results.IUnitResultCollection<T> items) : UnitResult<T>(items)
{
    protected override void AppendToSelf(Dictionary<string, object?> source)
    {
        if (items.HasValue)
        {
            source.TryAdd(items.NamedResult ?? Abstractions.Metadata.Meta.ItemKey, items.Result);
        }
    }
}
