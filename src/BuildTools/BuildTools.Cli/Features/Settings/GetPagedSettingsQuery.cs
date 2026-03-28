using IDFCR.Abstractions.Results;

public record GetPagedSettingsQuery : PagedQuery
{
    public string? NameContains { get; init; }
    public string? Type { get; init; }
}
