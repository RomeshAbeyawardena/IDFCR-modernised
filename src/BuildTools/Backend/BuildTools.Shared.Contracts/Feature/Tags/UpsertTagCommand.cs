using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Mediator;

namespace BuildTools.Shared.Contracts.Feature.Tags;

public class TagDto : MapperBase<ITag>, ITag
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

public record UpsertTagCommand : IUnitResultRequest<object>
{
    public TagDto? Tag { get; init; }
}