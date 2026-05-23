namespace IDFCR.Cryptography;

/// <summary>
/// Represents options for generating a key derived from a password. This interface defines properties that can be used to specify parameters for the key derivation process, such as a checksum, key length, and nonce length.
/// </summary>
public interface IPasswordDerivedKeyGeneratorOptions
{
    /// <summary>
    /// Gets a checksum value that can be used to verify the integrity of the derived key. The checksum can be used to ensure that the derived key has not been tampered with or corrupted during storage or transmission.
    /// </summary>
    string Checksum { get; }
    /// <summary>
    /// Gets the length of the derived key in bytes. This property can be used to specify the desired length of the key that will be generated from the password. The actual length of the derived key may depend on the key derivation algorithm being used and the parameters provided.
    /// </summary>
    int? KeyLength { get; }
    /// <summary>
    /// Gets the length of the nonce (number used once) in bytes. A nonce is a random value that can be used to ensure the uniqueness of the derived key and to prevent replay attacks. The nonce length can be specified to ensure that the generated nonce is of an appropriate size for the key derivation algorithm being used.
    /// </summary>
    int? NonceLength { get; }
}
