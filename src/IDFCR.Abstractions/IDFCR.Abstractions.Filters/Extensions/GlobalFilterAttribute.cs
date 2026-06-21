namespace IDFCR.Abstractions.Filters.Extensions;

/// <summary>
/// Decorates a generic filter that is intended to be registered as a global filter with fine-tuned control over its filter behavior.
/// </summary>
/// <param name="isStandardFilter">Indicates whether the filter is a standard filter.</param>
/// <param name="isPagedFilter">Indicates whether the filter is a paged filter.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class GlobalFilterAttribute(bool isStandardFilter = true, bool isPagedFilter = false) : Attribute
{
    /// <summary>
    /// Gets a value indicating whether the filter is a standard filter.
    /// </summary>
    public bool IsStandard => isStandardFilter;
    /// <summary>
    /// Gets a value indicating whether the filter is a paged filter.
    /// </summary>
    public bool IsPaged => isPagedFilter;
}
