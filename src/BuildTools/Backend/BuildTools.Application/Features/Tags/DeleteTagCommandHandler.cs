using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Shared.Contracts.Features.Tags;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildTools.Application.Features.Tags;

public class DeleteTagCommandHandler(ITagRepository tagRepository) : IUnitResultRequestHandler<DeleteTagCommand>
{
    public async Task<IUnitResult> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var foundEntity = await tagRepository.GetTagAsync(request.Name, cancellationToken);

        if (!foundEntity.HasValue)
        {
            return foundEntity;
        }

        if (foundEntity.Result.Id is Guid id)
        {
            return await tagRepository.DeleteAsync(id, cancellationToken);
        }

        return UnitResult.Failed(new InvalidCastException("Unable to cast PRIMARY KEY to the correct format"), UnitAction.None, FailureReason.InternalError);
    }
}
