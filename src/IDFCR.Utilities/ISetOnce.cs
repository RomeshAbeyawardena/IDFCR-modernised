namespace IDFCR.Utilities;

/// <summary>
/// Represents a value that can be set only once. Once the value is set, it cannot be changed.
/// </summary>
public interface ISetOnce
{
    /// <summary>
    /// Gets a value indicating whether the value has been set. If the value has not been set, it returns false; otherwise, it returns true.
    /// </summary>
    bool IsSet { get; }
    /// <summary>
    /// Gets the value that can be set only once. If the value has not been set, it returns null.
    /// </summary>
    object? Value { get; }
    /// <summary>
    /// Sets the value. If the value has already been set, it throws an InvalidOperationException.
    /// </summary>
    /// <param name="value"></param>
    void SetValue(object? value);
}

/// <summary>
/// Represents a value that can be set only once. Once the value is set, it cannot be changed.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISetOnce<T> : ISetOnce
{
    /// <summary>
    /// Gets the value that can be set only once. If the value has not been set, it returns null.
    /// </summary>
    new T? Value { get; }
    /// <summary>
    /// Sets the value. If the value has already been set, it throws an InvalidOperationException.
    /// </summary>
    /// <param name="value">The value to set.</param>
    void SetValue(T? value);
}