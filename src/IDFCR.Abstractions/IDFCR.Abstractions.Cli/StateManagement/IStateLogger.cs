namespace IDFCR.Abstractions.Cli.StateManagement;

/// <summary>
/// Defines a logger interface for tracking state changes of a specific type.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IStateLogger<T>
{
    /// <summary>
    /// Defines a method to append a new state value associated with a given name. This allows for recording the initial state or adding new entries over time.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="initialValue"></param>
    void Append(string name, T initialValue);
    /// <summary>
    /// Defines a method to update the state value associated with a given name. This allows for tracking changes over time.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void Update(string name, T value);
}
