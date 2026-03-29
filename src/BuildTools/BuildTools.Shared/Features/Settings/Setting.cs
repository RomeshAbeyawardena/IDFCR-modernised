using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Settings;

/// <inheritdoc cref="ISetting" />
public class Setting : MapperBase<ISetting>, ISetting
{
    public object? Id { get; set; } = null!;
    /// <inheritdoc/>
    public string Type { get; set; } = null!;
    /// <inheritdoc/>
    public string Key { get; set; } = null!;
    /// <inheritdoc/>
    public string? Value { get; set; } = null!;
    /// <inheritdoc/>
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }

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