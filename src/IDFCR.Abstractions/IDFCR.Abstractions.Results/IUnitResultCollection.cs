namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a unit result whose payload is a collection.
/// </summary>
/// <typeparam name="TResult">The element type.</typeparam>
public interface IUnitResultCollection<TResult> : IUnitResult<IEnumerable<TResult>?>
{
}
