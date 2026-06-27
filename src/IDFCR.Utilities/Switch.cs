using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace IDFCR.Utilities;

public interface ISwitchBuilder<TKey, TValue>
    where TKey: notnull
{
    ISwitchBuilder<TKey, TValue> CaseWhen(TKey key, Func<TKey, TValue> valueFactory);
    ISwitchBuilder<TKey, TValue> CaseWhen(TKey key, TValue value);

    ISwitchBuilder<TKey, TValue> Else(Func<TKey, TValue> valueFactory);
    ISwitchBuilder<TKey, TValue> Else(TValue value);

    ISwitch<TKey, TValue> Build();
}

public interface ISwitch<TKey, TValue>
    where TKey : notnull
{
    Func<TKey, TValue>? Then(TKey key);
    TValue? ThenValue(TKey key);
}

internal sealed class DefaultSwitch<TKey, TValue>(IDictionary<TKey, Func<TKey, TValue>> internalDictionary) : ISwitch<TKey, TValue>
    where TKey : notnull
{
    public Func<TKey, TValue>? Then(TKey key)
    {
        throw new NotImplementedException();
    }

    public TValue? ThenValue(TKey key)
    {
        throw new NotImplementedException();
    }
}

internal sealed class DefaultSwitchBuilder<TKey, TValue> : ISwitchBuilder<TKey, TValue>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, Func<TKey, TValue>> internalDictionary = [];

    public ISwitch<TKey, TValue> Build()
    {
        return new DefaultSwitch<TKey, TValue>(internalDictionary.AsReadOnly());
    }

    public ISwitchBuilder<TKey, TValue> CaseWhen(TKey key, Func<TKey, TValue> valueFactory)
    {
        
    }

    public ISwitchBuilder<TKey, TValue> CaseWhen(TKey key, TValue value)
    {
        throw new NotImplementedException();
    }

    public ISwitchBuilder<TKey, TValue> Else(Func<TKey, TValue> valueFactory)
    {
        throw new NotImplementedException();
    }

    public ISwitchBuilder<TKey, TValue> Else(TValue value)
    {
        throw new NotImplementedException();
    }
}

public static class SwitchBuilder
{
}
