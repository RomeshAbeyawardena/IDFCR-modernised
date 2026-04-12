using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Packages.Version;

public interface IVersionLock : IMapper<IVersionLock>
{
    object? Id { get; }
    object? PackageId { get; }
    string? Reference { get; }
    string VersionPrefix { get; }
    int? RevisionId { get; }
    DateTimeOffset LastRequestedTimestampUtc { get; }
    DateTimeOffset? LockedFromTimestampUtc { get; }
    DateTimeOffset? LockedUntilTimestampUtc { get; }
    DateTimeOffset? LockReleasedTimestampUtc { get; }
}
