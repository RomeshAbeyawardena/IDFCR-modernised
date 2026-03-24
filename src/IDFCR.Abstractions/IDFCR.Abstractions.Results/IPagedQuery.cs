namespace IDFCR.Abstractions.Results;

/// <summary>
/// Describes a request that carries page size and page index information.
/// </summary>
public interface IPagedQuery
{
    /// <summary>
    /// Gets the requested page size.
    /// </summary>
    int? PageSize { get; }

    /// <summary>
    /// Gets the requested page index.
    /// </summary>
    int? PageIndex { get; }

    /// <summary>
    /// Copies paging values from another paging request.
    /// </summary>
    void Map(IPagedQuery source);

    /// <summary>
    /// Copies paging values from a conventional skip/take request.
    /// </summary>
    void Map(IConventionalPagedQuery source);
}

/// <summary>
/// Default paging request implementation.
/// </summary>
public record PagedQuery() : IPagedQuery
{
    /// <summary>
    /// Creates a paging request with the supplied page size and page index.
    /// </summary>
    /// <param name="pageSize">The page size.</param>
    /// <param name="pageIndex">The zero-based page index.</param>
    public PagedQuery(int? pageSize, int? pageIndex) : this()
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
    }

    /// <inheritdoc />
    public int? PageSize { get; set; }

    /// <inheritdoc />
    public int? PageIndex { get; set; }

    /// <inheritdoc />
    public void Map(IPagedQuery source)
    {
        PageIndex = source.PageIndex;
        PageSize = source.PageSize;
    }

    /// <inheritdoc />
    public void Map(IConventionalPagedQuery source)
    {
        if (source.Skip.HasValue && source.Take.HasValue)
        {
            PageSize = source.Take;
            PageIndex = source.Skip.HasValue && source.Take.HasValue && source.Take.Value != 0
                            ? source.Skip / source.Take
                            : null;
        }
    }
}
