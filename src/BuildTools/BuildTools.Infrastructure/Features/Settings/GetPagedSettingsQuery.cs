namespace BuildTools.Infrastructure.Features.Settings;

using IDFCR.Abstractions.Results;

public record GetPagedSettingsQuery : PagedSortedQuery
{
    public string? Environment { get; init; } = null!;
    public string? Key { get; init; } = null!;
    public string? KeyContains { get; init; }
    public string? Type { get; init; }
}
