using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings;

public class SettingEntity : MapperBase<ISetting>, ISetting, IIdentifiable<Guid>
{
    object? ISetting.Id => Id;
    public Guid Id { get; set; }
    /// <inheritdoc/>
    public string Type { get; set; } = null!;
    /// <inheritdoc/>
    public string Key { get; set; } = null!;
    /// <inheritdoc/>
    public string? Value { get; set; } = null!;
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    /// <inheritdoc/>
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }
    /// <inheritdoc/>
    public override void Map(ISetting source)
    {
        if (source.Id is not null && source.Id is Guid id)
        {
            Id = id;
        }

        Type = source.Type;
        Key = source.Key;
        Value = source.Value;
        CreatedTimestampUtc = source.CreatedTimestampUtc;
        ModifiedTimestampUtc = source.ModifiedTimestampUtc;
    }
}
