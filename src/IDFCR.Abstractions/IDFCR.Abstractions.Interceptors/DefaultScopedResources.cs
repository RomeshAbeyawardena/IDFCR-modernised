using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Interceptors;

internal class DefaultScopedResources : IScopedResources
{
    private readonly ConcurrentDictionary<Type, object?> _sharedContextObjects = [];
    public IReadOnlyDictionary<Type, object?> Items => _sharedContextObjects.AsReadOnly();

    public void AddOrUpdate<T>(T value)
    {
        _sharedContextObjects.AddOrUpdate(typeof(T), value, (_,_) =>
        {
            return value;
        });
    }

    public bool Contains<T>()
    {
        return _sharedContextObjects.ContainsKey(typeof(T));
    }

    public T? GetScopedResource<T>()
    {
        if (TryGetScopedResource<T>(out var value))
        {
            return value;
        }

        return default;
    }

    public bool TryGetScopedResource<T>([NotNullWhen(true)] out T? value)
    {
        if (_sharedContextObjects.TryGetValue(typeof(T), out var _value)
            && _value is T val)
        {
            value = val;
            return true;
        }

        value = default;
        return false;
    }
}
