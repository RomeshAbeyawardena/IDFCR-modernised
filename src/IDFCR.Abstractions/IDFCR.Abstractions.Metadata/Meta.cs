namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Defines the metadata keys used in unit results and other related types.
/// </summary>
public static class Meta
{
    /// <summary>
    /// Defines the key used to store metadata in unit results and other related types.
    /// </summary>
    public const string Key = "_meta";

    /// <summary>
    /// Defines the keys used for paging information in paged query results.
    /// </summary>
    public static class Paging
    {
        /// <summary>
        /// The key used to store the total number of pages in paged query results.
        /// </summary>
        public const string TotalPages = "total_pages";
        /// <summary>
        /// The key used to store the current page index in paged query results.
        /// </summary>
        public const string PageIndex = "page_index";
        /// <summary>
        /// The key used to store the page size in paged query results.
        /// </summary>
        public const string PageSize = "page_size";
    }
}

