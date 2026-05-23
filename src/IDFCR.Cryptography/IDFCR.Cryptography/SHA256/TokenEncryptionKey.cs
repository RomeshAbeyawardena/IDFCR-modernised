using SysCryptography = System.Security.Cryptography;
using System.Text;

namespace IDFCR.Cryptography.SHA256;

/// <summary>
/// Represents a utility class for deriving a token encryption key from a signing key. This is typically used to generate a consistent encryption key that can be used for protecting token payloads, based on a given signing key. The derived encryption key is generated using a hash function (SHA256) to ensure it has the appropriate length and randomness for cryptographic operations.
/// </summary>
public static class TokenEncryptionKey
{
    /// <summary>
    /// Derives a token encryption key from the given signing key using the SHA256 hash function. The resulting encryption key is a byte array that can be used for cryptographic operations, such as encrypting and decrypting token payloads. This method ensures that the derived encryption key is consistent for the same signing key, allowing for reliable protection and unprotection of token data.
    /// </summary>
    /// <param name="signingKey">The signing key from which to derive the encryption key.</param>
    /// <returns>A byte array representing the derived encryption key.</returns>
    public static byte[] Derive(string signingKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(signingKey);

        return SysCryptography.SHA256.HashData(Encoding.UTF8.GetBytes(signingKey));
    }
}
