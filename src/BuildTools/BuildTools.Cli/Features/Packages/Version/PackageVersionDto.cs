using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Mapper;

namespace BuildTools.Cli.Features.Packages.Version;

public class PackageVersionDto : MapperBase<IPackageVersion>, IPackageVersion
{
    public object PackageId { get; set; } = null!;
    public string VersionPrefix { get; set; } = null!;
    public int RevisionNumber { get; set; }
    public DateTimeOffset ReleaseDateTimestampUtc { get; set; }
    public string CommitId { get; set; } = null!;
    public bool PublishedToFeed { get; set; }
    public DateTimeOffset? LastErrorOnPublishAttemptTimestampUtc { get; set; }
    public DateTimeOffset? PublishedTimestampUtc { get; set; }
    public string Version { get; set; } = null!;
    public object? PackageVersionId { get; set; }

    public override void Map(IPackageVersion source)
    {
        Version = source.Version;
        PackageVersionId = source.PackageVersionId;
        PackageId = source.PackageId;
        VersionPrefix = source.VersionPrefix;
        RevisionNumber = source.RevisionNumber;
        ReleaseDateTimestampUtc = source.ReleaseDateTimestampUtc;
        CommitId = source.CommitId;
        PublishedToFeed = source.PublishedToFeed;
        LastErrorOnPublishAttemptTimestampUtc =
            source.LastErrorOnPublishAttemptTimestampUtc;
        PublishedTimestampUtc = source.PublishedTimestampUtc;
    }
}
