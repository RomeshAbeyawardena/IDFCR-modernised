namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a unit result that is part of a chain of results, allowing access to the previous result in the chain.
/// </summary>
public interface IChainedUnitResult : IUnitResult
{
    /// <summary>
    /// Gets the current unit result in the chain, allowing access to the information and status of the current result. This property represents the most recent result in the chain, while the Last property allows access to the previous result's information and status.
    /// </summary>
    IUnitResult Current { get; }
    /// <summary>
    /// Gets the last unit result in the chain, allowing access to the previous result's information and status.
    /// </summary>
    IUnitResult Last { get; }
}

/// <summary>
/// Represents a unit result that is part of a chain of results, allowing access to the previous result in the chain and containing a value of type T.
/// </summary>
/// <typeparam name="T">The type of the value contained in the result.</typeparam>
public interface IChainedUnitResult<T> : IUnitResult<T>, IChainedUnitResult
{
    /// <summary>
    /// Gets the current unit result in the chain, allowing access to the information and status of the current result. This property represents the most recent result in the chain, while the Last property allows access to the previous result's information and status.
    /// </summary>
    new IUnitResult<T> Current { get; }
}

