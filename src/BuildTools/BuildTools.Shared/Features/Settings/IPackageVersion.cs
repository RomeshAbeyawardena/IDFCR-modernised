namespace BuildTools.Shared.Features.Settings;

/// <summary>
/// Defines version and publication state for a specific package revision.
/// </summary>
/// <remarks>
/// Implementations track build lineage (package identity, semantic version components, and source commit)
/// together with feed publication lifecycle metadata in UTC for auditing and retry workflows.
/// </remarks>
public interface IPackageVersion
{
    /// <summary>
    /// Gets the unique package identifier this version record belongs to.
    /// </summary>
    object PackageId { get; }
    /// <summary>
    /// Gets the semantic version prefix (for example, <c>1.4</c>) used before revision suffixing.
    /// </summary>
    string VersionPrefix { get; }
    /// <summary>
    /// Gets the monotonically increasing revision component for this package build.
    /// </summary>
    int RevisionNumber { get; }
    /// <summary>
    /// Gets the UTC timestamp when this package revision was released.
    /// </summary>
    DateTime ReleaseDateTimestampUtc { get; }
    /// <summary>
    /// Gets the source control commit SHA associated with the produced package revision.
    /// </summary>
    string CommitId { get; }
    /// <summary>
    /// Gets a value indicating whether this revision has been successfully published to the package feed.
    /// </summary>
    bool PublishedToFeed { get; }
    /// <summary>
    /// Gets the UTC timestamp of the most recent failed publish attempt, or <see langword="null"/> when no failures have occurred.
    /// </summary>
    DateTime? LastErrorOnPublishAttemptTimestampUtc { get; }
    /// <summary>
    /// Gets the UTC timestamp when this revision was published to the feed, or <see langword="null"/> if unpublished.
    /// </summary>
    DateTime? PublishedTimestampUtc { get; }
}
