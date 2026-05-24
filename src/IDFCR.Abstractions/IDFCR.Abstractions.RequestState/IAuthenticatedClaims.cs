namespace IDFCR.Abstractions.RequestState;

/// <summary>
/// Represents the claims associated with an authenticated user, providing access to user information and permissions in a key-value format.
/// </summary>
public interface IAuthenticatedClaims : IStateDictionary
{

}

internal class DefaultAuthenticatedClaims(IDictionary<string, object?> values, Func<object, string> getStringValue)
    : StateDictionaryBase(values, getStringValue), IAuthenticatedClaims;