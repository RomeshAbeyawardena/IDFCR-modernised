namespace IDFCR.Abstractions.Cli;

/// <summary>
/// Represents the default implementation of the <see cref="IPromptGreeter"/> interface, responsible for generating greeting prompts based on the time of day and provided options. This implementation utilizes the options defined in <see cref="IPromptGreeterOptions"/> to determine the appropriate greeting based on the current time and the specified configuration. The generated greeting prompts can include dynamic values such as the current time and the time of day label, allowing for a personalized user experience in CLI applications that utilize this functionality. By using this default implementation, developers can easily integrate dynamic greeting functionality into their CLI applications, creating a more engaging and personalized user experience without needing to implement custom logic for generating greetings based on time of day.
/// </summary>
/// <param name="options"></param>
public sealed class DefaultPromptGreeter(IPromptGreeterOptions options) : IPromptGreeter
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