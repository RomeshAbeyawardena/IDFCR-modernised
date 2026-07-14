namespace IDFCR.Utilities;

/// <summary>
/// Defines a static class for creating instances of <see cref="ISetOnce{T}"/> that can be set only once. Once the value is set, it cannot be changed.
/// </summary>
public static class SetOnce
{
    /// <summary>
    /// Creates an instance of <see cref="ISetOnce{T}"/> that can be set only once. Once the value is set, it cannot be changed.
    /// </summary>
    /// <typeparam name="T">The type of the value that can be set only once.</typeparam>
    /// <returns>An instance of <see cref="ISetOnce{T}"/>.</returns>
    public static ISetOnce<T> CreateInstance<T>()
    {
        return new SetOnce<T>();
    }

    /// <summary>
    /// Creates an instance of <see cref="ISetOnce{T}"/> that can be set only once and sets its value. Once the value is set, it cannot be changed.
    /// </summary>
    /// <typeparam name="T">The type of the value that can be set only once.</typeparam>
    /// <param name="value">The value to set.</param>
    /// <returns>An instance of <see cref="ISetOnce{T}"/> with the specified value set.</returns>
    public static ISetOnce<T> CreateInstance<T>(T value)
    {
        var instance = CreateInstance<T>();

        instance.SetValue(value);
        return instance;
    }
}

internal class SetOnce<T> : ISetOnce<T>
{
    private readonly Lock _lock = new();
    private T? _value;
    private T? GetValue()
    {
        lock (_lock)
        {
            return _value;
        }
    }

    public T? Value => GetValue();
    public bool IsSet { get; private set; }
    object? ISetOnce.Value => Value;

    public void SetValue(T? value)
    {
        if (value is null)
        {
            return;
        }

        lock (_lock)
        {
            if (IsSet)
            {
                throw new InvalidOperationException("Cannot set the value more than once.");
            }

            _value = value;
            IsSet = true;
        }
    }

    void ISetOnce.SetValue(object? value)
    {
        if (value is T val)
        {
            SetValue(val);
        }
    }
}