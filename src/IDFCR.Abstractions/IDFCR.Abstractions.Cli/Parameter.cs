namespace IDFCR.Abstractions.Cli;

/// <summary>
/// Represents a command-line parameter with a key, optional value, and flag indicator.
/// </summary>
/// <param name="Key">The parameter key or name (e.g., "--option" or "-o").</param>
/// <param name="Value">The optional value associated with the parameter. Null for flag parameters.</param>
/// <param name="IsFlag">Indicates whether this parameter is a boolean flag (true) or a key-value pair (false).</param>
public record Parameter(string Key, string? Value = null, bool IsFlag = false);
