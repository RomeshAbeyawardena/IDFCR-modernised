using System.Collections.Frozen;

namespace IDFCR.Utilities;

internal sealed class DefaultContextualSwitchBuilder<TKey, TContext, TValue>() : IContextualSwitchBuilder<TKey, TContext, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, Func<TKey, TContext, TValue>> internalDictionary = [];

    public DefaultContextualSwitchBuilder(IContextualSwitch<TKey, TContext, TValue> sourceSwitch)
        : this()
    {
        if (sourceSwitch is DefaultContextualSwitch<TKey, TContext, TValue> defaultSwitch)
        {
            foreach (var (key, value) in defaultSwitch.InternalDictionary)
            {
                CaseWhen(key, value);
            }

            elseValueFactory = defaultSwitch.ElseValueFactory;
        }
    }

    private Func<TKey, TContext, TValue>? elseValueFactory;

    public IContextualSwitch<TKey, TContext, TValue> Build()
    {
        return new DefaultContextualSwitch<TKey, TContext, TValue>(internalDictionary
            .ToFrozenDictionary(), elseValueFactory);
    }

    public IContextualSwitchBuilder<TKey, TContext, TValue> CaseWhen(TKey key, Func<TKey, TContext, TValue> valueFactory)
    {
        internalDictionary[key] = valueFactory;
        return this;
    }

    public IContextualSwitchBuilder<TKey, TContext, TValue> CaseWhen(TKey key, TValue value)
    {
        return CaseWhen(key, (_, _) => value);
    }

    public IContextualSwitchBuilder<TKey, TContext, TValue> Else(Func<TKey, TContext, TValue> valueFactory)
    {
        elseValueFactory = valueFactory;
        return this;
    }

    public IContextualSwitchBuilder<TKey, TContext, TValue> Else(TValue value)
    {
        return Else((_, _) => value);
    }
}
