using System.Collections.Concurrent;

namespace BuildTools.Cli.StateManagement;

public class CommandOperationState
{
    protected readonly ConcurrentDictionary<string, object?> _stateDictionary;

    internal CommandOperationState(IDictionary<string, object?>? stateDictionary)
    {
        _stateDictionary = stateDictionary is null
             ? [] 
             : new(stateDictionary);
    }

    public CommandOperationState() : this(null) { }

    public T? GetValue<T>(string key)
    {
        if (_stateDictionary.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    public void SetValue<T>(string key, T value)
    {
        _stateDictionary[key] = value;
    }
}
