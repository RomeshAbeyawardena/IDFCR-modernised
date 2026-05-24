using IDFCR.Abstractions.Cryptography;

namespace IDFCR.Cryptography.Tests;

/// <summary>
/// Test implementation of IPasswordDerivedKeyGeneratorOptions for testing purposes
/// </summary>
internal class TestPasswordDerivedKeyGeneratorOptions : IPasswordDerivedKeyGeneratorOptions
{
    public string Checksum { get; set; } = string.Empty;
    public int? KeyLength { get; set; }
    public int? NonceLength { get; set; }
}
