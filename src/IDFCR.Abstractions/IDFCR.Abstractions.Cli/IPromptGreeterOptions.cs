namespace IDFCR.Abstractions.Cli;

/// <summary>
/// Represents the options for configuring the prompt greeter, which is responsible for generating greeting prompts based on the time of day. This interface defines properties that allow developers to enable or disable the prompt greeting and specify the start times for different periods of the day, such as morning, afternoon, and evening. By implementing this interface, developers can customize the behavior of the prompt greeter to provide appropriate greetings based on the current time, enhancing the user experience in CLI applications that utilize this functionality.
/// <para>This supports future configuration/database integration.</para>
/// </summary>
public interface IPromptGreeterOptions
{
    /// <summary>
    /// Combines the current options with another instance of <see cref="IPromptGreeterOptions"/>. This method allows for merging the settings from another options instance, enabling developers to easily integrate additional configurations or override existing ones. By calling this method, developers can ensure that the prompt greeter options are updated with the latest settings, providing a flexible way to manage and customize the behavior of the prompt greeter in CLI applications.
    /// </summary>
    /// <param name="options">The new options to combine with the current options.</param>
    IPromptGreeterOptions Combine(IPromptGreeterOptions options);
    /// <summary>
    /// Gets the default prompt template. This property allows developers to specify a custom template for generating greeting prompts. The template can include placeholders that will be replaced with dynamic values, such as the current time or the time of day label (e.g., "morning", "afternoon", "evening"). By providing a customizable prompt template, developers can tailor the greeting messages to better suit the tone and style of their CLI applications, creating a more personalized user experience based on the time of day.
    /// </summary>
    string? DefaultPromptTemplate { get; }
    /// <summary>
    /// Gets the label for the morning greeting. This property allows developers to specify a custom label that will be used when generating morning greeting prompts. By providing a customizable label, developers can tailor the greeting messages to better suit the tone and style of their CLI applications, creating a more personalized user experience during the morning hours.
    /// </summary>
    string? MorningLabel { get; }
    /// <summary>
    /// Gets the label for the afternoon greeting. This property allows developers to specify a custom label that will be used when generating afternoon greeting prompts. By providing a customizable label, developers can tailor the greeting messages to better suit the tone and style of their CLI applications, creating a more personalized user experience during the afternoon hours.
    /// </summary>
    string? AfternoonLabel { get; }
    /// <summary>
    /// Gets the label for the evening greeting. This property allows developers to specify a custom label that will be used when generating evening greeting prompts. By providing a customizable label, developers can tailor the greeting messages to better suit the tone and style of their CLI applications, creating a more personalized user experience during the evening hours.
    /// </summary>
    string? EveningLabel { get; }
    /// <summary>
    /// Gets a value indicating whether the prompt greeting is enabled. When set to true, the prompt greeter will generate greeting prompts based on the time of day. If set to false, the prompt greeter will not generate any greetings, allowing for a more neutral user experience. This property allows developers to easily enable or disable the greeting functionality as needed, providing flexibility in how the prompt greeter is used within CLI applications. By controlling this option, developers can tailor the user experience to suit different contexts or preferences regarding greeting prompts.
    /// </summary>
    bool EnablePromptGreeting { get; }
    /// <summary>
    /// Gets the start time for the morning period. This property defines the time at which the prompt greeter will begin generating morning greetings. By specifying this time, developers can ensure that users receive appropriate greetings during the morning hours, enhancing the user experience in CLI applications that utilize the prompt greeter. The value of this property can be customized to fit different definitions of what constitutes "morning" based on user preferences or cultural norms.
    /// </summary>
    TimeOnly? MorningStartTime { get; }
    /// <summary>
    /// Gets the start time for the afternoon period. This property defines the time at which the prompt greeter will begin generating afternoon greetings. By specifying this time, developers can ensure that users receive appropriate greetings during the afternoon hours, enhancing the user experience in CLI applications that utilize the prompt greeter. The value of this property can be customized to fit different definitions of what constitutes "afternoon" based on user preferences or cultural norms.
    /// </summary>
    TimeOnly? AfternoonStartTime { get; }
    /// <summary>
    /// Gets the start time for the evening period. This property defines the time at which the prompt greeter will begin generating evening greetings. By specifying this time, developers can ensure that users receive appropriate greetings during the evening hours, enhancing the user experience in CLI applications that utilize the prompt greeter. The value of this property can be customized to fit different definitions of what constitutes "evening" based on user preferences or cultural norms.
    /// </summary>
    TimeOnly? EveningStartTime { get; }
}
