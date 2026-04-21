using BuildTools.Shared.Contracts.GRPC.Features.Tags;
using Grpc.Core;
using IDFCR.Abstractions.GRPC;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using Contracts = BuildTools.Shared.Contracts.Features.Tags;
using GRPCUnitResultExtensions = IDFCR.Abstractions.GRPC.Extensions;

namespace BuildTools.GRPC.Application.Features.Tags;

[RegisteredGRPCServiceImplementation(true)]
public class DefaultPagedTagsQueryService(IMediator mediator) : GetPagedTagsQueryService.GetPagedTagsQueryServiceBase
{
    public override async Task<GetPagedTagsQueryResult> GetPagedTags(GetPagedTagsQuery request, ServerCallContext context)
    {
        var result = await mediator.Send(new Contracts.GetPagedTagsQuery
        {
            Name = request.Name,
            NameContains = request.NameContains,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        }, context.CancellationToken);

        var totalPages = -1;
        if (result.Meta.TryGetValue("totalPages", out var _totalPages)
            && int.TryParse(_totalPages?.ToString(), out int pages))
        {
            totalPages = pages;
        }

        var response = new GetPagedTagsQueryResult
        {
            TotalRows = result.TotalRows,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            Result = GRPCUnitResultExtensions.UnitResultExtensions.From(result),
        };

        if (result.HasValue)
        {
            response.Tags.AddRange(result.Result.Select(x => new Shared.Contracts.Features.Tags.TagDto
            {
                DisplayName = x.DisplayName,
                Id = x.Id?.ToString(),
                Name = x.Name,
            }));
        }

        return response;
    }
}
