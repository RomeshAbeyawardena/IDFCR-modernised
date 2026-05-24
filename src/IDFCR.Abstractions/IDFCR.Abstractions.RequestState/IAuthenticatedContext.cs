using IDFCR.Abstractions.RequestState;

namespace IDFCR.Abstractions.RequestState;

/// <summary>
/// Represents the context of an authenticated HTTP request, providing access to authentication status, claims, and headers.
/// </summary>
public interface IAuthenticatedContext : IRequestContext
{
    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
    /// <summary>
    /// Gets the claims associated with the authenticated user, providing access to user information and permissions.
    /// </summary>
    IAuthenticatedClaims Claims { get; }
}

internal class DefaultAuthenticatedContext(bool isAuthenticated,
    IDictionary<string, object?> headerValues, 
    Func<object, string> getHeaderStringValue,
    IDictionary<string, object?> claimValues,
    Func<object, string> getClaimStringValue) 
    : DefaultRequestContext(headerValues, getHeaderStringValue), IAuthenticatedContext
{
    public bool IsAuthenticated { get; } = isAuthenticated;
    public IAuthenticatedClaims Claims { get; } = new DefaultAuthenticatedClaims(claimValues, getClaimStringValue);
}