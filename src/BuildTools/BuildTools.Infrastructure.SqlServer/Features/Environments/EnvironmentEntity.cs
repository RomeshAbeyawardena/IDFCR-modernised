using BuildTools.Infrastructure.SqlServer.Features.Settings;
using BuildTools.Shared.Features.Environments;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;


namespace BuildTools.Infrastructure.SqlServer.Features.Environments;

public class EnvironmentEntity : MapperBase<IEnvironment>, IEnvironment, IIdentifiable<Guid>
{
    object? IEnvironment.Id => Id;
    public Guid Id { get; set; }
    public string ExternalReference { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }

    public ICollection<SettingEntity> Settings { get; set; } = [];

    public override void Map(IEnvironment source)
    {
        if (source.Id is not null && source.Id is Guid id)
        {
            Id = id;
        }

        ExternalReference = source.ExternalReference;
        Name = source.Name;
        DisplayName = source.DisplayName;
    }
}