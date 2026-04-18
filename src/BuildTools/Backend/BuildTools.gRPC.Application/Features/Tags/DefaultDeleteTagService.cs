using BuildTools.Shared.Contracts.GRPC.Feature.Tags;
using Contracts = BuildTools.Shared.Contracts.Features.Tags;
using Grpc.Core;
using MediatR;
using System.Threading.Tasks;
using BuildTools.GRPC.Application.Extensions;
using BuildTools.GRPC.Shared.Contracts.Common;
using System;


namespace BuildTools.GRPC.Application.Features.Tags;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RegisteredServiceAttribute : Attribute { }


[RegisteredService]
public class DefaultDeleteTagService(IMediator mediator) : DeleteTagCommandService.DeleteTagCommandServiceBase
{
    public override async Task<UnitResult> DeleteTag(DeleteTagCommand request, ServerCallContext context)
    {
        var result = await mediator.Send(new Contracts.DeleteTagCommand
        {
            Name = request.Name,
            CommitChanges = true
        }, context.CancellationToken);

        return result.Map();
    }
}
