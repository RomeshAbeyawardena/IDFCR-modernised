using BuildTools.Shared.Contracts.Features.SystemSettings;
using BuildTools.Shared.Contracts.GRPC.Features.SystemSettings;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IDFCR.Abstractions.GRPC.Extensions;
using MediatR;
using System.Linq;
using System.Threading.Tasks;

namespace BuildTools.GRPC.Application.Features.SystemSettings;
public class DefaultPagedSettingsService(IMediator mediator) : GetPagedSettingsQueryService.GetPagedSettingsQueryServiceBase
{
    public override async Task<GetPagedSettingsQueryResult> GetPagedSettings(GetPagedSettingsQuery request, 
        ServerCallContext context)
    {
        var sortFields = request.OrderedFields.Select(x => x.From());

        var result = await mediator.Send(new GetPagedSystemSettingsQuery
        {
            Environment = request.Environment,
            Key = request.Key,
            KeyContains = request.KeyContains,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            SortFields = sortFields
        }, context.CancellationToken);

        var totalPages = -1;
        if (result.Meta.TryGetValue("totalPages", out var _totalPages)
            && int.TryParse(_totalPages?.ToString(), out int pages))
        {
            totalPages = pages;
        }

        var response = new GetPagedSettingsQueryResult
        {
            TotalRows = result.TotalRows,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            Result = result.From(),
        };

        if (result.HasValue)
        {
            response.Settings.AddRange(result.Result.Select(x => new SettingDto
            {
                Id = x.Id?.ToString(),
                Environment = x.Environment.Name,
                Key = x.Key,
                Modified = Timestamp.FromDateTimeOffset(x.ModifiedTimestampUtc.GetValueOrDefault(x.CreatedTimestampUtc)),
                Type = x.Type,
                Value = x.Value
            }));
        }

        return response;
    }
}
