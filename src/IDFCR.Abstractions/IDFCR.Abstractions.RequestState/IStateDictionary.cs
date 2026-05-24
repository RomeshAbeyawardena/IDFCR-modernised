namespace IDFCR.Abstractions.RequestState;

/// <summary>
/// Represents a dictionary-like structure for storing key-value pairs, where keys are strings and values can be of any type. This interface is used to represent both claims and headers in the context of an authenticated HTTP request, allowing for flexible access to authentication-related data.
/// </summary>
public interface IStateDictionary : IReadOnlyDictionary<string, object?>
{
    /// <summary>
    /// Attempts to retrieve the value associated with the specified key. Returns true if the key exists and the value is successfully retrieved; otherwise, returns false. The value is returned as a nullable string, allowing for cases where the value may not be present or is null.
    /// </summary>
    /// <param name="key">The key whose value to retrieve.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, null.</param>
    /// <returns>true if the key exists and the value is successfully retrieved; otherwise, false.</returns>
    bool TryGetValue(string key, out string? value);
}
