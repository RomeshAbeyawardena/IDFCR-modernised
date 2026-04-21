namespace IDFCR.Abstractions.Metadata;

internal record DefaultOrderedRequest : IOrderedRequest
{
    public string? OrderBy { get; init; }
    public OrderDirection? DefaultOrderDirection { get; init; }
}
