using BuildTools.Shared.Contracts.Features.SystemSettings;
using BuildTools.Shared.Contracts.GRPC.Features.SystemSettings;
using Grpc.Core;
using MediatR;
using System.Threading.Tasks;

namespace BuildTools.GRPC.Application.Features.SystemSettings;

public class UpsetSettingService(IMediator mediator) : UpsertSettingCommandService.UpsertSettingCommandServiceBase
{
    public override async Task<UpsertSettingCommandResult> UpsertSetting(UpsertSettingCommand request, ServerCallContext context)
    {
        var result = await mediator.Send(new UpsertSystemSettingCommand
        {
            Setting = new SystemSettingDto() 
            {
                EnvironmentName = request.Setting.Environment,
                Id = request.Setting.Id,
                Key = request.Setting.Key,
                Type = request.Setting.Type,
                Value = request.Setting.Value
            } // request.Setting
        }, context.CancellationToken);

        
    }
}
