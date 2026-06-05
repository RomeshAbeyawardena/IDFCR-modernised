namespace IDFCR.Abstractions.Metadata;

internal class DefaultMetaNamingConvention : IMetaNamingConvention
{
    private readonly DefaultMetaPagingNamingConvention _defaultPagingNamingConvention = new();
    public string Key => Meta.Key;
    public string ItemKey => Meta.ItemKey;
    public string SuccessKey => Meta.SuccessKey;
    public string FailureReason => Meta.FailureReason;
    public string ErrorMessage => Meta.ErrorMessage;
    public string ActionKey => Meta.ActionKey;
    public string CurrentEntityState => Meta.CurrentEntityState;

    public IMetaPagingNamingConvention Paging => _defaultPagingNamingConvention;
}
