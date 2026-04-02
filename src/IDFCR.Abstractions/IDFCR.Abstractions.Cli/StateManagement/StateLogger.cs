using System.Collections.Concurrent;

namespace IDFCR.Abstractions.Cli.StateManagement;

/// <summary>
/// Represents a thread-safe logger for state snapshots of a specific type, allowing for appending new state entries and updating existing ones while maintaining timestamps for each entry. This class is designed to facilitate tracking changes to state information across command operation processing in a concurrent environment.
/// </summary>
/// <typeparam name="T">The type of the value stored in the state snapshots.</typeparam>
/// <param name="timeProvider">The time provider used to obtain the current UTC time for state updates.</param>
public class StateLogger<T>(TimeProvider timeProvider) : IStateLogger<T>
{
    private readonly ConcurrentDictionary<string, IState<T>> _stateValues = [];

    /// <summary>
    /// Defines a method to append a new state entry with the specified name and initial value. If an entry with the same name already exists, an InvalidOperationException is thrown to prevent duplicate keys. The method uses the provided time provider to set the creation timestamp for the new state entry.
    /// </summary>
    /// <param name="name">The name of the state entry to append.</param>
    /// <param name="initialValue">The initial value of the state entry.</param>
    /// <exception cref="InvalidOperationException">Thrown if a state entry with the same name already exists or if the state entry cannot be added.</exception>
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

    /// <summary>
    /// Defines a method to update the value of an existing state entry identified by the specified name. If no entry with the given name exists, a NullReferenceException is thrown. The method uses the provided time provider to set the updated timestamp for the state entry when the value is updated.
    /// </summary>
    /// <param name="name">The name of the state entry to update.</param>
    /// <param name="value">The new value to assign to the state entry.</param>
    /// <exception cref="NullReferenceException">Thrown if no state entry with the specified name exists.</exception>
    public void Update(string name, T value)
    {
        if (!_stateValues.TryGetValue(name, out var state))
        {
            throw new NullReferenceException("Key not found");
        }

        state.Update(value, timeProvider.GetUtcNow());
    }
}
