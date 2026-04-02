namespace IDFCR.Abstractions.Results;

/// <summary>
/// Describes a request that carries page size and page index information.
/// </summary>
public interface IPagedQuery
{
    /// <summary>
    /// Gets the requested page size.
    /// </summary>
    int? PageSize { get; internal set; }

    /// <summary>
    /// Gets the requested page index.
    /// </summary>
    int? PageIndex { get; internal set; }

    /// <summary>
    /// Copies paging values from another paging request.
    /// </summary>
    void Map(IPagedQuery source);

    /// <summary>
    /// Copies paging values from a conventional skip/take request.
    /// </summary>
    void Map(IConventionalPagedQuery source);
}
