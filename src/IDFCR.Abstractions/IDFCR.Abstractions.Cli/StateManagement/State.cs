namespace IDFCR.Abstractions.Cli.StateManagement;

/// <summary>
/// Represents a snapshot of state information with a strongly-typed value at a given point in time during command operation processing.
/// </summary>
/// <typeparam name="T">The type of the value stored in the state.</typeparam>
/// <param name="Name">The name of the state snapshot.</param>
/// <param name="OriginalValue">The original value of the state snapshot.</param>
/// <param name="CreatedTimestampUtc">The timestamp when the state snapshot was created.</param>
public record State<T>(string Name, T OriginalValue, DateTimeOffset CreatedTimestampUtc) : IState<T>
{
    /// <summary>
    /// Defines a method to update the state with a new value and an updated timestamp, returning a new instance of the state with the updated information.
    /// </summary>
    /// <param name="value">The new value to assign to the state.</param>
    /// <param name="updatedTimestampUtc">The timestamp, in Coordinated Universal Time (UTC), indicating when the update occurred.</param>
    public void Update(T value, DateTimeOffset updatedTimestampUtc)
    {
        Value = value;
        UpdatedTimestampUtc = updatedTimestampUtc;
    }

    /// <summary>
    /// Gets or sets the value stored in the current instance. This property is initialized with the original value and can be updated using the Update method. The setter is private to ensure that updates are controlled through the Update method, which also manages the associated timestamp.
    /// </summary>
    public T Value { get; private set; } = OriginalValue;
    /// <summary>
    /// Gets or sets the timestamp when the state snapshot was last updated, if applicable. This property is updated whenever the Update method is called to reflect the most recent update time. The setter is private to ensure that updates are controlled through the Update method, which also manages the associated value.
    /// </summary>
    public DateTimeOffset? UpdatedTimestampUtc { get; private set; }
}
