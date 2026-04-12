using BuildTools.Infrastructure.Features.Packages.Version;
using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;
using IDFCR.Persistence.EntityFrameworkCore;
using System.Data.Entity;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages.Version;

public class VersionLockEntity : MapperBase<IVersionLock>, IVersionLock
    , IIdentifiable<Guid>
{
    object? IVersionLock.PackageId => PackageId;
    object? IVersionLock.Id => Id;
    public Guid PackageId { get; set; }
    public string? Reference { get; set; } = null!;
    public string VersionPrefix { get; set; } = null!;
    public int? RevisionId { get; set; }
    public DateTimeOffset LastRequestedTimestampUtc { get; set; }
    public DateTimeOffset? LockedFromTimestampUtc { get; set; }
    public DateTimeOffset? LockedUntilTimestampUtc { get; set; }
    public DateTimeOffset? LockReleasedTimestampUtc { get; set; }
    public Guid Id { get; set; }

    public override void Map(IVersionLock source)
    {
        if (source.Id is not null && source.Id is Guid id)
        {
            Id = id;
        }

        if (source.PackageId is not null && source.PackageId is Guid packageid)
        {
            PackageId = packageid;
        }

        Reference = source.Reference;
        VersionPrefix = source.VersionPrefix;
        LastRequestedTimestampUtc = source.LastRequestedTimestampUtc;
        LockedFromTimestampUtc = source.LockedFromTimestampUtc;
        LockedUntilTimestampUtc = source.LockedUntilTimestampUtc;
        LockReleasedTimestampUtc = source.LockReleasedTimestampUtc;
        RevisionId = source.RevisionId;
    }
}

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
                && x.VersionPrefix == versionPrefix);

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
            Id = existingResult.Result?.Id,
            LastRequestedTimestampUtc = utcNow,
            LockedFromTimestampUtc = lockedFromTimestampUtc,
            LockedUntilTimestampUtc = lockedUntilTimestampUtc,
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
