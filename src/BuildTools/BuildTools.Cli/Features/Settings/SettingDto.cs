using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Mapper;

public class SettingDto : MapperBase<ISetting>, ISetting
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
