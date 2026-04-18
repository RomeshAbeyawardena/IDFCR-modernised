using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Shared.Contracts.Feature.Tags;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace BuildTools.Application.Features.Tags;

public class GetTagQueryHandler(ITagRepository tagRepository) : IUnitResultRequestHandler<GetTagQuery, TagDto>
{
    public async Task<IUnitResult<TagDto>> Handle(GetTagQuery request, CancellationToken cancellationToken)
    {
        var tagResult = await tagRepository.GetTagAsync(request.Name, cancellationToken);

        if (!tagResult.HasValue)
        {
            return UnitResult.Failed<TagDto>(tagResult.Exception, tagResult.Action, tagResult.FailureReason);
        }

        return UnitResult.FromResult(tagResult.GetResultOrDefault().Map<TagDto>(), tagResult.Action, true, tagResult.Exception);
    }
}
