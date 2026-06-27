using System.Collections.Frozen;

namespace IDFCR.Utilities;

internal sealed class DefaultSwitchBuilder<TKey, TValue>() : ISwitchBuilder<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, Func<TKey, TValue>> internalDictionary = [];

    public DefaultSwitchBuilder(ISwitch<TKey, TValue> sourceSwitch)
        : this()
    {
        if (sourceSwitch is DefaultSwitch<TKey,TValue> defaultSwitch)
        {
            foreach(var (key, value) in defaultSwitch.InternalDictionary)
            {
                CaseWhen(key, value);
            }

            elseValueFactory = defaultSwitch.ElseValueFactory;
        }
    }

    private Func<TKey, TValue>? elseValueFactory;
    public ISwitch<TKey, TValue> Build()
    {
        return new DefaultSwitch<TKey, TValue>(internalDictionary
            .ToFrozenDictionary(), elseValueFactory);
    }

    public ISwitchBuilder<TKey, TValue> CaseWhen(TKey key, Func<TKey, TValue> valueFactory)
    {
        internalDictionary[key] = valueFactory;
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
