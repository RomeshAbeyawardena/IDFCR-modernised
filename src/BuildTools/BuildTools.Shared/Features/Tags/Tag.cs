using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Tags;

public class Tag : MapperBase<ITag>, ITag
{
    public object? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }

    public override void Map(ITag source)
    {
        Id = source.Id;
        Name = source.Name;
        DisplayName = source.DisplayName;
    }
}