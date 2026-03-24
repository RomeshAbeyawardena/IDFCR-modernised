namespace IDFCR.Abstractions.Filters.Extensions;

/// <summary>
/// Decorates a generic filter that is intended to be registered as a global filter.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class GlobalFilterAttribute : Attribute
{
}
