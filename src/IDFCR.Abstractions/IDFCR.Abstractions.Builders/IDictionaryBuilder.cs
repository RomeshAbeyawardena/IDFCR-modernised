namespace IDFCR.Abstractions.Builders;

/// <summary>
/// Represents a builder for constructing a dictionary of key-value pairs. This interface provides methods for adding or updating entries in the dictionary, allowing developers to build a dictionary incrementally by specifying keys and their corresponding values. The AddOrUpdate method allows for adding new entries or updating existing entries based on the provided key, while the Build method finalizes the construction of the dictionary and returns the resulting <see cref="Dictionary{TKey, TValue}"/> instance. By using this builder interface, developers can create dictionaries in a flexible and efficient manner, ensuring that they can easily manage and manipulate key-value pairs within their applications and systems that require dictionary data structures.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
public interface IDictionaryBuilder<TKey, TValue>
where TKey : notnull
{
    /// <summary>
    /// Adds a new entry to the dictionary or updates an existing entry based on the provided key. If the key does not exist in the dictionary, a new entry will be added with the specified value. If the key already exists, the existing entry will be updated with the new value. The AddOrUpdate method allows for flexible management of key-value pairs in the dictionary, enabling developers to easily add or modify entries as needed during the construction process. By using this method, developers can ensure that their dictionaries are built efficiently and accurately, reflecting the desired key-value relationships within their applications and systems that utilize dictionary data structures.
    /// </summary>
    /// <param name="key">The key of the entry to add or update.</param>
    /// <param name="value">The value to associate with the specified key.</param>
    /// <returns>The current instance of the dictionary builder, allowing for method chaining.</returns>
    IDictionaryBuilder<TKey, TValue> AddOrUpdate(TKey key, TValue value);
    /// <summary>
    /// Adds a new entry to the dictionary or updates an existing entry based on the provided key and an update function. If the key does not exist in the dictionary, a new entry will be added with the specified value. If the key already exists, the existing entry will be updated by applying the provided update function to the current value associated with the key. The AddOrUpdate method with an update function allows for more complex management of key-value pairs in the dictionary, enabling developers to define custom logic for updating existing entries based on their current values during the construction process. By using this method, developers can ensure that their dictionaries are built efficiently and accurately, reflecting the desired key-value relationships and update logic within their applications and systems that utilize dictionary data structures.
    /// </summary>
    /// <param name="key">The key of the entry to add or update.</param>
    /// <param name="value">The value to associate with the specified key.</param>
    /// <param name="updateFunc">A function to update the value if the key already exists.</param>
    /// <returns>The current instance of the dictionary builder, allowing for method chaining.</returns>
    IDictionaryBuilder<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue> updateFunc);
    /// <summary>
    /// Builds the dictionary based on the entries that have been added or updated using the AddOrUpdate methods. This method finalizes the construction of the dictionary and returns a new instance of <see cref="Dictionary{TKey, TValue}"/> containing all the key-value pairs that have been specified during the building process. By calling the Build method, developers can obtain a fully constructed dictionary that reflects all the additions and updates made through the builder, allowing for efficient and accurate management of key-value pairs within their applications and systems that utilize dictionary data structures.
    /// </summary>
    /// <returns>A new instance of <see cref="Dictionary{TKey, TValue}"/> containing all the key-value pairs added or updated during the building process.</returns>
    Dictionary<TKey, TValue> Build();
}
