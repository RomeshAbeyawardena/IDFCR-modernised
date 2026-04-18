using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Shared.Contracts.Features.Tags;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Contracts = BuildTools.Shared.Contracts.Features.Tags;

namespace BuildTools.Application.Features.Tags;

public class GetPagedTagsQueryHandler(ITagRepository tagRepository) : IUnitPagedResultCollectionRequestHandler<Contracts.GetPagedTagsQuery, TagDto>
{
    public async Task<IUnitPagedResult<TagDto>> Handle(Contracts.GetPagedTagsQuery request, CancellationToken cancellationToken)
    {
        var result = await tagRepository.GetPagedAsync(new Infrastructure.Features.Tags.GetPagedTagsQuery
        {
            Name = request.Name,
            NameContains = request.NameContains,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        }, cancellationToken);

        return result.Convert(x => x.Map<TagDto>());
    }
}