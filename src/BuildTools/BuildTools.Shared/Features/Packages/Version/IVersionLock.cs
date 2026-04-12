using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Packages.Version;

public interface IVersionLock : IMapper<IVersionLock>
{
    object? PackageId { get; }
    string? Reference { get; }
    string VersionPrefix { get; }
    int? RevisionId { get; }
    DateTimeOffset LastRequestedTimestampUtc { get; }
    DateTimeOffset? LockedFromTimestampUtc { get; }
    DateTimeOffset? LockedUntilTimestampUtc { get; }
    DateTimeOffset? LockReleasedTimestampUtc { get; }
}

public class VersionLock : MapperBase<IVersionLock>, IVersionLock
{
    public bool IsLocked(TimeProvider timeProvider)
    {
        var utcNow = timeProvider.GetUtcNow();
        return LockedFromTimestampUtc <= utcNow
            && LockedUntilTimestampUtc >= utcNow
            && !(LockReleasedTimestampUtc <= utcNow); // early release negates the lock
    }

    public object? PackageId { get; set; }
    public string? Reference { get; set; } = null!;
    public string VersionPrefix { get; set; } = null!;
    public int? RevisionId { get; set; }
    public DateTimeOffset LastRequestedTimestampUtc { get; set; }
    public DateTimeOffset? LockedFromTimestampUtc { get; set; }
    public DateTimeOffset? LockedUntilTimestampUtc { get; set; }

    public DateTimeOffset? LockReleasedTimestampUtc { get; set; }

    public override void Map(IVersionLock source)
    {
        PackageId = source.PackageId;
        Reference = source.Reference;
        VersionPrefix = source.VersionPrefix;
        LastRequestedTimestampUtc = source.LastRequestedTimestampUtc;
        LockedFromTimestampUtc = source.LockedFromTimestampUtc;
        LockedUntilTimestampUtc = source.LockedUntilTimestampUtc;
        LockReleasedTimestampUtc = source.LockReleasedTimestampUtc;
        RevisionId = source.RevisionId;
    }
}
