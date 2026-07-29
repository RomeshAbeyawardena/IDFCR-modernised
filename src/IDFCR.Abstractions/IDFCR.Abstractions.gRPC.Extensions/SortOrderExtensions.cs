using IDFCR.Abstractions.Metadata;
using V1Common = IDFCR.Abstractions.GRPC.Contracts.Common.V1;
namespace IDFCR.Abstractions.GRPC.Extensions;

/// <summary>
/// Defines extension methods for converting between gRPC contract types and common types related to sorting specifications. These extension methods facilitate the transformation of sorting specifications between the gRPC contract types used in communication with gRPC services and the common types used within the application, allowing for seamless integration and interoperability between different layers of the application architecture when handling sorting logic in requests and responses.
/// </summary>
public static class SortOrderExtensions
{
    /// <summary>
    /// Maps the sort fields from a gRPC contract OrderSet to a target structured ordered request of type TStructuredOrderedRequest. This method maps the sort specifications from the gRPC contract to the common type, allowing for consistent handling of sorting logic across different layers of the application.
    /// </summary>
    /// <typeparam name="TStructuredOrderedRequest">The type of the target structured ordered request.</typeparam>
    /// <param name="orderedRequest">The gRPC contract OrderSet containing the sort specifications.</param>
    /// <param name="target">The target structured ordered request to populate with the mapped sort fields.</param>
    /// <returns>The target structured ordered request with the mapped sort fields.</returns>
    public static TStructuredOrderedRequest MapSortFields<TStructuredOrderedRequest>(this V1Common.OrderSet orderedRequest, TStructuredOrderedRequest target)
        where TStructuredOrderedRequest : StructuredOrderedRequestBase, new()
    {

        target.SortedFields = orderedRequest.Sorts.Select(sort => new DefaultSort
        {
            Field = sort.FieldName,
            Order = sort.HasOrder ? sort.Order : (int)sort.Direction
        });

        return target;
    }

    /// <summary>
    /// Defines an extension method to convert an ISort from the common type to the gRPC contract SortOrder type.
    /// </summary>
    /// <param name="sort">The common ISort to convert.</param>
    /// <returns>A <see cref="V1Common.SortOrder"/> representing the converted sort order.</returns>
    public static V1Common.SortOrder From(this ISort sort)
    {
        V1Common.SortOrder result = new()
        {
            FieldName = sort.Field,
            Order = sort.Order,
            Direction = Enum.Parse<V1Common.OrderDirection>(sort.Direction.ToString()),
        };

        return result;
    }

    /// <summary>
    /// Defines an extension method to convert a gRPC contract SortOrder from the gRPC contract type to the common ISort type.
    /// </summary>
    /// <param name="sort">The gRPC contract SortOrder to convert.</param>
    /// <returns>An <see cref="ISort"/> representing the converted sort order.</returns>
    public static ISort From(this V1Common.SortOrder sort)
    {
        DefaultSort sortOrder = new()
        {
            Field = sort.FieldName,
            Order = sort.Order
        };

        return sortOrder;
    }
}
