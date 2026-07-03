using IDFCR.Abstractions.Results;
using MediatR;

namespace IDFCR.Abstractions.Mediator;

/// <summary>
/// Represents a request that returns a paged collection of unit results with values of type T, which indicates the success or failure of an operation along with the associated data in a paged collection format.
/// </summary>
/// <typeparam name="T">The type of result returned when successful</typeparam>
public interface IPagedUnitResultRequest<T> : IRequest<IPagedUnitResult<T>>, IPagedQuery
{

}
