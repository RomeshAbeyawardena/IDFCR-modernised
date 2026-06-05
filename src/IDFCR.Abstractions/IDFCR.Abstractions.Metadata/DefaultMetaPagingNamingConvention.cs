namespace IDFCR.Abstractions.Metadata;

internal class DefaultMetaPagingNamingConvention : IMetaPagingNamingConvention
{
    public string TotalRows => Meta.Paging.TotalRows;
    public string TotalPages => Meta.Paging.TotalPages;
    public string PageIndex => Meta.Paging.PageIndex;
    public string PageSize => Meta.Paging.PageSize;
}
