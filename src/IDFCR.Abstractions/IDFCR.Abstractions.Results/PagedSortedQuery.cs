using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a composition of a paged query and a sorted query, allowing for both pagination and sorting specifications to be included in a single request. This record combines the properties and functionality of both the <see cref="PagedQuery"/> and the <see cref="StructuredOrderedRequestBase"/> classes, enabling developers to easily create requests that require both pagination and sorting capabilities by inheriting from this composite record. The <see cref="PagedSortedQuery"/> class provides a convenient way to encapsulate all necessary information for handling paged and sorted queries in a unified manner, simplifying the process of creating and managing complex query requests throughout the application.
/// </summary>
public record PagedSortedQuery : StructuredOrderedRequestBase, IPagedQuery
{
    private readonly PagedQuery pagedQuery;

    /// <summary>
    /// Instantiates a new instance of the <see cref="PagedSortedQuery"/> record with the specified page size, page index, and optional sorting fields. This constructor initializes the internal <see cref="PagedQuery"/> instance with the provided pagination parameters and sets the sorting fields using the base class's functionality, allowing for a seamless combination of pagination and sorting specifications in a single request object.
    /// </summary>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="pageIndex">The zero-based index of the page.</param>
    /// <param name="sortFields">The optional sorting fields.</param>
    public PagedSortedQuery (int? pageSize, int? pageIndex, IEnumerable<ISort>? sortFields = null)
    {
        pagedQuery = new (pageSize, pageIndex);
        SortedFields = sortFields ?? [];
    }

    /// <summary>
    /// Instantiates a new instance of the <see cref="PagedSortedQuery"/> record with default pagination parameters (null for both page size and page index) and no sorting fields. This constructor allows for the creation of a paged and sorted query request without specifying any pagination or sorting details, providing a convenient way to initialize a request object that can be further configured with pagination and sorting specifications as needed.
    /// </summary>
    public PagedSortedQuery() : this(null, null)
    {
        
    }

    /// <inheritdoc />
    public int? PageSize { get => pagedQuery.PageSize; set => pagedQuery.PageSize = value; }

    /// <inheritdoc />
    public int? PageIndex { get => pagedQuery.PageIndex; set => pagedQuery.PageIndex = value; }

    /// <inheritdoc />
    public void Map(IPagedQuery source)
    {
        pagedQuery.Map(source);
    }

    /// <inheritdoc />
    public void Map(IConventionalPagedQuery source)
    {
        pagedQuery.Map(source);
    }
}
