using BuildTools.GRPC.Shared.Contracts.Features.Tags;
using Grpc.Core;
using IDFCR.Abstractions.GRPC;
using IDFCR.Abstractions.Results.Extensions;
using MediatR;
using System.Threading.Tasks;
using Contracts = BuildTools.Shared.Contracts.Features.Tags;
using GRPCUnitResultExtensions = IDFCR.Abstractions.GRPC.Extensions;

namespace BuildTools.GRPC.Application.Features.Tags;

[RegisteredGRPCServiceImplementation(true)]
public class DefaultUpsertTagCommandService(IMediator mediator) : UpsertTagCommandService.UpsertTagCommandServiceBase
{
    public override async Task<UpsertTagCommandResult> UpsertTag(UpsertTagCommand request, ServerCallContext context)
    {
        var tag = request.Tag.Map();

        var result = await mediator.Send(new Contracts.UpsertTagCommand { Tag = tag, CommitChanges = true }, context.CancellationToken);

        return new UpsertTagCommandResult
        {
            TagId = result.GetResultOrDefault()?.ToString(), //is a guid
            Result = GRPCUnitResultExtensions.UnitResultExtensions.From(result)
        };
    }
}
