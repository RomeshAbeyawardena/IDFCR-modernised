using System.Collections.ObjectModel;

namespace IDFCR.Abstractions.RequestState;

/// <summary>
/// Represents a base implementation of the IStateDictionary interface, providing a read-only dictionary structure for storing key-value pairs. This class serves as a foundation for more specific implementations, such as authenticated claims and headers, allowing for consistent access to authentication-related data in a dictionary-like format. The TryGetValue method is implemented to retrieve values as nullable strings, utilizing a provided function to convert object values to their string representations when necessary.
/// </summary>
/// <param name="values">The dictionary of key-value pairs to be stored in the read-only dictionary.</param>
/// <param name="getStringValue">A function to convert object values to their string representations.</param>
public abstract class StateDictionaryBase(IDictionary<string, object?> values, Func<object, string> getStringValue)
    : ReadOnlyDictionary<string, object?>(values), IStateDictionary
{
    /// <summary>
    /// Attempts to retrieve the value associated with the specified key. Returns true if the key exists and the value is successfully retrieved; otherwise, returns false. The value is returned as a nullable string, allowing for cases where the value may not be present or is null. The method utilizes the provided function to convert object values to their string representations when necessary.
    /// </summary>
    /// <param name="key">The key whose value to retrieve.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, null.</param>
    /// <returns>true if the key exists and the value is successfully retrieved; otherwise, false.</returns>
    public bool TryGetValue(string key, out string? value)
    {
        value = null;

        if (base.TryGetValue(key, out var val) && val is not null)
        {
            value = getStringValue?.Invoke(val);
            return true;
        }

        return false;
    }
}