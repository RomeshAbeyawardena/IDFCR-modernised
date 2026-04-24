using BuildTools.Infrastructure.Features.Settings;
using BuildTools.Shared.Contracts.Features.SystemSettings;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace BuildTools.Application.Features.SystemSettings;

public class GetPagedSystemSettingsQueryHandler(ISettingRepository settingRepository)
    : IUnitPagedResultCollectionRequestHandler<GetPagedSystemSettingsQuery, SystemSettingDto>
{
    public async Task<IUnitPagedResult<SystemSettingDto>> Handle(GetPagedSystemSettingsQuery request, 
        CancellationToken cancellationToken)
    {
        var result = await settingRepository.GetPagedAsync(new GetPagedSettingsQuery
        {
            Fields = request.SortFields,
            Environment = request.Environment,
            Key = request.Key,
            KeyContains = request.KeyContains,
            Type = request.Type,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        }, cancellationToken);

        return result.Convert(x => x.Map<SystemSettingDto>());
    }
}
