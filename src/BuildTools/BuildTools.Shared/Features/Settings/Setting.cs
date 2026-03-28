using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Settings;

/// <inheritdoc cref="ISetting" />
public class Setting : MapperBase<ISetting>, ISetting
{
    /// <inheritdoc/>
    public string Type { get; set; } = null!;
    /// <inheritdoc/>
    public string Key { get; set; } = null!;
    /// <inheritdoc/>
    public string? Value { get; set; } = null!;
    /// <inheritdoc/>
    public DateTime LastUpdatedTimestampUtc { get; set; }
    /// <inheritdoc/>
    public override void Map(ISetting source)
    {
        Type = source.Type;
        Key = source.Key;
        Value = source.Value;
        LastUpdatedTimestampUtc = source.LastUpdatedTimestampUtc;
    }
}