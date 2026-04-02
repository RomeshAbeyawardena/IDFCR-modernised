using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Mapper;

namespace BuildTools.Cli.Features.Settings;

public class SettingDto : MapperBase<ISetting>, ISetting
{
    public object? Id { get; set; } = null!;
    /// <inheritdoc/>
    public string Type { get; set; } = null!;
    /// <inheritdoc/>
    public string Key { get; set; } = null!;
    /// <inheritdoc/>
    public string? Value { get; set; } = null!;
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    /// <inheritdoc/>
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }

    public string LastUpdated => ModifiedTimestampUtc.GetValueOrDefault(CreatedTimestampUtc).ToLocalTime().ToString("dd MMM yyyy HH:mm") ?? "No data";

    /// <inheritdoc/>
    public override void Map(ISetting source)
    {
        Id = source.Id;
        Type = source.Type;
        Key = source.Key;
        Value = source.Value;
        CreatedTimestampUtc = source.CreatedTimestampUtc;
        ModifiedTimestampUtc = source.ModifiedTimestampUtc;
    }
}
