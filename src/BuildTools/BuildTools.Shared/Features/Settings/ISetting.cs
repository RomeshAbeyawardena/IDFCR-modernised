using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace BuildTools.Shared.Features.Settings;

/// <summary>
/// Represents a persisted configuration entry identified by a setting <see cref="Type"/> and <see cref="Key"/>.
/// </summary>
public interface ISetting : IMapper<ISetting>, IAuditCreatedTimestamp, IAuditModifiedTimestamp
{
    object? Id { get; }
    /// <summary>
    /// Gets the logical category for the setting (for example, tool, subsystem, or feature area).
    /// </summary>
    string Type { get; }
    /// <summary>
    /// Gets the unique setting name within the specified <see cref="Type"/>.
    /// </summary>
    string Key { get; }
    /// <summary>
    /// Gets the serialized setting value stored for this setting key.
    /// </summary>
    string? Value { get; }
    /// <summary>
    /// Gets the UTC timestamp of the most recent successful update to this setting.
    /// </summary>
}
