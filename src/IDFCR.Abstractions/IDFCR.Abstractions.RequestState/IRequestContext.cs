namespace IDFCR.Abstractions.RequestState;

/// <summary>
/// Represents a context for an HTTP request, providing access to headers and other relevant information about the request. This interface serves as a base for more specific contexts, such as authenticated contexts, allowing for a consistent way to access request data across different implementations.
/// </summary>
public interface IRequestContext
{
    /// <summary>
    /// Gets the headers from the authenticated HTTP request, allowing access to additional information sent by the client, such as authentication tokens or custom headers.
    /// </summary>
    IHeaders Headers { get; }
}

internal class DefaultRequestContext(IDictionary<string, object?> values, Func<object, string> getStringValue) : IRequestContext
{
    public IHeaders Headers { get; } = new DefaultHeaders(values, getStringValue);
}