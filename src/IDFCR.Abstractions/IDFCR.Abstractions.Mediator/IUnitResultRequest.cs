using IDFCR.Abstractions.Results;
using MediatR;

namespace IDFCR.Abstractions.Mediator;
/// <summary>
/// Represents a request that returns a unit result, which indicates the success or failure of an operation without returning any data. 
/// </summary>
public interface IUnitResultRequest : IRequest<IUnitResult>
{

}

/// <summary>
/// Represents a request that returns a unit result with a value of type T, which indicates the success or failure of an operation along with the associated data.
/// </summary>
/// <typeparam name="T">The type of result returned when successful</typeparam>
public interface IUnitResultRequest<T> : IRequest<IUnitResult<T>>
{

}