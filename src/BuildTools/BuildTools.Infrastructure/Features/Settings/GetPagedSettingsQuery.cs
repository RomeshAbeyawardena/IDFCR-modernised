namespace BuildTools.Infrastructure.Features.Settings;

using IDFCR.Abstractions.Results;

public record GetPagedSettingsQuery : PagedQuery
{
    public string? Key { get; init; } = null!;
    public string? KeyContains { get; init; }
    public string? Type { get; init; }
}
