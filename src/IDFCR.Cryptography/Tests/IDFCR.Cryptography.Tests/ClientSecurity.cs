using IDFCR.Abstractions.Cryptography;
using IDFCR.Abstractions.Mapper;
using System.Security.Cryptography;

namespace IDFCR.Cryptography.Tests;

internal class ClientSecurity : MapperBase<IClientSecurity>, IClientSecurity
{
    public object? ClientId { get; set; }
    public string Secret { get; set; } = null!;
    public string Checksum { get; set; } = null!;
    public int? KeyLength { get; set; }
    public int? NonceLength { get; set; }
    public string? Version { get; set; }
    public DateTimeOffset? ValidUntilTimestampUtc { get; set; }
    public object? Id { get; set; }
    public DateTimeOffset CreatedTimestampUtc { get; set; }

    public void SetVersion(int versionLength = 64)
    {
        byte[] salt = new byte[versionLength];
        RandomNumberGenerator.Fill(salt);
        Version = Convert.ToBase64String(salt);
    }

    public bool Validate(IPasswordDerivedKeyGenerator generator, string secret, int? iterations = null)
    {
        if (ClientId is null || !KeyLength.HasValue || !NonceLength.HasValue || Secret.Length < (KeyLength + NonceLength))
        {
            throw new InvalidOperationException("Unable to extract key and nonce");
        }

        return generator.ValidateSecret(Secret, secret, $"{Version}-{ClientId}", iterations, this, Checksum);
    }

    public void Compute(IPasswordDerivedKeyGenerator generator, int? iterations = null)
    {
        if (ClientId is not null)
        {
            KeyLength ??= 32;
            NonceLength ??= 24;

            if (string.IsNullOrWhiteSpace(Version))
            {
                SetVersion();
            }

            Secret = generator.ComputeSecret(Secret,
            $"{Version}-{ClientId}",
            iterations,
            this, out var checkSum);

            Checksum = checkSum;
        }
    }

    protected override void MapMembers(IClientSecurity source)
    {
        ClientId = source.ClientId;
        Secret = source.Secret;
        Checksum = source.Checksum;
        KeyLength = source.KeyLength;
        NonceLength = source.NonceLength;
        Version = source.Version;
        ValidUntilTimestampUtc = source.ValidUntilTimestampUtc;
        Id = source.Id;
        CreatedTimestampUtc = source.CreatedTimestampUtc;
    }
}
