namespace BuildTools.Shared.Features.Settings;

/// <summary>
/// Describes a package known to the build tooling package catalog.
/// </summary>
/// <remarks>
/// This contract exposes stable metadata used to identify and present a package in automation flows,
/// including display, discovery, and package-to-namespace mapping.
/// </remarks>
public interface IPackage
{
    /// <summary>
    /// Gets the canonical package name used by the tooling ecosystem.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets an optional alternate package identifier used for compatibility or legacy naming.
    /// </summary>
    string? Alias { get; }
    /// <summary>
    /// Gets the package namespace used to group related packages.
    /// </summary>
    string Namespace { get; }
    /// <summary>
    /// Gets optional human-readable package details shown in catalog or UI surfaces.
    /// </summary>
    string? Description { get; }
}
