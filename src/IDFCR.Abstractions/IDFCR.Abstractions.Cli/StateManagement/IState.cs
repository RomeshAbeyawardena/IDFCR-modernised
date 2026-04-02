namespace IDFCR.Abstractions.Cli.StateManagement;

/// <summary>
/// Represents a snapshot of state information at a given point in time during command operation processing.
/// </summary>
public interface IState
{
    /// <summary>
    /// Gets the name of the state snapshot.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets the timestamp when the state snapshot was created.
    /// </summary>
    DateTimeOffset CreatedTimestampUtc { get; }


    /// <summary>
    /// Gets the timestamp when the state snapshot was last updated, if applicable.
    /// </summary>
    DateTimeOffset? UpdatedTimestampUtc { get; }
}

/// <summary>
/// Represents a snapshot of state information with a strongly-typed value at a given point in time during command operation processing.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IState<T> : IState
{
    /// <summary>
    /// Gets the value stored in the current instance.
    /// </summary>
    T Value { get; }

    /// <summary>
    /// Updates the current instance with the specified value and timestamp.
    /// </summary>
    /// <remarks>This method may trigger any necessary state changes or notifications based on the updated
    /// value.</remarks>
    /// <param name="value">The new value to assign to the instance. This value must not be null.</param>
    /// <param name="updatedTimestampUtc">The timestamp, in Coordinated Universal Time (UTC), indicating when the update occurred.</param>
    void Update(T value, DateTimeOffset updatedTimestampUtc);
}

