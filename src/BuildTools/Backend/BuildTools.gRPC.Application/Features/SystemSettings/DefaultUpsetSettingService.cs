using BuildTools.Shared.Contracts.Features.SystemSettings;
using BuildTools.Shared.Contracts.GRPC.Features.SystemSettings;
using Grpc.Core;
using IDFCR.Abstractions.GRPC;
using IDFCR.Abstractions.Results.Extensions;
using MediatR;
using System.Threading.Tasks;
using GRPCUnitResultExtensions = IDFCR.Abstractions.GRPC.Extensions;

namespace BuildTools.GRPC.Application.Features.SystemSettings;

[RegisteredGRPCServiceImplementation(true)]
public class DefaultUpsetSettingService(IMediator mediator) : UpsertSettingCommandService.UpsertSettingCommandServiceBase
{
    public override async Task<UpsertSettingCommandResult> UpsertSetting(UpsertSettingCommand request, ServerCallContext context)
    {
        var result = await mediator.Send(new UpsertSystemSettingCommand
        {
            CommitChanges = true,
            Setting = new SystemSettingDto() 
            {
                EnvironmentName = request.Setting.Environment,
                Id = request.Setting.Id,
                Key = request.Setting.Key,
                Type = request.Setting.Type,
                Value = request.Setting.Value
            } // request.Setting
        }, context.CancellationToken);

        return new UpsertSettingCommandResult
        {
            SettingId = result.GetResultOrDefault()?.ToString() ?? "",
            Result = GRPCUnitResultExtensions.UnitResultExtensions.From(result)
        };
    }
}
