namespace IDFCR.Abstractions.Cli.Prompts;

/// <summary>
/// Represents a service responsible for generating greeting prompts based on the time of day and provided options. The <see cref="IPromptGreeter"/> interface defines methods for generating greeting prompts, allowing developers to create customized greetings that can be used in CLI applications. The implementation of this interface can utilize the options defined in <see cref="IPromptGreeterOptions"/> to determine the appropriate greeting based on the current time and the specified configuration, enhancing the user experience by providing contextually relevant greetings. By implementing this interface, developers can easily integrate dynamic greeting functionality into their CLI applications, creating a more engaging and personalized user experience.
/// </summary>
public interface IPromptGreeter
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
