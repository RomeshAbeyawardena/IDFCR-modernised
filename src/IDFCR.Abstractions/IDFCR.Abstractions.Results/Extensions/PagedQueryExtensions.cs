namespace IDFCR.Abstractions.Results.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IPagedQuery"/> implementations.
/// </summary>
public static class PagedQueryExtensions
{
    /// <summary>
    /// Maps paging properties from a source query to the target query.
    /// </summary>
    /// <typeparam name="T">The type of the target query that implements <see cref="IPagedQuery"/>.</typeparam>
    /// <param name="query">The target query to map values to.</param>
    /// <param name="sourceQuery">The source query to copy paging properties from.</param>
    public static void MapQuery<T>(this T query, IPagedQuery sourceQuery)
        where T : IPagedQuery
       
    {
        query.PageSize = sourceQuery.PageSize;
        query.PageIndex = sourceQuery.PageIndex;
    }
}
