using System.Collections.Concurrent;

namespace IDFCR.Abstractions.Builders;

/// <summary>
/// Defines a static class that provides a method for creating an instance of a dictionary builder using a fluent API. The Create method allows developers to easily construct a dictionary by providing an action that configures the builder, enabling the addition or updating of key-value pairs in a flexible and readable manner. This class serves as a convenient entry point for utilizing the dictionary builder functionality, allowing developers to build dictionaries in a more intuitive and efficient way within their applications and systems.
/// </summary>
public static class DictionaryBuilder
{
    /// <summary>
    /// Creates an instance of a dictionary builder and applies the provided configuration action to it. This method initializes a new instance of the <see cref="DictionaryBuilder{TKey, TValue}"/> class and then invokes the specified builder action, allowing developers to configure the builder by adding or updating key-value pairs. After the builder action is executed, the method returns the configured dictionary builder instance, which can then be used to build the final dictionary containing all the accumulated entries. This approach provides a fluent API for constructing dictionaries in a more readable and efficient manner within applications and systems that require dynamic dictionary construction.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="builderAction">An action that configures the dictionary builder.</param>
    /// <returns>The configured dictionary builder instance.</returns>
    public static IDictionaryBuilder<TKey, TValue> Create<TKey, TValue>(Action<IDictionaryBuilder<TKey, TValue>> builderAction)
        where TKey : notnull
    {
        var builder = new DictionaryBuilder<TKey, TValue>();
        builderAction(builder);
        return builder;
    }
}

/// <summary>
/// Represents a builder for constructing a dictionary with fluent API. It allows adding or updating key-value pairs and building the final dictionary.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
public sealed class DictionaryBuilder<TKey, TValue> : IDictionaryBuilder<TKey, TValue>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> _dictionary = [];
    /// <summary>
    /// Defines a method to add or update a key-value pair in the dictionary. If the key already exists, its value will be updated with the new value provided.
    /// </summary>
    /// <param name="key">The key of the element to add or update.</param>
    /// <param name="value">The value to be added or used to update the existing value.</param>
    /// <returns>The current instance of the dictionary builder.</returns>
    public IDictionaryBuilder<TKey, TValue> AddOrUpdate(TKey key, TValue value)
    {
        _dictionary.AddOrUpdate(key, value, (k, v) => value);
        return this;
    }

    /// <summary>
    /// Defines a method to add or update a key-value pair in the dictionary. If the key already exists, its value will be updated using the provided update function, which takes the existing value as input and returns the new value.
    /// </summary>
    /// <param name="key">The key of the element to add or update.</param>
    /// <param name="value">The value to be added or used to update the existing value.</param>
    /// <param name="updateFunc">The function to update the value if the key already exists.</param>
    /// <returns>The current instance of the dictionary builder.</returns>
    public IDictionaryBuilder<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue> updateFunc)
    {
        _dictionary.AddOrUpdate(key, value, (k, v) => updateFunc(v));
        return this;
    }

    /// <summary>
    /// Defines a method to build and retrieve the final dictionary containing all the key-value pairs that have been added or updated using the builder. The resulting dictionary is a standard <see cref="Dictionary{TKey, TValue}"/>  containing the accumulated entries.
    /// </summary>
    /// <returns>The final dictionary containing all the key-value pairs.</returns>
    public Dictionary<TKey, TValue> Build()
    {
        return _dictionary.ToDictionary();
    }
}
