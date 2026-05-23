namespace IDFCR.Cryptography;

/// <summary>
/// Represents a service that can generate a derived key from a password and a constant value, and can also validate the derived key against a stored hash. This is typically used for securely storing and validating passwords or other secrets by deriving a key from the secret and comparing it to a stored hash. The interface also includes methods for computing checksums to ensure data integrity.
/// </summary>
public interface IPasswordDerivedKeyGenerator
{
    /// <summary>
    /// Computes a checksum for the given data using the specified constant. The checksum can be used to verify the integrity of the data when validating secrets or comparing against stored hashes. The method may use a hashing algorithm to generate the checksum based on the constant and the data provided.
    /// </summary>
    /// <param name="constant">A constant value used in the checksum computation.</param>
    /// <param name="data">The data for which the checksum is to be computed.</param>
    /// <returns>The computed checksum as a string.</returns>
    string ComputeCheckSum(string constant, byte[] data);
    /// <summary>
    /// Computes a checksum for the given data using the specified constant. This overload accepts the data as a string instead of a byte array. The method may internally convert the string to bytes before computing the checksum, or it may use a different approach to generate the checksum based on the string input. The resulting checksum can be used for integrity verification when validating secrets or comparing against stored hashes.
    /// </summary>
    /// <param name="constant">A constant value used in the checksum computation.</param>
    /// <param name="data">The data for which the checksum is to be computed.</param>
    /// <returns>The computed checksum as a string.</returns>
    string ComputeCheckSum(string constant, string data);
    /// <summary>
    /// Computes a derived key from the given secret and constant, and also outputs a checksum for the derived key. The method may use a key derivation function (KDF) such as PBKDF2, bcrypt, or scrypt to generate the derived key based on the secret and constant. The number of iterations, key length, and nonce length can be specified to enhance security. The resulting derived key can be used for securely storing or validating secrets, while the checksum can be used to verify the integrity of the derived key when validating against stored hashes.
    /// </summary>
    /// <param name="secret">The secret from which to derive the key.</param>
    /// <param name="constant">A constant value used in the key derivation process.</param>
    /// <param name="checkSum">The output checksum for the derived key.</param>
    /// <param name="iterations">The number of iterations to use in the key derivation function.</param>
    /// <param name="defaultKeyLength">The default length of the derived key.</param>
    /// <param name="defaultNonceLength">The default length of the nonce.</param>
    /// <returns></returns>
    string ComputeSecret(string secret, string constant,
        out string checkSum, int? iterations = null, int defaultKeyLength = 32, int defaultNonceLength = 24);

    /// <summary>
    /// Computes a derived key from the given secret and constant, and also outputs a checksum for the derived key. This overload accepts an options object that can specify various parameters for the key derivation process, such as key length, nonce length, and checksum configuration. The method may use a key derivation function (KDF) to generate the derived key based on the secret and constant, and the resulting derived key can be used for securely storing or validating secrets. The checksum can be used to verify the integrity of the derived key when validating against stored hashes.
    /// </summary>
    /// <param name="secret">The secret from which to derive the key.</param>
    /// <param name="constant">A constant value used in the key derivation process.</param>
    /// <param name="iterations">The number of iterations to use in the key derivation function.</param>
    /// <param name="options">An options object specifying parameters for the key derivation process.</param>
    /// <param name="checkSum">The output checksum for the derived key.</param>
    /// <returns>The derived key as a string.</returns>
    string ComputeSecret(string secret, string constant,
        int? iterations, IPasswordDerivedKeyGeneratorOptions options, out string checkSum);

    /// <summary>
    /// Validates the given secret against a stored hash using the specified constant and optional parameters for iterations, key length, nonce length, and checksum. The method may compute a derived key from the secret and constant using the same parameters as when the stored hash was generated, and then compare the computed derived key against the stored hash to determine if they match. If a checksum is provided, it can also be used to verify the integrity of the data during validation. The method returns true if the secret is valid and matches the stored hash; otherwise, it returns false.
    /// </summary>
    /// <param name="storedHash">The stored hash against which to validate the secret.</param>
    /// <param name="secret">The secret to validate.</param>
    /// <param name="constant">A constant value used in the key derivation process.</param>
    /// <param name="iterations">The number of iterations to use in the key derivation function.</param>
    /// <param name="defaultKeyLength">The default length of the derived key.</param>
    /// <param name="defaultNonceLength">The default length of the nonce.</param>
    /// <param name="checkSum">The checksum to verify the integrity of the data.</param>
    /// <returns>True if the secret is valid and matches the stored hash; otherwise, false.</returns>
    bool ValidateSecret(string storedHash, string secret, string constant,
        int? iterations = null, int defaultKeyLength = 32, int defaultNonceLength = 24,
        string? checkSum = null);

    /// <summary>
    /// Validates the given secret against a stored hash using the specified constant and an options object that can specify various parameters for the validation process, such as iterations, key length, nonce length, and checksum configuration. The method may compute a derived key from the secret and constant using the parameters specified in the options object, and then compare the computed derived key against the stored hash to determine if they match. If a checksum is provided, it can also be used to verify the integrity of the data during validation. The method returns true if the secret is valid and matches the stored hash; otherwise, it returns false.
    /// </summary>
    /// <param name="storedHash">The stored hash against which to validate the secret.</param>
    /// <param name="secret">The secret to validate.</param>
    /// <param name="constant">A constant value used in the key derivation process.</param>
    /// <param name="iterations">The number of iterations to use in the key derivation function.</param>
    /// <param name="options">An options object specifying parameters for the validation process.</param>
    /// <param name="checkSum">The checksum to verify the integrity of the data.</param>
    /// <returns>True if the secret is valid and matches the stored hash; otherwise, false.</returns>
    bool ValidateSecret(string storedHash, string secret, string constant,
        int? iterations, IPasswordDerivedKeyGeneratorOptions options,
        string? checkSum = null);

    /// <summary>
    /// Verifies the integrity of the data by comparing the provided checksum with a stored hash using the specified constant. This method can be used to ensure that the data has not been tampered with or corrupted by validating that the computed checksum matches the expected value derived from the stored hash and constant. The method returns true if the checksum is valid and matches the expected value; otherwise, it returns false.
    /// </summary>
    /// <param name="checkSum">The checksum to verify the integrity of the data.</param>
    /// <param name="storedHash">The stored hash against which to validate the checksum.</param>
    /// <param name="constant">A constant value used in the key derivation process.</param>
    /// <returns>True if the checksum is valid and matches the expected value; otherwise, false.</returns>
    bool VerifyChecksum(string checkSum, string storedHash, string constant);
}