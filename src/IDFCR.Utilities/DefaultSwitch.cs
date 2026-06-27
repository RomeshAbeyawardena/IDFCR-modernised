namespace IDFCR.Utilities;

internal sealed class DefaultSwitch<TKey, TValue>(IReadOnlyDictionary<TKey, Func<TKey, TValue>> internalDictionary, Func<TKey, TValue>? elseValueFactory) : ISwitch<TKey, TValue>
    where TKey : notnull
{
    //Internal because we don't want this leaking in the debugger
    internal Func<TKey, TValue>? ElseValueFactory => elseValueFactory;
    //Internal because we don't want this leaking in the debugger
    internal IReadOnlyDictionary<TKey, Func<TKey, TValue>> InternalDictionary => internalDictionary;

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
