using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Cli;

/// <summary>
/// Provides a read-only dictionary implementation for accessing parsed command-line argument parameters.
/// </summary>
/// <param name="arguments">The collection of raw command-line argument strings to parse.</param>
/// <remarks>
/// This class wraps a <see cref="ReadOnlyDictionary{TKey, TValue}"/> that contains parsed command-line 
/// parameters. Arguments are parsed using <see cref="ArgumentSplitter"/> to extract parameter keys and values.
/// </remarks>
public class ArgumentParameters(IEnumerable<string> arguments) : IArgumentParameters
{
    private readonly ReadOnlyDictionary<string, Parameter> _dictionary = new(ArgumentSplitter.Split(arguments));

    /// <summary>
    /// Gets the <see cref="Parameter"/> associated with the specified key.
    /// </summary>
    /// <param name="key">The parameter key to retrieve.</param>
    /// <returns>The <see cref="Parameter"/> object associated with the specified key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the specified key is not found in the collection.</exception>
    public Parameter this[string key] => _dictionary[key];

    /// <summary>
    /// Gets a collection containing the parameter keys.
    /// </summary>
    /// <value>An enumerable collection of parameter key strings.</value>
    public IEnumerable<string> Keys => _dictionary.Keys;
    
    /// <summary>
    /// Gets a collection containing the parameter values.
    /// </summary>
    /// <value>An enumerable collection of <see cref="Parameter"/> objects.</value>
    public IEnumerable<Parameter> Values => _dictionary.Values;
    
    /// <summary>
    /// Gets the number of parameters contained in the collection.
    /// </summary>
    /// <value>The total count of parsed parameters.</value>
    public int Count => _dictionary.Count;

    /// <summary>
    /// Determines whether the collection contains a parameter with the specified key.
    /// </summary>
    /// <param name="key">The parameter key to locate.</param>
    /// <returns><see langword="true"/> if the collection contains a parameter with the specified key; otherwise, <see langword="false"/>.</returns>
    public bool ContainsKey(string key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the parameter collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection of key-value pairs.</returns>
    public IEnumerator<KeyValuePair<string, Parameter>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    /// <summary>
    /// Attempts to get the parameter value associated with the specified key.
    /// </summary>
    /// <param name="key">The parameter key to locate.</param>
    /// <param name="value">When this method returns, contains the <see cref="Parameter"/> associated with the specified key, 
    /// if the key is found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the collection contains a parameter with the specified key; otherwise, <see langword="false"/>.</returns>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Parameter value)
    {
        return TryGetValue(key, out value);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the parameter collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
