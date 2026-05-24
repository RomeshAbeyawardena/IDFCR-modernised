using IDFCR.Abstractions.Cryptography;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace IDFCR.Cryptography.Tests;

internal interface IClientSecurity : IMapper<IClientSecurity>, IIdentifiable, IAuditCreatedTimestamp, IPasswordDerivedKeyGeneratorOptions
{
    object? ClientId { get; }
    string Secret { get; }
    string? Version { get; }
    DateTimeOffset? ValidUntilTimestampUtc { get; }
}
