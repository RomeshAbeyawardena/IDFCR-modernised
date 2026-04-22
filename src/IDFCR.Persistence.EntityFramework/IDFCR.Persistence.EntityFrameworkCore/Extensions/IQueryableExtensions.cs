using System.Linq.Expressions;

namespace IDFCR.Persistence.EntityFrameworkCore.Extensions;

/// <summary>
/// Defines extension methods for applying dynamic ordering to an <see cref="IQueryable{T}"/> based on a string input. This allows for flexible sorting of query results at runtime without the need for hardcoded property names or multiple sorting methods. The method supports nested properties and both ascending and descending order specifications.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Defines an extension method for <see cref="IQueryable{T}"/> that applies dynamic ordering based on a string input. The input string can specify multiple sorting criteria, separated by commas, and can indicate ascending or descending order for each criterion. The method constructs the necessary expression trees to apply the specified ordering to the queryable source, allowing for flexible and efficient sorting of data at runtime.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the <see cref="IQueryable{T}"/>.</typeparam>
    /// <param name="source">The <see cref="IQueryable{T}"/> to apply the ordering to.</param>
    /// <param name="orderBy">A comma-separated string specifying the properties to order by and their respective directions (e.g., "Name desc, Age").</param>
    /// <param name="defaultSortDirection">A string specifying the default sort direction ("asc" or "desc") to use when no direction is provided for a property.</param>
    /// <returns>A new <see cref="IQueryable{T}"/> with the applied ordering.</returns>
    public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> source, string orderBy, string? defaultSortDirection = null)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
        {
            return source;
        }

        if (string.IsNullOrWhiteSpace(defaultSortDirection))
        {
            defaultSortDirection = "asc";
        }

        // Sanitize the default once
        bool defaultIsDescending = defaultSortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        var sortExpressions = orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var currentExpression = source.Expression;

        for (int i = 0; i < sortExpressions.Length; i++)
        {
            var parts = sortExpressions[i].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var propertyPath = parts[0];

            // Logic: If a direction was provided, use it. Otherwise, use the default parameter.
            bool isDescending = parts.Length > 1
                ? parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
                : defaultIsDescending;

            var parameter = Expression.Parameter(typeof(T), "x");

            var propertyAccess = propertyPath.Split('.')
                .Aggregate((Expression)parameter, Expression.PropertyOrField);

            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = i == 0
                ? (isDescending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy))
                : (isDescending ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy));

            currentExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                [typeof(T), propertyAccess.Type],
                currentExpression,
                Expression.Quote(lambda)
            );
        }

        return source.Provider.CreateQuery<T>(currentExpression);
    }
}