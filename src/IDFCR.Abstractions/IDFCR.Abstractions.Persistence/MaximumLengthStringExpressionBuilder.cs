using System.Linq.Expressions;
using System.Reflection;

namespace IDFCR.Abstractions.Persistence;

/// <summary>
/// Builds an expression that projects the runtime character lengths of all string
/// properties in <typeparamref name="T"/> into a dictionary, equivalent to SQL
/// <c>LEN(column)</c> per column. Apply <c>Max</c> across rows to achieve
/// <c>MAX(LEN(column))</c> per column.
/// </summary>
/// <typeparam name="T">The model type to inspect.</typeparam>
public static class MaximumLengthStringExpressionBuilder<T>
{
    private static readonly PropertyInfo LengthProperty =
        typeof(string).GetProperty(nameof(string.Length))!;

    private static readonly MethodInfo AddMethod =
        typeof(Dictionary<string, int>).GetMethod(nameof(Dictionary<,>.Add),
            [typeof(string), typeof(int)])!;

    private static readonly Expression NullString =
        Expression.Constant(null, typeof(string));

    /// <summary>
    /// Builds an <see cref="Expression{TDelegate}"/> that, when compiled and invoked
    /// with a <typeparamref name="T"/> instance, returns a <see cref="Dictionary{TKey,TValue}"/>
    /// mapping each public string property name to <c>instance.Property?.Length ?? 0</c>.
    /// </summary>
    public static Expression<Func<T, Dictionary<string, int>>> BuildExpression()
    {
        var parameter = Expression.Parameter(typeof(T), "t");

        var elementInits = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(string))
            .Select(p =>
            {
                var propertyAccess = Expression.Property(parameter, p);

                // t.Property != null ? t.Property.Length : 0
                var nullSafeLength = Expression.Condition(
                    Expression.Equal(propertyAccess, NullString),
                    Expression.Constant(0),
                    Expression.Property(propertyAccess, LengthProperty));

                return Expression.ElementInit(AddMethod, Expression.Constant(p.Name), nullSafeLength);
            })
            .ToList();

        var newDict = Expression.New(typeof(Dictionary<string, int>));

        Expression body = elementInits.Count > 0
            ? Expression.ListInit(newDict, elementInits)
            : newDict;

        return Expression.Lambda<Func<T, Dictionary<string, int>>>(body, parameter);
    }
}
