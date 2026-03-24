namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a paged unit result.
/// </summary>
/// <typeparam name="TResult">The element type.</typeparam>
public interface IUnitPagedResult<TResult> : IUnitResult<IEnumerable<TResult>>
{
    /// <summary>
    /// Gets the total number of rows before paging was applied.
    /// </summary>
    int TotalRows { get; }

    /// <summary>
    /// Gets the paging query associated with the result.
    /// </summary>
    IPagedQuery PagedQuery { get; }
}
