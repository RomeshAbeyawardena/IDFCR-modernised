using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Results;

/// <summary>
/// Factory helpers for paged unit results.
/// </summary>
public static class PagedUnitResult
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
    public static IPagedUnitResult<TResult> FromResult<TResult>(
        IEnumerable<TResult>? result,
        int totalRows,
        IPagedQuery pagedQuery,
        UnitAction action = UnitAction.None,
        bool isSuccess = true,
        Exception? exception = null,
        FailureReason? failureReason = null,
        string? namedResult = null)
    {
        return new PagedUnitResult<TResult>(result, totalRows, pagedQuery, action, isSuccess, exception, failureReason, namedResult);
    }
}

/// <summary>
/// Represents a paged collection result.
/// </summary>
/// <typeparam name="TResult">The element type.</typeparam>
internal sealed record PagedUnitResult<TResult> : UnitResultCollection<TResult>, IPagedUnitResult<TResult>
{
    public static explicit operator PagedUnitResult<TResult>(DefaultUnitResult result)
    {
        return new PagedUnitResult<TResult>([], 0, null!, result.Action, result.IsSuccess, result.Exception, result.FailureReason);
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
    public PagedUnitResult(
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
        var pagingMetaNamingConvention = MetaNaming.Convention.Paging;

        PagedQuery = pagedQuery;
        if (pagedQuery is not null)
        {
            base.AddMeta(pagingMetaNamingConvention.PageIndex, pagedQuery.PageIndex);
            base.AddMeta(pagingMetaNamingConvention.PageSize, pagedQuery.PageSize);
            TotalRows = totalRows;
            base.AddMeta(pagingMetaNamingConvention.TotalRows, totalRows);
            if (pagedQuery.PageSize > 0)
            {
                base.AddMeta(pagingMetaNamingConvention.TotalPages, (int)Math.Ceiling((double)totalRows / pagedQuery.PageSize.Value));
            }
        }
    }

    /// <summary>
    /// Creates a paged result without paging metadata.
    /// </summary>
    public PagedUnitResult(IEnumerable<TResult>? result, UnitAction action = UnitAction.None, bool isSuccess = true, Exception? exception = null,
        FailureReason? failureReason = null, string? namedResult = null)
        : this(result, 0, null!, action, isSuccess, exception, failureReason, namedResult)
    {

    }
}
