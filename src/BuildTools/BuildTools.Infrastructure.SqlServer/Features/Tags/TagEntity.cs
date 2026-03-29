using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildTools.Infrastructure.SqlServer.Features.Tags;

public class TagEntity : MapperBase<ITag>, ITag, IIdentifiable<Guid>
{
    public Guid Id { get; set; }
    object? ITag.Id => Id;
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }

    public override void Map(ITag source)
    {
        if (source.Id is not null && source.Id is Guid id)
        {
            Id = id;
        }

        Name = source.Name;
        DisplayName = source.DisplayName;
    }
}

public class TagEntityConfiguration : IEntityTypeConfiguration<TagEntity>
{
    public void Configure(EntityTypeBuilder<TagEntity> builder)
    {
        
    }
}