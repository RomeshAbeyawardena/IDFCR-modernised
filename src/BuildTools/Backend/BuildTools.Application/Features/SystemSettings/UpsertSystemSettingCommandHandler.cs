using BuildTools.Infrastructure.Features.Environments;
using BuildTools.Infrastructure.Features.Settings;
using BuildTools.Shared.Contracts.Features.SystemSettings;
using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildTools.Application.Features.SystemSettings
{
    public class UpsertSystemSettingCommandHandler(ISettingRepository settingRepository,
        IEnvironmentRepository environmentRepository) 
        : IUnitResultRequestHandler<UpsertSystemSettingCommand, object>
    {
        public async Task<IUnitResult<object>> Handle(UpsertSystemSettingCommand request, CancellationToken cancellationToken)
        {
            if (request.Setting is null)
            {
                return UnitResult.Failed<object>(new NullReferenceException("System setting not specified"), UnitAction.None, FailureReason.ValidationError);
            }

            Shared.Features.Environments.Environment? environment = null;
            if (!string.IsNullOrWhiteSpace(request.Setting.EnvironmentName))
            {
                var environmentName = request.Setting.EnvironmentName;

                environment = (await environmentRepository.GetEnvironmentAsync(
                    new GetPagedEnvironmentQuery { Name = environmentName },
                    cancellationToken)).GetResultOrDefault();

                if (environment is null)
                {
                    environment = new()
                    {
                        Name = environmentName,
                        ExternalReference = environmentName,
                        DisplayName = environmentName
                    };

                    var saveEnvironmentResult = await environmentRepository
                        .UpsertAsync(environment, cancellationToken);

                    if (!saveEnvironmentResult.IsSuccess)
                    {
                        return saveEnvironmentResult.As<object>();
                    }

                    var id = saveEnvironmentResult.Result;

                    environment.Id = id;
                }

            }
            else if (request.Setting.EnvironmentId is not null && request.Setting.EnvironmentId is Guid id)
            {
                environment = (await environmentRepository.FindAsync(id, cancellationToken)).GetResultOrDefault();
            }

            var mappedSetting = request.Setting.Map<Setting>();

            var foundSettingResult = await settingRepository.GetSettingAsync(mappedSetting.Key,
                mappedSetting.Type, environment?.Name, cancellationToken);
            var foundSetting = foundSettingResult.GetResultOrDefault();

            if (foundSetting is not null)
            {
                mappedSetting.Id = foundSetting.Id;
                mappedSetting.CreatedTimestampUtc = foundSetting.CreatedTimestampUtc;
                mappedSetting.EnvironmentId = foundSetting.EnvironmentId;
            }
            else
            {
                mappedSetting.EnvironmentId = environment?.Id;
            }

            var result = await settingRepository.UpsertAsync(mappedSetting, cancellationToken);

            return result.As<object>();
        }
    }
}
