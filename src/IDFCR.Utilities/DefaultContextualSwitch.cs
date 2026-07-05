namespace IDFCR.Utilities;

internal sealed class DefaultContextualSwitch<TKey, TContext, TValue>(
    IReadOnlyDictionary<TKey, Func<TKey, TContext, TValue>> internalDictionary,
    Func<TKey, TContext, TValue>? elseValueFactory) : IContextualSwitch<TKey, TContext, TValue>
    where TKey : notnull
{
    //Internal because we don't want this leaking in the debugger
    internal Func<TKey, TContext, TValue>? ElseValueFactory => elseValueFactory;
    //Internal because we don't want this leaking in the debugger
    internal IReadOnlyDictionary<TKey, Func<TKey, TContext, TValue>> InternalDictionary => internalDictionary;

    public Func<TKey, TContext, TValue>? Then(TKey key)
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

    public TValue? ThenValue(TKey key, TContext context)
    {
        var valueFactory = Then(key);

        if (valueFactory is not null)
        {
            return valueFactory(key, context);
        }

        return default;
    }
}
