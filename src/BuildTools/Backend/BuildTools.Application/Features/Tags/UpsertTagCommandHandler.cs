using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Shared.Contracts.Features.Tags;
using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildTools.Application.Features.Tags;

public class UpsertTagCommandHandler(ITagRepository tagRepository) : IUnitResultRequestHandler<UpsertTagCommand, object>
{
    public async Task<IUnitResult<object>> Handle(UpsertTagCommand request, CancellationToken cancellationToken)
    {
        if (request.Tag is null)
        {
            return UnitResult.Failed<object>(new NullReferenceException("Tag not specified"), UnitAction.None, FailureReason.ValidationError);
        }

        var foundTagResult = await tagRepository.GetTagAsync(request.Tag.Name, cancellationToken);
        var mappedTag = request.Tag.Map<Tag>();
        Tag? foundTag = foundTagResult.GetResultOrDefault();
        if (foundTag is not null)
        {
            mappedTag.Id = foundTag.Id;
        }

        var result = await tagRepository.UpsertAsync(mappedTag, cancellationToken);

        return result.As<object>();
    }
}
