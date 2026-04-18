using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Shared.Contracts.Feature.Tags;
using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using System.Threading;
using System.Threading.Tasks;

namespace BuildTools.Application.Features.Tags;

public class UpsertTagCommandHandler(ITagRepository tagRepository) : IUnitResultRequestHandler<UpsertTagCommand, object>
{
    public async Task<IUnitResult<object>> Handle(UpsertTagCommand request, CancellationToken cancellationToken)
    {
        return (await tagRepository.UpsertAsync(request.Tag.Map<Tag>(), cancellationToken)).As<object>();
    }
}
