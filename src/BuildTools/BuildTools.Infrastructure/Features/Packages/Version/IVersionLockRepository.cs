using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Packages.Version;

public interface IVersionLockRepository : IRepository<VersionLock, Guid>
{
    Task<IUnitResult<VersionLock>> GetVersionLockAsync(object packageId,
        string versionPrefix,
        CancellationToken cancellationToken);

    Task<IUnitResult<object>> SetVersionLockAsync(object packageId,
        string versionPrefix,
        string? reference = null,
        DateTimeOffset? lockedFromTimestampUtc = null,
        DateTimeOffset? lockedUntilTimestampUtc = null,
        DateTimeOffset? lockReleasedTimestampUtc = null,
        int? revisionId = null,
        CancellationToken? cancellationToken = null);
}
