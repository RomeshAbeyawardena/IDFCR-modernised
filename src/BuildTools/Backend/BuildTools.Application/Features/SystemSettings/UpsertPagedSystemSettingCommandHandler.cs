using BuildTools.Infrastructure.Features.Settings;
using BuildTools.Shared.Contracts.Features.SystemSettings;
using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildTools.Application.Features.SystemSettings
{
    public class UpsertPagedSystemSettingCommandHandler(ISettingRepository settingRepository) : IUnitResultRequestHandler<UpsertPagedSystemSettingCommand, object>
    {
        public async Task<IUnitResult<object>> Handle(UpsertPagedSystemSettingCommand request, CancellationToken cancellationToken)
        {
            if (request.Setting is null)
            {
                return UnitResult.Failed<object>(new NullReferenceException("System setting not specified"), UnitAction.None, FailureReason.ValidationError);
            }

            var mappedSetting = request.Setting.Map<Setting>();

            var foundSettingResult = await settingRepository.GetSettingAsync(mappedSetting.Key,
                mappedSetting.Type, request.Setting.EnvironmentName, cancellationToken);

            


            var result = await settingRepository.UpsertAsync(, cancellationToken);



        }
    }
}
