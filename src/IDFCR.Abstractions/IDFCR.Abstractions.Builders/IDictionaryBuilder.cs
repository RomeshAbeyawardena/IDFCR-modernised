namespace IDFCR.Abstractions.Builders
{
    public interface IDictionaryBuilder<TKey, TValue>
    where TKey : notnull
    {
        IDictionaryBuilder<TKey, TValue> AddOrUpdate(TKey key, TValue value);
        IDictionaryBuilder<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue> updateFunc);
        Dictionary<TKey, TValue> Build();
    }
}
