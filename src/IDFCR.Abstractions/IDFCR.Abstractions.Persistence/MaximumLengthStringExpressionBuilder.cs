using System.Linq.Expressions;
using System.Reflection;

namespace IDFCR.Abstractions.Persistence;

/// <summary>
/// Builds an expression that projects the runtime character lengths of all string
/// properties in <typeparamref name="T"/> as a sequence of <see cref="KeyValuePair{TKey,TValue}"/>,
/// equivalent to SQL <c>LEN(column)</c> per column per row.
/// Aggregate with <c>SelectMany → GroupBy → Max</c> to achieve <c>MAX(LEN(column))</c>.
/// </summary>
/// <typeparam name="T">The model type to inspect.</typeparam>
public static class MaximumLengthStringExpressionBuilder<T>
{
    private static readonly PropertyInfo LengthProperty =
        typeof(string).GetProperty(nameof(string.Length))!;

    private static readonly ConstructorInfo KvpConstructor =
        typeof(KeyValuePair<string, int>).GetConstructor([typeof(string), typeof(int)])!;

    private static readonly Expression NullString =
        Expression.Constant(null, typeof(string));

    /// <summary>
    /// Builds an <see cref="Expression{TDelegate}"/> that, when compiled and invoked
    /// with a <typeparamref name="T"/> instance, returns an
    /// <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>
    /// mapping each public string property name to <c>instance.Property?.Length ?? 0</c>.
    /// </summary>
    public static Expression<Func<T, IEnumerable<KeyValuePair<string, int>>>> BuildExpression()
    {
        var parameter = Expression.Parameter(typeof(T), "t");

        var elements = typeof(T)
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

                return (Expression)Expression.New(KvpConstructor, Expression.Constant(p.Name), nullSafeLength);
            })
            .ToArray();

        var arrayInit = Expression.NewArrayInit(typeof(KeyValuePair<string, int>), elements);

        return Expression.Lambda<Func<T, IEnumerable<KeyValuePair<string, int>>>>(arrayInit, parameter);
    }
}
