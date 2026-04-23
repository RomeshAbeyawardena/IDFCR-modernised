using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Metadata;

namespace BuildTools.Shared.Contracts.Features.SystemSettings;

public record GetPagedSystemSettingsQuery : PagedUnitResultRequestBase<SystemSettingDto>
{
    public IEnumerable<ISort> SortFields { get; init; } = [];
    public string? Environment { get; init; }
    public string? Key { get; init; }
    public string? KeyContains { get; init; }
    public string? Type { get; init; }
}
