using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Shared.Contracts.Feature.Tags;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildTools.Application.Features.Tags;

public class GetTagsQueryHandler(ITagRepository tagRepository) : IUnitResultCollectionRequestHandler<GetTagsQuery, TagDto>
{
    public async Task<IUnitResultCollection<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tagsResult = await tagRepository.GetExistingTagsAsync(request.Names, cancellationToken);

        return tagsResult.Convert(x => x.Map<TagDto>());
    }
}