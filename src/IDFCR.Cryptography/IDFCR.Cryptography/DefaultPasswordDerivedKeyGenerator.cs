using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace IDFCR.Cryptography;

internal sealed class DefaultPasswordDerivedKeyGenerator : IPasswordDerivedKeyGenerator
{
    public string ComputeCheckSum(string constant, byte[] data)
    {
        var encoding = Encoding.UTF8;
        return Convert.ToBase64String(
                HMACSHA384.HashData(
                encoding.GetBytes(constant), data));
    }

    public string ComputeCheckSum(string constant, string data)
    {
        var encoding = Encoding.UTF8;
        return ComputeCheckSum(constant, encoding.GetBytes(data));
    }

    public string ComputeSecret(string secret, string constant, out string checkSum, int? iterations, int defaultKeyLength, int defaultNonceLength)
    {
        byte[] salt = new byte[defaultNonceLength];

        RandomNumberGenerator.Fill(salt);

        Debug.WriteLine(Convert.ToBase64String(salt), nameof(ComputeSecret));

        var clientSecret = Rfc2898DeriveBytes.Pbkdf2(secret, salt, iterations.GetValueOrDefault(10000), HashAlgorithmName.SHA384, defaultKeyLength);

        List<byte> combined = [.. clientSecret];
        combined.AddRange(salt);

        var hashedSecret = Convert.ToBase64String(combined.ToArray());

        var encoding = Encoding.UTF8;
        checkSum = ComputeCheckSum(constant, [.. combined]);

        return hashedSecret;
    }

    public string ComputeSecret(string secret, string constant, int? iterations,
        IPasswordDerivedKeyGeneratorOptions options, out string checkSum)
    {
        return ComputeSecret(secret, constant, out checkSum, iterations,
            options.KeyLength.GetValueOrDefault(32), options.NonceLength.GetValueOrDefault(24));
    }


    public bool VerifyChecksum(string checkSum, string storedHash, string constant)
    {
        var encoding = Encoding.UTF8;
        var expectedChecksum = Convert.ToBase64String(
            HMACSHA384.HashData(
                encoding.GetBytes(constant),
                Convert.FromBase64String(storedHash)));

        var expectedBytes = encoding.GetBytes(expectedChecksum);
        var actualBytes = encoding.GetBytes(checkSum);

        return CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
    }

    public bool ValidateSecret(string storedHash, string secret, string constant,
        int? iterations, int defaultKeyLength, int defaultNonceLength,
        string? checkSum)
    {
        if (!string.IsNullOrWhiteSpace(checkSum)
            && !VerifyChecksum(checkSum, storedHash, constant))
        {
            return false;
        }

        var secretBytes = Convert.FromBase64String(storedHash);
        var secretBytesMemory = secretBytes.AsMemory();

        var extractedSecret = secretBytesMemory[..defaultKeyLength].ToArray();
        var salt = secretBytesMemory.Slice(defaultKeyLength, defaultNonceLength).ToArray();

        var clientSecret = Rfc2898DeriveBytes.Pbkdf2(secret, salt, iterations.GetValueOrDefault(10000), HashAlgorithmName.SHA384, defaultKeyLength);

        return CryptographicOperations.FixedTimeEquals(clientSecret, extractedSecret);
    }

    public bool ValidateSecret(string storedHash, string secret, string constant,
        int? iterations, IPasswordDerivedKeyGeneratorOptions options, string? checkSum)
    {
        return ValidateSecret(storedHash, secret, constant, iterations,
            options.KeyLength.GetValueOrDefault(32), options.NonceLength.GetValueOrDefault(24), checkSum);
    }
}