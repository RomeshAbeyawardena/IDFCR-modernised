using System.Collections.Concurrent;

namespace IDFCR.Utilities;

internal sealed class DefaultSwitchBuilder<TKey, TValue> : ISwitchBuilder<TKey, TValue>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, Func<TKey, TValue>> internalDictionary = [];

    private Func<TKey, TValue>? elseValueFactory;
    public ISwitch<TKey, TValue> Build()
    {
        return new DefaultSwitch<TKey, TValue>(internalDictionary.AsReadOnly(), elseValueFactory);
    }

    public ISwitchBuilder<TKey, TValue> CaseWhen(TKey key, Func<TKey, TValue> valueFactory)
    {
        if (!internalDictionary.TryAdd(key, valueFactory))
        {
            internalDictionary[key] = valueFactory;
        }

        return this;
    }

    public ISwitchBuilder<TKey, TValue> CaseWhen(TKey key, TValue value)
    {
        return CaseWhen(key, _ => value);
    }

    public ISwitchBuilder<TKey, TValue> Else(Func<TKey, TValue> valueFactory)
    {
        elseValueFactory = valueFactory;
        return this;
    }

    public ISwitchBuilder<TKey, TValue> Else(TValue value)
    {
        return Else(_ => value);
    }
}
