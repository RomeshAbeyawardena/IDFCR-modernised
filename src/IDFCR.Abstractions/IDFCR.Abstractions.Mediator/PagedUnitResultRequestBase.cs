using IDFCR.Abstractions.Results;
using MediatR;

namespace IDFCR.Abstractions.Mediator;

/// <summary>
/// Represents a base record for a request that returns a paged collection of unit results with values of type T. It provides a convenient way to create specific requests by inheriting from this base record and passing the necessary paging information through the constructor.
/// </summary>
/// <typeparam name="T">The type of result returned when successful</typeparam>
public abstract record PagedUnitResultRequestBase<T>() : PagedQuery, IUnitPagedResultRequest<T>
{
    /// <summary>
    /// Initialises a new instance of the <see cref="PagedUnitResultRequestBase{T}"/> record with the specified paging information from an <see cref="IPagedQuery"/>. This constructor allows you to easily create a paged unit result request by passing an existing paged query, which can be useful for scenarios where you want to reuse paging parameters across different requests.
    /// </summary>
    /// <param name="pagedQuery">The paged query to copy paged parameters</param>
    protected PagedUnitResultRequestBase(IPagedQuery pagedQuery) : this()
    {
        base.PageIndex = pagedQuery.PageIndex;
        base.PageSize = pagedQuery.PageSize;
    }
}