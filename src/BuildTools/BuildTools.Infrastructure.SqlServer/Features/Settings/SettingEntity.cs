using BuildTools.Infrastructure.SqlServer.Features.Environments;
using BuildTools.Shared.Features.Environments;
using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings;

public class SettingEntity : MapperBase<ISetting>, ISetting, IIdentifiable<Guid>
{
    object? ISetting.Id => Id;
    IEnvironment? ISetting.Environment => Environment;
    object? ISetting.EnvironmentId => EnvironmentId;
    public Guid Id { get; set; }

    public Guid? EnvironmentId { get; set; }
    /// <inheritdoc/>
    public string Type { get; set; } = null!;
    /// <inheritdoc/>
    public string Key { get; set; } = null!;
    /// <inheritdoc/>
    public string? Value { get; set; } = null!;
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    /// <inheritdoc/>
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }

    public virtual EnvironmentEntity? Environment { get; set; }

    /// <inheritdoc/>
    public override void Map(ISetting source)
    {
        if (source.Id is not null && source.Id is Guid id)
        {
            Id = id;
        }

        if (source.EnvironmentId is not null && source.EnvironmentId is Guid environmentId)
        {
            EnvironmentId = environmentId;
        }

        Type = source.Type;
        Key = source.Key;
        Value = source.Value;
        CreatedTimestampUtc = source.CreatedTimestampUtc;
        ModifiedTimestampUtc = source.ModifiedTimestampUtc;

        Environment = source.Environment?.Map<EnvironmentEntity>();
    }
}
