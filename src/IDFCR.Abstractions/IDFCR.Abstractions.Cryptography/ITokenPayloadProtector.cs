namespace IDFCR.Abstractions.Cryptography;

/// <summary>
/// Represents a service that can protect and unprotect token payloads using a signing key. This is typically used to ensure the integrity and confidentiality of token data when generating and validating tokens.
/// </summary>
public interface ITokenPayloadProtector
{
    /// <summary>
    /// Protects the given value using the specified signing key. The protected value can be safely transmitted or stored, and can later be unprotected using the same signing key to retrieve the original value.
    /// </summary>
    /// <param name="value">The value to be protected.</param>
    /// <param name="signingKey">The key used to sign and protect the value.</param>
    /// <returns>The protected value.</returns>
    string Protect(string value, string signingKey);
    /// <summary>
    /// Unprotects the given protected value using the specified signing key. If the protected value is valid and was protected using the same signing key, this method will return the original value. If the protected value is invalid or was not protected with the same signing key, an exception may be thrown or an error may occur.
    /// </summary>
    /// <param name="protectedValue">The value to be unprotected.</param>
    /// <param name="signingKey">The key used to unprotect the value.</param>
    /// <returns>The original value if the protected value is valid; otherwise, an error may occur.</returns>
    string Unprotect(string protectedValue, string signingKey);
}
