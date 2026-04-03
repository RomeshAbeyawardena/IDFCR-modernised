namespace IDFCR.Abstractions.Cli;

/// <summary>
/// Represents the options for configuring the prompt greeter, which is responsible for generating greeting prompts based on the time of day. This interface defines properties that allow developers to enable or disable the prompt greeting and specify the start times for different periods of the day, such as morning, afternoon, and evening. By implementing this interface, developers can customize the behavior of the prompt greeter to provide appropriate greetings based on the current time, enhancing the user experience in CLI applications that utilize this functionality.
/// </summary>
public interface IPromptGreeterOptions
{
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
    TimeOnly MorningStartTime { get; }
    /// <summary>
    /// Gets the start time for the afternoon period. This property defines the time at which the prompt greeter will begin generating afternoon greetings. By specifying this time, developers can ensure that users receive appropriate greetings during the afternoon hours, enhancing the user experience in CLI applications that utilize the prompt greeter. The value of this property can be customized to fit different definitions of what constitutes "afternoon" based on user preferences or cultural norms.
    /// </summary>
    TimeOnly AfternoonStartTime { get; }
    /// <summary>
    /// Gets the start time for the evening period. This property defines the time at which the prompt greeter will begin generating evening greetings. By specifying this time, developers can ensure that users receive appropriate greetings during the evening hours, enhancing the user experience in CLI applications that utilize the prompt greeter. The value of this property can be customized to fit different definitions of what constitutes "evening" based on user preferences or cultural norms.
    /// </summary>
    TimeOnly EveningStartTime { get; }
}

/// <summary>
/// Represents the options for the prompt greeter.
/// </summary>
/// <param name="EnablePromptGreeting">Indicates whether the prompt greeting is enabled.</param>
/// <param name="MorningStartTime">The start time for the morning period.</param>
/// <param name="AfternoonStartTime">The start time for the afternoon period.</param>
/// <param name="EveningStartTime">The start time for the evening period.</param>
public record PromptGreeterOptions(bool EnablePromptGreeting = true, TimeOnly MorningStartTime = default, TimeOnly AfternoonStartTime = default, TimeOnly EveningStartTime = default) : IPromptGreeterOptions
{
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
        DefaultPromptTemplate= "Good {TimeOfDay}. The current time is {CurrentTime}.",
        MorningLabel = "morning",
        AfternoonLabel = "afternoon",
        EveningLabel = "evening"
    };
}

/// <summary>
/// Represents a service responsible for generating greeting prompts based on the time of day and provided options. The <see cref="IPromptGreeter"/> interface defines methods for generating greeting prompts, allowing developers to create customized greetings that can be used in CLI applications. The implementation of this interface can utilize the options defined in <see cref="IPromptGreeterOptions"/> to determine the appropriate greeting based on the current time and the specified configuration, enhancing the user experience by providing contextually relevant greetings. By implementing this interface, developers can easily integrate dynamic greeting functionality into their CLI applications, creating a more engaging and personalized user experience.
/// </summary>
internal interface IPromptGreeter
{
    /// <summary>
    /// Defines a method to generate a greeting prompt based on the provided prompt template, time of day, and placeholder key-value pairs. This method takes an optional prompt template, the current time of day, and a dictionary of placeholder key-value pairs that can be used to replace placeholders in the prompt template. The method returns a generated greeting prompt with the placeholders replaced by their corresponding values from the dictionary, allowing for dynamic and contextually relevant greetings based on the time of day and provided options. By utilizing this method, developers can create customized greeting prompts that enhance the user experience in CLI applications.
    /// </summary>
    /// <param name="prompt">The optional prompt template to use for generating the greeting.</param>
    /// <param name="timeOfDay">The current time of day used to determine the appropriate greeting.</param>
    /// <param name="placeholderKeyValuePairs">A dictionary of placeholder key-value pairs to replace in the prompt template.</param>
    /// <returns>The generated greeting prompt with placeholders replaced by their corresponding values.</returns>
    string GenerateGreetingPrompt(string? prompt, DateTimeOffset timeOfDay, IDictionary<string, string> placeholderKeyValuePairs);

    /// <summary>
    /// Defines a method to generate a greeting prompt based on the current time of day and the provided options. This method takes the current time of day, an optional set of prompt greeter options, and an optional override prompt template. The method uses the options to determine the appropriate greeting based on the time of day and generates a greeting prompt accordingly. If an override prompt template is provided, it will be used instead of the default prompt template defined in the options. The generated greeting prompt can include dynamic values such as the current time and the time of day label, allowing for a personalized user experience in CLI applications that utilize this functionality.
    /// </summary>
    /// <param name="timeOfDay"></param>
    /// <param name="options"></param>
    /// <param name="overridePrompt"></param>
    /// <returns></returns>
    string GenerateGreetingPrompt(DateTimeOffset timeOfDay, IPromptGreeterOptions? options = null, string? overridePrompt = null);
}

/// <summary>
/// Represents the default implementation of the <see cref="IPromptGreeter"/> interface, responsible for generating greeting prompts based on the time of day and provided options. This implementation utilizes the options defined in <see cref="IPromptGreeterOptions"/> to determine the appropriate greeting based on the current time and the specified configuration. The generated greeting prompts can include dynamic values such as the current time and the time of day label, allowing for a personalized user experience in CLI applications that utilize this functionality. By using this default implementation, developers can easily integrate dynamic greeting functionality into their CLI applications, creating a more engaging and personalized user experience without needing to implement custom logic for generating greetings based on time of day.
/// </summary>
/// <param name="options"></param>
public class DefaultPromptGreeter(IPromptGreeterOptions options) : IPromptGreeter
{
    private string GetTimeOfDayLabel(DateTimeOffset timeOfDay)
    {
        var timeOnly = TimeOnly.FromDateTime(timeOfDay.DateTime);
        if (timeOnly >= options.MorningStartTime && timeOnly < options.AfternoonStartTime)
        {
            return options.MorningLabel ?? PromptGreeterOptions.WesternDefault.MorningLabel!;
        }
        else if (timeOnly >= options.AfternoonStartTime && timeOnly < options.EveningStartTime)
        {
            return options.AfternoonLabel ?? PromptGreeterOptions.WesternDefault.AfternoonLabel!;
        }
        else
        {
            return options.EveningLabel ?? PromptGreeterOptions.WesternDefault.EveningLabel!;
        }
    }
    /// <summary>
    /// Defines a method to generate a greeting prompt based on the provided prompt template, time of day, and placeholder key-value pairs. This method takes an optional prompt template, the current time of day, and a dictionary of placeholder key-value pairs that can be used to replace placeholders in the prompt template. The method returns a generated greeting prompt with the placeholders replaced by their corresponding values from the dictionary, allowing for dynamic and contextually relevant greetings based on the time of day and provided options. By utilizing this method, developers can create customized greeting prompts that enhance the user experience in CLI applications.
    /// </summary>
    /// <param name="prompt">The prompt template to use for generating the greeting.</param>
    /// <param name="timeOfDay">The current time of day.</param>
    /// <param name="placeholderKeyValuePairs">A dictionary of placeholder key-value pairs to replace in the prompt template.</param>
    /// <returns>The generated greeting prompt.</returns>
    public string GenerateGreetingPrompt(string? prompt, DateTimeOffset timeOfDay, IDictionary<string, string> placeholderKeyValuePairs)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return string.Empty;
        }

        var greeting = prompt;
        foreach (var kvp in placeholderKeyValuePairs)
        {
            greeting = greeting.Replace($"{{{kvp.Key}}}", kvp.Value);
        }
        return greeting;
    }

    /// <summary>
    /// Defines a method to generate a greeting prompt based on the current time of day and the provided options. This method takes the current time of day, an optional set of prompt greeter options, and an optional override prompt template. The method uses the options to determine the appropriate greeting based on the time of day and generates a greeting prompt accordingly. If an override prompt template is provided, it will be used instead of the default prompt template defined in the options. The generated greeting prompt can include dynamic values such as the current time and the time of day label, allowing for a personalized user experience in CLI applications that utilize this functionality.
    /// </summary>
    /// <param name="timeOfDay">The current time of day.</param>
    /// <param name="options">An optional set of prompt greeter options to customize the greeting.</param>
    /// <param name="overridePrompt">An optional override prompt template to use instead of the default template.</param>
    /// <returns>The generated greeting prompt.</returns>
    public string GenerateGreetingPrompt(DateTimeOffset timeOfDay, IPromptGreeterOptions? options = null, string? overridePrompt = null)
    {
        options ??= PromptGreeterOptions.WesternDefault;

        return GenerateGreetingPrompt(options.DefaultPromptTemplate ?? overridePrompt, timeOfDay, new Dictionary<string, string>
        {
            { "CurrentTime", timeOfDay.ToLocalTime().ToString("HH:mm") },
            { "TimeOfDay", GetTimeOfDayLabel(timeOfDay) }
        });
    }
}