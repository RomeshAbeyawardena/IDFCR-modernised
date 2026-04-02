using System.Collections.Concurrent;

namespace IDFCR.Abstractions.Cli.StateManagement;

/// <summary>
/// Represents a thread-safe key/value state container used to share data
/// across command operation processing.
/// </summary>
public class CommandOperationState
{
    /// <summary>
    /// Stores state entries by key in a concurrent dictionary to support
    /// safe access from multiple threads.
    /// </summary>
    protected readonly ConcurrentDictionary<string, object?> _stateDictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandOperationState"/> class
    /// using an optional initial state dictionary.
    /// </summary>
    /// <param name="stateDictionary">
    /// Optional initial state values to seed the internal dictionary.
    /// When <see langword="null"/>, an empty dictionary is created.
    /// </param>
    internal CommandOperationState(IDictionary<string, object?>? stateDictionary)
    {
        _stateDictionary = stateDictionary is null
             ? []
             : new(stateDictionary);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandOperationState"/> class
    /// with an empty state dictionary.
    /// </summary>
    public CommandOperationState() : this(null) { }

    /// <summary>
    /// Gets a typed value from the state dictionary for the specified key.
    /// </summary>
    /// <typeparam name="T">The expected type of the stored value.</typeparam>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>
    /// The value cast to <typeparamref name="T"/> when present and type-compatible;
    /// otherwise <see langword="default"/>.
    /// </returns>
    public T? GetValue<T>(string key)
    {
        if (_stateDictionary.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    /// <summary>
    /// Sets or replaces a value in the state dictionary for the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value being stored.</typeparam>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to associate with the key.</param>
    public void SetValue<T>(string key, T value)
    {
        _stateDictionary[key] = value;
    }
}
