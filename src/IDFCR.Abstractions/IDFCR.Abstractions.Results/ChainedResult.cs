namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a utility class for creating builders that can be used to construct chained unit results, providing a convenient way to create complex result chains by adding individual unit results and then building a final chained unit result that combines the information from all the added results. This class serves as a factory for obtaining instances of IChainedResultBuilder, which can be used to build chained unit results in a flexible and modular manner.
/// </summary>
public static class ChainedResult
{
    /// <summary>
    /// Creates a new instance of a chained result builder, which can be used to construct a chained unit result by adding individual unit results and then building a final chained unit result that combines the information from all the added results.
    /// </summary>
    /// <returns>An instance of IChainedResultBuilder.</returns>
    public static IChainedResultBuilder NewBuilder() => new DefaultChainedResultBuilder();
}
