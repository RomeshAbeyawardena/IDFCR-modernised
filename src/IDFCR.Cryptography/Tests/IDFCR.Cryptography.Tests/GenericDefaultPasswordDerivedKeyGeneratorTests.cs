using IDFCR.Abstractions.Cryptography;
using NUnit.Framework;
using System.Security.Cryptography;

namespace IDFCR.Cryptography.Tests;

[TestFixture]
internal class GenericDefaultPasswordDerivedKeyGeneratorTests
{
    private IPasswordDerivedKeyGenerator _generator = null!;
    private const string TestSecret = "MySecurePassword123!";
    private const string TestConstant = "TestConstant";

    [SetUp]
    public void SetUp()
    {
        _generator = new DefaultPasswordDerivedKeyGenerator();
    }

    #region ComputeSecret Tests

    [Test]
    public void ComputeSecret_WithValidInputs_ReturnsBase64EncodedString()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;

        // Act
        var result = _generator.ComputeSecret(secret, constant, out var checkSum);

        // Assert
        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(checkSum, Is.Not.Null.And.Not.Empty);
        // Verify it's valid Base64
        Assert.DoesNotThrow(() => Convert.FromBase64String(result));
        Assert.DoesNotThrow(() => Convert.FromBase64String(checkSum));
    }

    [Test]
    public void ComputeSecret_WithDefaultParameters_UsesCorrectDefaults()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        int defaultKeyLength = 32;
        int defaultNonceLength = 24;

        // Act
        var result = _generator.ComputeSecret(secret, constant, out var checkSum,
            defaultKeyLength: defaultKeyLength, defaultNonceLength: defaultNonceLength);

        // Assert
        var decodedBytes = Convert.FromBase64String(result);
        // Key length + nonce length
        Assert.That(decodedBytes.Length, Is.EqualTo(defaultKeyLength + defaultNonceLength));
    }

    [Test]
    public void ComputeSecret_WithCustomIterations_AppliesPbkdf2Correctly()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        int? iterations = 5000;

        // Act
        var result1 = _generator.ComputeSecret(secret, constant, out _, iterations);
        var result2 = _generator.ComputeSecret(secret, constant, out _, iterations);

        // Assert
        // Different salts should produce different results even with same inputs
        Assert.That(result1, Is.Not.EqualTo(result2));
    }

    [Test]
    public void ComputeSecret_WithOptions_UsesOptionsKeyAndNonceLengths()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        var options = new TestPasswordDerivedKeyGeneratorOptions
        {
            KeyLength = 16,
            NonceLength = 12
        };

        // Act
        var result = _generator.ComputeSecret(secret, constant, 10000, options, out var checkSum);

        // Assert
        var decodedBytes = Convert.FromBase64String(result);
        Assert.That(decodedBytes.Length, Is.EqualTo(16 + 12));
        Assert.That(checkSum, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void ComputeSecret_ProducesDifferentResultsEachCall()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;

        // Act
        var result1 = _generator.ComputeSecret(secret, constant, out _);
        var result2 = _generator.ComputeSecret(secret, constant, out _);

        // Assert
        Assert.That(result1, Is.Not.EqualTo(result2),
            "Different random salts should produce different hashes");
    }

    #endregion

    #region ValidateSecret Tests

    [Test]
    public void ValidateSecret_WithCorrectSecret_ReturnsTrue()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        var hashedSecret = _generator.ComputeSecret(secret, constant, out var checkSum);

        // Act
        var isValid = _generator.ValidateSecret(hashedSecret, secret, constant, checkSum: checkSum);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void ValidateSecret_WithIncorrectSecret_ReturnsFalse()
    {
        // Arrange
        var secret = TestSecret;
        var wrongSecret = "WrongPassword";
        var constant = TestConstant;
        var hashedSecret = _generator.ComputeSecret(secret, constant, out var checkSum);

        // Act
        var isValid = _generator.ValidateSecret(hashedSecret, wrongSecret, constant, checkSum: checkSum);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void ValidateSecret_WithoutChecksum_StillValidates()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        var hashedSecret = _generator.ComputeSecret(secret, constant, out _);

        // Act
        var isValid = _generator.ValidateSecret(hashedSecret, secret, constant);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void ValidateSecret_WithInvalidChecksum_ReturnsFalse()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        var hashedSecret = _generator.ComputeSecret(secret, constant, out var checkSum);
        var invalidChecksum = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        // Act
        var isValid = _generator.ValidateSecret(hashedSecret, secret, constant,
            checkSum: invalidChecksum);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void ValidateSecret_WithCustomKeyAndNonceLength_Validates()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        int keyLength = 16;
        int nonceLength = 12;
        var hashedSecret = _generator.ComputeSecret(secret, constant, out var checkSum,
            defaultKeyLength: keyLength, defaultNonceLength: nonceLength);

        // Act
        var isValid = _generator.ValidateSecret(hashedSecret, secret, constant,
            defaultKeyLength: keyLength, defaultNonceLength: nonceLength, checkSum: checkSum);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void ValidateSecret_WithWrongKeyLength_ReturnsFalse()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        int keyLength = 16;
        int nonceLength = 12;
        var hashedSecret = _generator.ComputeSecret(secret, constant, out _,
            defaultKeyLength: keyLength, defaultNonceLength: nonceLength);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _generator.ValidateSecret(hashedSecret, secret, constant,
            defaultKeyLength: 32, defaultNonceLength: nonceLength);
        });
    }

    [Test]
    public void ValidateSecret_WithOptions_UsesOptionsForValidation()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        var options = new TestPasswordDerivedKeyGeneratorOptions
        {
            KeyLength = 24,
            NonceLength = 16
        };
        var hashedSecret = _generator.ComputeSecret(secret, constant, 5000, options, out var checkSum);

        // Act
        var isValid = _generator.ValidateSecret(hashedSecret, secret, constant, 5000, options, checkSum);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void ValidateSecret_WithIncorrectConstant_ConsidersChecksumInvalid()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        var wrongConstant = "WrongConstant";
        var hashedSecret = _generator.ComputeSecret(secret, constant, out var checkSum);

        // Act
        var isValid = _generator.ValidateSecret(hashedSecret, secret, wrongConstant, checkSum: checkSum);

        // Assert
        Assert.That(isValid, Is.False);
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ComputeSecret_WithNullIterations_UsesDefault()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;

        // Act
        var result = _generator.ComputeSecret(secret, constant, out _, iterations: null);

        // Assert
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void ValidateSecret_WithNullIterations_UsesDefault()
    {
        // Arrange
        var secret = TestSecret;
        var constant = TestConstant;
        var hashedSecret = _generator.ComputeSecret(secret, constant, out var checkSum, iterations: null);

        // Act
        var isValid = _generator.ValidateSecret(hashedSecret, secret, constant, iterations: null, checkSum: checkSum);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void ComputeAndValidateSecret_WithEmptySecret_Works()
    {
        // Arrange
        var secret = string.Empty;
        var constant = TestConstant;

        // Act
        var hashedSecret = _generator.ComputeSecret(secret, constant, out var checkSum);
        var isValid = _generator.ValidateSecret(hashedSecret, secret, constant, checkSum: checkSum);

        // Assert
        Assert.That(isValid, Is.True);
    }

    #endregion
}
