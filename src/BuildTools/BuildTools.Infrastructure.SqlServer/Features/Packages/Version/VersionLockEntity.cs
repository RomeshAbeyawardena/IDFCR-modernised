using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

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
