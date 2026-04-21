using Grpc.Core;
using MediatR;
using System;
using System.Threading.Tasks;
using Contracts = BuildTools.Shared.Contracts.Features.Tags;
using BuildTools.Shared.Contracts.GRPC.Features.Tags;
using IDFCR.Abstractions.GRPC.Contracts.Common.V1;
using IDFCR.Abstractions.GRPC.Extensions;
using IDFCR.Abstractions.GRPC;

namespace BuildTools.GRPC.Application.Features.Tags;

[RegisteredGRPCServiceImplementation(true)]
public class DefaultDeleteTagService(IMediator mediator) : DeleteTagCommandService.DeleteTagCommandServiceBase
{
    public override async Task<UnitResult> DeleteTag(DeleteTagCommand request, ServerCallContext context)
    {
        var result = await mediator.Send(new Contracts.DeleteTagCommand
        {
            Name = request.Name,
            CommitChanges = true
        }, context.CancellationToken);

        return UnitResultExtensions.From(result);
    }
}
