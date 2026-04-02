namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Represents a static class that defines constant fields used for various purposes within the CLI extensions. This class serves as a centralized location for defining constant values that can be referenced throughout the CLI extension codebase, providing consistency and ease of maintenance. The fields defined in this class can be used for specific scenarios, such as indicating special conditions or providing default values for certain parameters. By using constant fields, the code can avoid magic numbers and improve readability by giving meaningful names to specific values used in the CLI extensions.
/// </summary>
public static class Fields {
    /// <summary>
    /// Defines a constant field that represents a special value used to indicate that a positional parameter should be ignored. This field can be used in scenarios where a positional parameter is not applicable or should be skipped, allowing for more flexible handling of command-line arguments. By using this constant, the code can clearly communicate the intention to ignore a positional parameter, improving readability and maintainability when processing command-line input in the CLI extensions.
    /// </summary>
    public const int IgnorePositionalParameter = -1;
}
