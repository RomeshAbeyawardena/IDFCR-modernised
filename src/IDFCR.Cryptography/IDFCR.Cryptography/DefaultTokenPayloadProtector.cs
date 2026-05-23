using IDFCR.Cryptography.SHA256;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace IDFCR.Cryptography;

/// <summary>
/// Represents the default implementation of the ITokenPayloadProtector interface, which uses AES-GCM encryption to protect and unprotect token payloads. This implementation derives an encryption key from the provided signing key and uses it to encrypt the token payload, ensuring both confidentiality and integrity of the data.
/// </summary>
internal sealed class DefaultTokenPayloadProtector : ITokenPayloadProtector
{
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public string Protect(string value, string signingKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(signingKey);

        var key = DeriveKey(signingKey);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var plaintext = Encoding.UTF8.GetBytes(value);
        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        var protectedPayload = new byte[NonceSize + TagSize + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, protectedPayload, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, protectedPayload, NonceSize, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, protectedPayload, NonceSize + TagSize, ciphertext.Length);

        return Base64UrlEncoder.Encode(protectedPayload);
    }

    public string Unprotect(string protectedValue, string signingKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(protectedValue);
        ArgumentException.ThrowIfNullOrWhiteSpace(signingKey);

        var protectedPayload = Base64UrlEncoder.DecodeBytes(protectedValue);
        if (protectedPayload.Length <= NonceSize + TagSize)
        {
            throw new CryptographicException("Protected token payload is invalid.");
        }

        var key = DeriveKey(signingKey);
        var nonce = protectedPayload[..NonceSize];
        var tag = protectedPayload[NonceSize..(NonceSize + TagSize)];
        var ciphertext = protectedPayload[(NonceSize + TagSize)..];
        var plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }

    private static byte[] DeriveKey(string signingKey) => TokenEncryptionKey.Derive(signingKey);
}
