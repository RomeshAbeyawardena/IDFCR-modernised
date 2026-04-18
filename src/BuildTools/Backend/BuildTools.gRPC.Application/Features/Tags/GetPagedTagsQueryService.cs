using BuildTools.Shared.Contracts.GRPC.Feature.Tags;
using Contracts = BuildTools.Shared.Contracts.Features.Tags;
using Grpc.Core;
using MediatR;
using System.Threading.Tasks;
using BuildTools.GRPC.Application.Extensions;
using System.Linq;


namespace BuildTools.GRPC.Application.Features.Tags;

public class DefaultPagedTagsQueryService(IMediator mediator) : GetPagedTagsQueryService.GetPagedTagsQueryServiceBase
{
    public override async Task<GetPagedTagsQueryResult> GetPagedTags(GetPagedTagsQuery request, ServerCallContext context)
    {
        var result = await mediator.Send(new Contracts.GetPagedTagsQuery {
            Name = request.Name,
            NameContains = request.NameContains,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        }, context.CancellationToken);

        var response = new GetPagedTagsQueryResult
        {
            Result = result.Map(),
        };

        if (result.HasValue)
        {
            response.Tags.AddRange(result.Result.Select(x => x.Map()));
        }

        return response;
    }
}
