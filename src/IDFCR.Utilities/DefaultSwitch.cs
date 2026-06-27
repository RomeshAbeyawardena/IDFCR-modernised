namespace IDFCR.Utilities;

internal sealed class DefaultSwitch<TKey, TValue>(IDictionary<TKey, Func<TKey, TValue>> internalDictionary, Func<TKey, TValue>? elseValueFactory) : ISwitch<TKey, TValue>
    where TKey : notnull
{
    public Func<TKey, TValue>? Then(TKey key)
    {
        if (internalDictionary.TryGetValue(key, out var valueFactory))
        {
            return valueFactory;
        }

        if (elseValueFactory is not null)
        {
            return elseValueFactory;
        }

        return null;
    }

    public TValue? ThenValue(TKey key)
    {
        var valueFactory = Then(key);

        if (valueFactory is not null)
        {
            return valueFactory(key);
        }

        return default;
    }
}
