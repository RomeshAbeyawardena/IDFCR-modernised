using NUnit.Framework;
using System.Security.Cryptography;
using System.Text;

namespace IDFCR.Cryptography.Tests;

[TestFixture]
internal class DefaultPasswordDerivedKeyGeneratorTests
{
    private DefaultPasswordDerivedKeyGenerator _generator;

    [SetUp]
    public void SetUp()
    {
        _generator = new();
    }

    [Test]
    public void Compute_Should_CreateASecret_That_Validate_ReturnsTrue()
    {
        const string magicSecret = "my-super-magic-secret";
        Guid id = Guid.NewGuid();
        ClientSecurity clientSecurity = new()
        {
            ClientId = id,
            Secret = magicSecret
        };

        clientSecurity.Compute(_generator);
        Assert.That(clientSecurity.Validate(_generator, magicSecret), Is.True);
    }

    [Test]
    public void Compute_Should_SetChecksum_From_ClientId_And_ComputedSecret()
    {
        // Arrange
        var sut = new ClientSecurity
        {
            ClientId = "client-1",
            Secret = "plain-text-secret"
        };

        // Act
        sut.Compute(_generator);

        // Assert
        var expectedChecksum = Convert.ToBase64String(
            HMACSHA384.HashData(
                Encoding.UTF8.GetBytes($"{sut.Version}-{sut.ClientId}"),
                Convert.FromBase64String(sut.Secret)));

        Assert.That(sut.Checksum, Is.EqualTo(expectedChecksum));
    }

    [Test]
    public void Compute_Should_HashSecret_And_CreateMatchingChecksum()
    {
        // Arrange
        const string originalSecret = "plain-text-secret";

        var sut = new ClientSecurity
        {
            ClientId = "client-1",
            Secret = originalSecret
        };

        // Act
        sut.Compute(_generator);

        // Assert
        Assert.That(sut.Secret, Is.Not.EqualTo(originalSecret));

        var expectedChecksum = Convert.ToBase64String(
            HMACSHA384.HashData(
                Encoding.UTF8.GetBytes($"{sut.Version}-{sut.ClientId}"),
                Convert.FromBase64String(sut.Secret)));

        Assert.That(sut.Checksum, Is.EqualTo(expectedChecksum));
    }

    [Test]
    public void SetVersion_Should_GenerateUniqueBase64String()
    {
        var sut1 = new ClientSecurity();
        var sut2 = new ClientSecurity();

        sut1.SetVersion();
        sut2.SetVersion();

        Assert.That(sut1.Version, Is.Not.Null);
        Assert.That(sut1.Version, Is.Not.EqualTo(sut2.Version));
        // Check that it's a valid Base64 string (optional but thorough)
        Assert.DoesNotThrow(() => Convert.FromBase64String(sut1.Version!));
    }
}