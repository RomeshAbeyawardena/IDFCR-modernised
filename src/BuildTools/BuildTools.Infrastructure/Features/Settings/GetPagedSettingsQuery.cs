namespace BuildTools.Infrastructure.Features.Settings;

using IDFCR.Abstractions.Results;

public record GetPagedSettingsQuery : PagedQuery
{
    public string Name { get; init; }
    public string? NameContains { get; init; }
    public string? Type { get; init; }
}
