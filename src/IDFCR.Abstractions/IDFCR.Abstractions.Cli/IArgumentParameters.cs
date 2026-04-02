namespace IDFCR.Abstractions.Cli;

/// <summary>
/// Represents a read-only collection of command-line argument parameters indexed by their key names.
/// </summary>
/// <remarks>
/// This interface extends <see cref="IReadOnlyDictionary{TKey, TValue}"/> to provide type-safe access 
/// to parsed command-line parameters, where each key corresponds to a parameter name and the value 
/// is a <see cref="Parameter"/> object containing the parameter's details.
/// </remarks>
public interface IArgumentParameters : IReadOnlyDictionary<string, Parameter>;
