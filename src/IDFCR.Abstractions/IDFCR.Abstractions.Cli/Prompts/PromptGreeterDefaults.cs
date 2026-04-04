namespace IDFCR.Abstractions.Cli.Prompts;

/// <summary>
/// Enumerates the default configurations for the prompt greeter, which is responsible for generating greeting prompts based on the time of day. This enumeration currently includes a single default configuration labeled "Western", which can be used as a baseline for generating greetings in a Western cultural context. By using this enumeration, developers can easily reference and utilize predefined configurations for the prompt greeter, allowing for consistent and culturally appropriate greeting prompts in CLI applications that utilize this functionality. Future configurations can be added to this enumeration as needed to support different cultural contexts or user preferences for greeting prompts.
/// </summary>
public enum PromptGreeterDefaults
{
    /// <summary>
    /// Represents the Western region or style within the enumeration.
    /// </summary>
    Western = 0
}
