namespace IDFCR.Abstractions.Results;

/// <summary>
/// Factory helpers for paged unit results.
/// </summary>
public static class UnitPagedResult
{
    /// <summary>
    /// Creates a paged result from the supplied data and paging request.
    /// </summary>
    /// <typeparam name="TResult">The element type.</typeparam>
    /// <param name="result">The page payload.</param>
    /// <param name="totalRows">The total row count before paging.</param>
    /// <param name="pagedQuery">The paging request that produced the result.</param>
    /// <param name="action">The associated action.</param>
    /// <param name="isSuccess">A value indicating whether the operation succeeded.</param>
    /// <param name="exception">The captured exception.</param>
    /// <param name="failureReason">The failure reason.</param>
    /// <param name="namedResult">An optional name for the result.</param>
    /// <returns>A paged unit result.</returns>
    public static IUnitPagedResult<TResult> FromResult<TResult>(
        IEnumerable<TResult>? result,
        int totalRows,
        IPagedQuery pagedQuery,
        UnitAction action = UnitAction.None,
        bool isSuccess = true,
        Exception? exception = null,
        FailureReason? failureReason = null,
        string? namedResult = null)
    {
        return new UnitPagedResult<TResult>(result, totalRows, pagedQuery, action, isSuccess, exception, failureReason, namedResult);
    }
}

/// <summary>
/// Represents a paged collection result.
/// </summary>
/// <typeparam name="TResult">The element type.</typeparam>
internal sealed record UnitPagedResult<TResult> : UnitResultCollection<TResult>, IUnitPagedResult<TResult>
{
    public static explicit operator UnitPagedResult<TResult>(DefaultUnitResult result)
    {
        return new UnitPagedResult<TResult>([], 0, null!, result.Action, result.IsSuccess, result.Exception, result.FailureReason);
    }

    /// <summary>
    /// Gets the paging request used to produce this result.
    /// </summary>
    public IPagedQuery PagedQuery { get; }

    /// <summary>
    /// Gets the total number of rows before paging.
    /// </summary>
    public int TotalRows { get; }

    /// <summary>
    /// Creates a paged result from the supplied data and paging request.
    /// </summary>
    public UnitPagedResult(
        IEnumerable<TResult>? result,
        int totalRows,
        IPagedQuery pagedQuery,
        UnitAction action = UnitAction.None,
        bool isSuccess = true,
        Exception? exception = null,
        FailureReason? failureReason = null,
        string? namedResult = null)
        : base(result, action, isSuccess, exception, failureReason, namedResult)
    {
        PagedQuery = pagedQuery;
        if (pagedQuery is not null)
        {
            base.AddMeta("pageIndex", pagedQuery.PageIndex);
            base.AddMeta("pageSize", pagedQuery.PageSize);
            TotalRows = totalRows;
            base.AddMeta("totalRows", totalRows);
            if (pagedQuery.PageSize.HasValue)
            {
                base.AddMeta("totalPages", (int)Math.Ceiling((double)totalRows / pagedQuery.PageSize.Value));
            }
        }
    }

    /// <summary>
    /// Creates a paged result without paging metadata.
    /// </summary>
    public UnitPagedResult(IEnumerable<TResult>? result, UnitAction action = UnitAction.None, bool isSuccess = true, Exception? exception = null,
        FailureReason? failureReason = null, string? namedResult = null)
        : this(result, 0, null!, action, isSuccess, exception, failureReason, namedResult)
    {

    }
}
