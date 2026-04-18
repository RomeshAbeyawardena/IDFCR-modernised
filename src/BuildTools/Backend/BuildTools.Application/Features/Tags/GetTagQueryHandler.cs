using BuildTools.Shared.Contracts.Feature.Tags;
using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using System.Threading;
using System.Threading.Tasks;

namespace BuildTools.Application.Features.Tags;

public class GetTagQueryHandler : IUnitResultRequestHandler<GetTagQuery, TagDto>
{
    public Task<IUnitResult<TagDto>> Handle(GetTagQuery request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}

public class Get
