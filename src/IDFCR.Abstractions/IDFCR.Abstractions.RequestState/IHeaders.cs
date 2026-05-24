namespace IDFCR.Abstractions.RequestState;

/// <summary>
/// Represents the headers from an authenticated HTTP request, allowing access to additional information sent by the client, such as authentication tokens or custom headers. This interface extends IStateDictionary, providing a dictionary-like structure for storing header key-value pairs, where keys are strings and values can be of any type. The TryGetValue method allows for convenient retrieval of header values in a nullable string format, accommodating cases where headers may not be present or may have null values.
/// </summary>
public interface IHeaders : IStateDictionary
{

}

internal class DefaultHeaders(IDictionary<string, object?> values, Func<object, string> getStringValue) : StateDictionaryBase(values, getStringValue), IHeaders;