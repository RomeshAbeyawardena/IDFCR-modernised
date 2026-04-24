using BuildTools.Shared.Features.Environments;
using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Contracts.Features.SystemSettings;

public class SystemSettingDto : MapperBase<ISetting>, ISetting
{
    public object? Id { get; set; } = null!;
    public object? EnvironmentId { get; set; }
    public string? EnvironmentName { get; set; }
    /// <inheritdoc/>
    public string Type { get; set; } = null!;
    /// <inheritdoc/>
    public string Key { get; set; } = null!;
    /// <inheritdoc/>
    public string? Value { get; set; } = null!;
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    /// <inheritdoc/>
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }

    public IEnvironment? Environment { get; set; }

    /// <inheritdoc/>
    public override void Map(ISetting source)
    {
        Id = source.Id;
        EnvironmentId = source.EnvironmentId;
        Type = source.Type;
        Key = source.Key;
        Value = source.Value;
        CreatedTimestampUtc = source.CreatedTimestampUtc;
        ModifiedTimestampUtc = source.ModifiedTimestampUtc;
        Environment = source.Environment?.Map<EnvironmentDto>();
    }
}
