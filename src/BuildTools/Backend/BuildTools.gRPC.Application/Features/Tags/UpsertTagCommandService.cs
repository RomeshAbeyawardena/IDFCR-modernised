using BuildTools.GRPC.Shared.Contracts.Feature.Tags;
using Contracts = BuildTools.Shared.Contracts.Features.Tags;
using Grpc.Core;
using MediatR;
using System.Threading.Tasks;
using IDFCR.Abstractions.Results.Extensions;

using BuildTools.GRPC.Application.Extensions;

namespace BuildTools.GRPC.Application.Features.Tags;

[RegisteredService]
public class DefaultUpsertTagCommandService(IMediator mediator) : UpsertTagCommandService.UpsertTagCommandServiceBase
{
    public override async Task<UpsertTagCommandResult> UpsertTag(UpsertTagCommand request, ServerCallContext context)
    {
        var tag = request.Tag.Map();

        var foundResult = await mediator.Send(new Contracts.GetTagQuery { Name = tag.Name }, context.CancellationToken);

        var foundEntity = foundResult.GetResultOrDefault();

        tag.Id = foundEntity?.Id;

        var result = await mediator.Send(new Contracts.UpsertTagCommand { Tag = tag, CommitChanges = true }, context.CancellationToken);

        return new UpsertTagCommandResult
        {
            TagId = result.GetResultOrDefault()?.ToString(), //is a guid
            Result = result.Map()
        };
    }
}
