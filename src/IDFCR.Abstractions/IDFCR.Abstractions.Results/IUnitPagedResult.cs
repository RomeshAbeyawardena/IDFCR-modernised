namespace IDFCR.Abstractions.Results;

public interface IUnitPagedResult<TResult> : IUnitResult<IEnumerable<TResult>>
{
    int TotalRows { get; }
    IPagedQuery PagedQuery { get; }
}
