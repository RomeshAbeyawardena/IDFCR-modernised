using BuildTools.GRPC.Shared.Contracts.Feature.Tags;
using Contracts = BuildTools.Shared.Contracts.Features.Tags;
namespace BuildTools.GRPC.Application.Features.Tags;

public static class TagDtoExtensions
{
    public static Contracts.TagDto Map(this TagDto tagDto)
    {
        return new Contracts.TagDto
        {
            DisplayName = tagDto.DisplayName,
            Id = tagDto.Id,
            Name = tagDto.Name
        };
    }

    public static TagDto Map(this Contracts.TagDto tagDto)
    {
        return new TagDto
        {
            DisplayName = tagDto.DisplayName,
            Id = tagDto.Id?.ToString() ?? string.Empty,
            Name = tagDto.Name
        };
    }
}
