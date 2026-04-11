using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages.Version;
public class PackageVersionEntity : MapperBase<IPackageVersion>, IPackageVersion, IIdentifiable<Guid>
{
    object IPackageVersion.PackageId => PackageId;
    object? IPackageVersion.PackageVersionId => Id;

    string IPackageVersion.Version => $"{VersionPrefix}.{RevisionNumber}";
    public Guid Id { get; set; }
    public Guid PackageId { get; set; }
    public string VersionPrefix { get; set; } = null!;
    public int RevisionNumber { get; set; }
    public DateTimeOffset ReleaseDateTimestampUtc { get; set; }
    public string CommitId { get; set; } = null!;
    public bool PublishedToFeed { get; set; }
    public DateTimeOffset? LastErrorOnPublishAttemptTimestampUtc { get; set; }
    public DateTimeOffset? PublishedTimestampUtc { get; set; }

    public virtual PackageEntity Package { get; set; } = null!;

    public override void Map(IPackageVersion source)
    {
        if (source.PackageVersionId is not null && source.PackageVersionId is Guid id)
        {
            Id = id;
        }

        if (source.PackageId is not null && source.PackageId is Guid packageId)
        {
            PackageId = packageId;
        }
        
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
