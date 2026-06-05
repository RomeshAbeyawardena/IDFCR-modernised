namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents the naming convention for paging metadata keys used in paged query results. This interface allows for customization of the keys used to store paging information such as total rows, total pages, page index, and page size. By implementing this interface, users can define their own naming conventions for these keys instead of using the default ones defined in the <see cref="Meta.Paging"/> class.
/// </summary>
public interface IMetaPagingNamingConvention
{
    /// <summary>
    /// Gets the key used to store the total number of rows in paged query results. This key is used to identify the total row count in the paging metadata, allowing for consistent access to this information across different implementations. By default, it returns the value defined in <see cref="Meta.Paging.TotalRows"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string TotalRows { get; }
    /// <summary>
    /// Gets the key used to store the total number of pages in paged query results. This key is used to identify the total page count in the paging metadata, allowing for consistent access to this information across different implementations. By default, it returns the value defined in <see cref="Meta.Paging.TotalPages"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string TotalPages { get; }
    /// <summary>
    /// Gets the key used to store the current page index in paged query results. This key is used to identify the current page index in the paging metadata, allowing for consistent access to this information across different implementations. By default, it returns the value defined in <see cref="Meta.Paging.PageIndex"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string PageIndex { get; }
    /// <summary>
    /// Gets the key used to store the page size in paged query results. This key is used to identify the page size in the paging metadata, allowing for consistent access to this information across different implementations. By default, it returns the value defined in <see cref="Meta.Paging.PageSize"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string PageSize { get; }
}
