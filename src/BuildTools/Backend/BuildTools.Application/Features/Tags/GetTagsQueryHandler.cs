using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Shared.Contracts.Features.Tags;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Contracts = BuildTools.Shared.Contracts.Features.Tags;

namespace BuildTools.Application.Features.Tags;

public class GetTagsQueryHandler(ITagRepository tagRepository) : IUnitResultCollectionRequestHandler<GetTagsQuery, TagDto>
{
    public async Task<IUnitResultCollection<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tagsResult = await tagRepository.GetExistingTagsAsync(request.Names, cancellationToken);

        return tagsResult.Convert(x => x.Map<TagDto>());
    }
}

public class GetPagedTagsQueryHandler(ITagRepository tagRepository) : IUnitPagedResultCollectionRequestHandler<Contracts.GetPagedTagsQuery, TagDto>
{
    public Task<IUnitPagedResult<TagDto>> Handle(Contracts.GetPagedTagsQuery request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}