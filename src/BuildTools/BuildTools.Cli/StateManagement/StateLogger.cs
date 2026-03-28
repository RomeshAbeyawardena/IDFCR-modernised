using System.Collections.Concurrent;

namespace BuildTools.Cli.StateManagement;

public class StateLogger<T>(TimeProvider timeProvider) : IStateLogger<T>
{
    private readonly ConcurrentDictionary<string, IState<T>> _stateValues = [];
    public void Append(string name, T initialValue)
    {
        if (_stateValues.ContainsKey(name))
        {
            throw new InvalidOperationException("State key already exists");
        }

        if (!_stateValues.TryAdd(name, new State<T>(name, initialValue, timeProvider.GetUtcNow())))
        {
            throw new InvalidOperationException("Unable to add state");
        }
    }

    public void Update(string name, T value)
    {
        if (!_stateValues.TryGetValue(name, out var state))
        {
            throw new NullReferenceException("Key not found");
        }

        state.Update(value, timeProvider.GetUtcNow());
    }
}
