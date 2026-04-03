namespace IDFCR.Abstractions.Cli;

/// <summary>
/// Represents the options for the prompt greeter.
/// </summary>
/// <param name="EnablePromptGreeting">Indicates whether the prompt greeting is enabled.</param>
/// <param name="MorningStartTime">The start time for the morning period.</param>
/// <param name="AfternoonStartTime">The start time for the afternoon period.</param>
/// <param name="EveningStartTime">The start time for the evening period.</param>
public record PromptGreeterOptions(bool EnablePromptGreeting = true, TimeOnly? MorningStartTime = default, TimeOnly? AfternoonStartTime = default, TimeOnly? EveningStartTime = default) : IPromptGreeterOptions
{
    /// <summary>
    /// Gets the original prompt template. This property allows developers to specify a custom template for generating greeting prompts. The template can include placeholders that will be replaced with dynamic values, such as the current time or the time of day label (e.g., "morning", "afternoon", "evening"). By providing a customizable prompt template, developers can tailor the greeting messages to better suit the tone and style of their CLI applications, creating a more personalized user experience based on the time of day.
    /// </summary>
    public string? OriginalPromptTemplate { get; private set; }
    /// <summary>
    /// Gets the default prompt template. This property allows developers to specify a custom template for generating greeting prompts. The template can include placeholders that will be replaced with dynamic values, such as the current time or the time of day label (e.g., "morning", "afternoon", "evening"). By providing a customizable prompt template, developers can tailor the greeting messages to better suit the tone and style of their CLI applications, creating a more personalized user experience based on the time of day.
    /// </summary>
    public string? DefaultPromptTemplate { get; init; }
    /// <summary>
    /// Gets the label for the morning greeting. This property allows developers to specify a custom label that will be used when generating morning greeting prompts. By providing a customizable label, developers can tailor the greeting messages to better suit the tone and style of their CLI applications, creating a more personalized user experience during the morning hours.
    /// </summary>
    public string? MorningLabel { get; init; }
    /// <summary>
    /// Gets the label for the afternoon greeting. This property allows developers to specify a custom label that will be used when generating afternoon greeting prompts. By providing a customizable label, developers can tailor the greeting messages to better suit the tone and style of their CLI applications, creating a more personalized user experience during the afternoon hours.
    /// </summary>
    public string? AfternoonLabel { get; init; }
    /// <summary>
    /// Gets the label for the evening greeting. This property allows developers to specify a custom label that will be used when generating evening greeting prompts. By providing a customizable label, developers can tailor the greeting messages to better suit the tone and style of their CLI applications, creating a more personalized user experience during the evening hours.
    /// </summary>
    public string? EveningLabel { get; init; }
    /// <summary>
    /// Defines a static readonly instance of the <see cref="PromptGreeterOptions"/> class with default values. This instance can be used as a default configuration for the prompt greeter, providing a convenient way to access commonly used settings without needing to create new instances of the options class. The default values for the properties are set to enable the prompt greeting and specify typical start times for morning, afternoon, and evening periods, allowing for a standard greeting behavior based on the time of day. By using this default instance, developers can easily implement prompt greetings in their applications without needing to configure custom options unless specific behavior is desired.
    /// </summary>
    public static readonly PromptGreeterOptions WesternDefault = new(true, new TimeOnly(00, 30), new TimeOnly(12, 00), new TimeOnly(17, 00))
    {
        DefaultPromptTemplate = "Good {TimeOfDay}. The current time is {CurrentTime}.",
        MorningLabel = "morning",
        AfternoonLabel = "afternoon",
        EveningLabel = "evening"
    };

    /// <summary>
    /// Defines a method to combine the current instance of <see cref="PromptGreeterOptions"/> with another instance of <see cref="IPromptGreeterOptions"/>. This method allows for merging the properties of two options instances, where the values from the provided options will override the current instance's values if they are not null or default. This is particularly useful for scenarios where developers want to create a new configuration based on an existing one, allowing for flexible and dynamic configuration of the prompt greeter behavior in CLI applications. By using this method, developers can easily customize their greeting prompts while still retaining default values where necessary.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public IPromptGreeterOptions Combine(IPromptGreeterOptions options)
    {
        if (options == null)
        {
            return this;
        }

        return this with
        {
            OriginalPromptTemplate = DefaultPromptTemplate,
            DefaultPromptTemplate = string.IsNullOrWhiteSpace(options.DefaultPromptTemplate) ? DefaultPromptTemplate : options.DefaultPromptTemplate,
            MorningLabel = string.IsNullOrWhiteSpace(options.MorningLabel) ? MorningLabel : options.MorningLabel,
            AfternoonLabel = string.IsNullOrWhiteSpace(options.AfternoonLabel) ? AfternoonLabel : options.AfternoonLabel,
            EveningLabel = string.IsNullOrWhiteSpace(options.EveningLabel) ? EveningLabel : options.EveningLabel,
            EnablePromptGreeting = options.EnablePromptGreeting,
            MorningStartTime = options.MorningStartTime ?? MorningStartTime,
            AfternoonStartTime = options.AfternoonStartTime ?? AfternoonStartTime,
            EveningStartTime = options.EveningStartTime ?? EveningStartTime
        };
    }
}