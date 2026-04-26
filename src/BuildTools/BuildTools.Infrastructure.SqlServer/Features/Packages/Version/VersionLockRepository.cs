using BuildTools.Infrastructure.Features.Packages.Version;
using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors.Factories;
using IDFCR.Abstractions.Results;
using IDFCR.Persistence.EntityFrameworkCore;
using IDFCR.Persistence.EntityFrameworkCore.Attributes;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages.Version;

[RegisteredRepository]
public class VersionLockRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory, TimeProvider timeProvider)
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, IVersionLock, VersionLockEntity, VersionLock, Guid>(db, filterFactory, entityInterceptorFactory), IVersionLockRepository
{
    public async Task<IUnitResult<VersionLock>> GetVersionLockAsync(object packageId, string versionPrefix, CancellationToken cancellationToken)
    {
        if (packageId is not Guid _packageId)
        {
            return UnitResult.Failed<VersionLock>(new InvalidOperationException("Package ID not specified or in the wrong format"), FailureReason: FailureReason.ValidationError);
        }

        var result = await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PackageId == _packageId
                && x.VersionPrefix == versionPrefix, cancellationToken);

        if (result is null)
        {
            return UnitResult.NotFound<VersionLock>(packageId);
        }

        return UnitResult.FromResult(Map(result));
    }

    public async Task<IUnitResult<object>> SetVersionLockAsync(object packageId, string versionPrefix, string? reference = null, DateTimeOffset? lockedFromTimestampUtc = null, DateTimeOffset? lockedUntilTimestampUtc = null, DateTimeOffset? lockReleasedTimestampUtc = null, int? revisionId = null, CancellationToken? cancellationToken = null)
    {
        var ct = cancellationToken.GetValueOrDefault(CancellationToken.None);

        var utcNow = timeProvider.GetUtcNow();

        var existingResult = (await GetVersionLockAsync(packageId, versionPrefix, ct));

        if (!existingResult.IsSuccess && existingResult.FailureReason != FailureReason.NotFound)
        {
            return UnitResult.Failed<object>(existingResult.Exception
                ?? new InvalidOperationException("Unknown error"));
        }

        var result = await UpsertAsync(new VersionLock
        {
            RowVersion = existingResult.Result?.RowVersion,
            Id = existingResult.Result?.Id,
            LastRequestedTimestampUtc = utcNow,
            LockedFromTimestampUtc = lockedFromTimestampUtc ?? existingResult.Result?.LockedFromTimestampUtc,
            LockedUntilTimestampUtc = lockedUntilTimestampUtc ?? existingResult.Result?.LockedUntilTimestampUtc,
            LockReleasedTimestampUtc = lockReleasedTimestampUtc,
            PackageId = packageId,
            Reference = reference,
            RevisionId = revisionId,
            VersionPrefix = versionPrefix
        }, ct);

        return result.As<object>();
    }

    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}
