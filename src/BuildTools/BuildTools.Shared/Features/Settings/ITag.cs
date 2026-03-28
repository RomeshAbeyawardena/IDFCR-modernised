namespace BuildTools.Shared.Features.Settings;

/// <summary>
/// Represents a tag used to label and classify entities in tooling metadata.
/// </summary>
public interface ITag
{
    /// <summary>
    /// Gets the stable tag identifier used in code and storage.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets the optional human-friendly label shown for the tag in UI or reports.
    /// </summary>
    string? DisplayName { get; }
}