namespace IDFCR.Abstractions.Cli;

/// <summary>
/// Represents a builder for configuring the options of the prompt greeter, which is responsible for generating greeting prompts based on the time of day. This interface defines methods that allow developers to configure the prompt greeter options using a generic type parameter, direct configuration values, or by using predefined defaults. By implementing this interface, developers can customize the behavior of the prompt greeter to provide appropriate greetings based on the current time, enhancing the user experience in CLI applications that utilize this functionality. The builder pattern allows for a fluent and flexible configuration of the prompt greeter options, making it easy to set up and customize as needed.
/// </summary>
public interface IPromptGreeterOptionsBuilder
{
    /// <summary>
    /// Defines a method for configuring the prompt greeter options using a generic type parameter. This method allows developers to specify a custom implementation of the IPromptGreeterOptions interface, which can be used to provide specific configuration values for the prompt greeter. By using a generic type parameter, developers can easily create and use different configurations for the prompt greeter, enabling greater flexibility and customization in how greeting prompts are generated based on the time of day. This approach also supports future configuration and database integration, allowing for dynamic retrieval of configuration values as needed.
    /// </summary>
    /// <typeparam name="T">The type of the custom prompt greeter options implementation.</typeparam>
    /// <param name="options">The options to configure the prompt greeter with.</param>
    /// <returns>The current instance of the IPromptGreeterOptionsBuilder for fluent configuration.</returns>
    IPromptGreeterOptionsBuilder Configure<T>(T options)
        where T : IPromptGreeterOptions;
    /// <summary>
    /// Defines a method for configuring the prompt greeter options using direct configuration values. This method allows developers to specify individual configuration parameters such as the default prompt template, labels for different times of the day (morning, afternoon, evening), whether to enable the prompt greeting, and the start times for each period of the day. By providing these parameters directly, developers can easily customize the behavior of the prompt greeter without needing to create a separate implementation of the IPromptGreeterOptions interface. This approach offers a straightforward way to set up the prompt greeter options while still supporting future configuration and database integration for dynamic retrieval of these values as needed.
    /// </summary>
    /// <param name="defaultPromptTemplate">The default template for the prompt greeting.</param>
    /// <param name="morningLabel">The label for the morning period.</param>
    /// <param name="afternoonLabel">The label for the afternoon period.</param>
    /// <param name="eveningLabel">The label for the evening period.</param>
    /// <param name="enablePromptGreeting">A value indicating whether to enable the prompt greeting.</param>
    /// <param name="morningStartTime">The start time for the morning period.</param>
    /// <param name="afternoonStartTime">The start time for the afternoon period.</param>
    /// <param name="eveningStartTime">The start time for the evening period.</param>
    /// <returns>The current instance of the IPromptGreeterOptionsBuilder for fluent configuration.</returns>
    IPromptGreeterOptionsBuilder Configure(bool enablePromptGreeting = true,
        string? defaultPromptTemplate = null,
        string? morningLabel = null,
        string? afternoonLabel = null,
        string? eveningLabel = null,
        TimeOnly? morningStartTime = null,
        TimeOnly? afternoonStartTime = null,
        TimeOnly? eveningStartTime = null);
    /// <summary>
    /// Defines a method for configuring the prompt greeter options using predefined defaults. This method allows developers to specify a set of default configurations for the prompt greeter based on a predefined enumeration (PromptGreeterDefaults). By selecting a default configuration, developers can quickly set up the prompt greeter with commonly used settings that are appropriate for different cultural or regional contexts (e.g., Western). This approach provides a convenient way to apply standard configurations while still allowing for further customization if needed. Additionally, it supports future configuration and database integration by enabling the use of predefined defaults that can be easily extended or modified as necessary.
    /// </summary>
    /// <param name="greeterDefaults"></param>
    /// <returns></returns>
    IPromptGreeterOptionsBuilder UseDefault(PromptGreeterDefaults greeterDefaults = PromptGreeterDefaults.Western);
    /// <summary>
    /// Builds the configured prompt greeter options and returns an instance of IPromptGreeterOptions. This method finalizes the configuration process and provides the resulting options that can be used by the prompt greeter to generate greeting prompts based on the time of day. By calling this method, developers can obtain a fully configured instance of IPromptGreeterOptions that reflects the settings specified through the various configuration methods provided by the builder. This allows for a seamless integration of the prompt greeter options into CLI applications, enabling dynamic generation of greeting prompts that enhance the user experience based on the current time.
    /// </summary>
    /// <returns>The configured instance of IPromptGreeterOptions.</returns>
    IPromptGreeterOptions Build();
}

/// <summary>
/// Represents the default implementation of the IPromptGreeterOptionsBuilder interface, which provides methods for configuring the options of the prompt greeter. This class allows developers to set up the prompt greeter options using a generic type parameter, direct configuration values, or by using predefined defaults. By implementing this builder, developers can easily customize the behavior of the prompt greeter to generate appropriate greetings based on the time of day, enhancing the user experience in CLI applications that utilize this functionality. The builder pattern used in this implementation enables a fluent and flexible configuration process, making it straightforward to set up and customize the prompt greeter options as needed.
/// </summary>
public class DefaultPromptGreeterOptionsBuilder : IPromptGreeterOptionsBuilder
{
    private IPromptGreeterOptions? _options = null;
    /// <summary>
    /// Defines a method for configuring the prompt greeter options using a generic type parameter. This method allows developers to specify a custom implementation of the IPromptGreeterOptions interface, which can be used to provide specific configuration values for the prompt greeter. By using a generic type parameter, developers can easily create and use different configurations for the prompt greeter, enabling greater flexibility and customization in how greeting prompts are generated based on the time of day. This approach also supports future configuration and database integration, allowing for dynamic retrieval of configuration values as needed.
    /// </summary>
    /// <typeparam name="T">The type of the options to configure.</typeparam>
    /// <param name="options">The options to configure the prompt greeter with.</param>
    /// <returns>The current instance of IPromptGreeterOptionsBuilder.</returns>
    public IPromptGreeterOptionsBuilder Configure<T>(T options) where T : IPromptGreeterOptions
    {
        if (_options is not null)
        {
            _options = _options.Combine(options);
        }
        else
        {
            _options = options;
        }

        return this;
    }

    /// <summary>
    /// Defines a method for configuring the prompt greeter options using direct configuration values. This method allows developers to specify individual configuration parameters such as the default prompt template, labels for different times of the day (morning, afternoon, evening), whether to enable the prompt greeting, and the start times for each period of the day. By providing these parameters directly, developers can easily customize the behavior of the prompt greeter without needing to create a separate implementation of the IPromptGreeterOptions interface. This approach offers a straightforward way to set up the prompt greeter options while still supporting future configuration and database integration for dynamic retrieval of these values as needed.
    /// </summary>
    /// <param name="defaultPromptTemplate">The default template for the prompt.</param>
    /// <param name="morningLabel">The label for the morning period.</param>
    /// <param name="afternoonLabel">The label for the afternoon period.</param>
    /// <param name="eveningLabel">The label for the evening period.</param>
    /// <param name="enablePromptGreeting">Indicates whether to enable the prompt greeting.</param>
    /// <param name="morningStartTime">The start time for the morning period.</param>
    /// <param name="afternoonStartTime">The start time for the afternoon period.</param>
    /// <param name="eveningStartTime">The start time for the evening period.</param>
    /// <returns>The current instance of IPromptGreeterOptionsBuilder.</returns>
    public IPromptGreeterOptionsBuilder Configure(bool enablePromptGreeting,
        string? defaultPromptTemplate, 
        string? morningLabel, 
        string? afternoonLabel, 
        string? eveningLabel,
        TimeOnly? morningStartTime, 
        TimeOnly? afternoonStartTime, 
        TimeOnly? eveningStartTime)
    {
        return Configure(new PromptGreeterOptions
        {
            DefaultPromptTemplate = defaultPromptTemplate,
            MorningLabel = morningLabel,
            AfternoonLabel = afternoonLabel,
            EveningLabel = eveningLabel,
            EnablePromptGreeting = enablePromptGreeting,
            MorningStartTime = morningStartTime,
            AfternoonStartTime = afternoonStartTime,
            EveningStartTime = eveningStartTime
        });
    }

    /// <summary>
    /// Uses predefined defaults to configure the prompt greeter options. This method allows developers to specify a set of default configurations for the prompt greeter based on a predefined enumeration (PromptGreeterDefaults). By selecting a default configuration, developers can quickly set up the prompt greeter with commonly used settings that are appropriate for different cultural or regional contexts (e.g., Western). This approach provides a convenient way to apply standard configurations while still allowing for further customization if needed. Additionally, it supports future configuration and database integration by enabling the use of predefined defaults that can be easily extended or modified as necessary.
    /// </summary>
    /// <param name="greeterDefaults"></param>
    /// <returns></returns>
    public IPromptGreeterOptionsBuilder UseDefault(PromptGreeterDefaults greeterDefaults = PromptGreeterDefaults.Western)
    {
        var options = greeterDefaults switch
        {
            PromptGreeterDefaults.Western => PromptGreeterOptions.WesternDefault,
            _ => new()
        };

        return Configure(options);
    }

    /// <summary>
    /// Builds the configured prompt greeter options and returns an instance of IPromptGreeterOptions. This method finalizes the configuration process and provides the resulting options that can be used by the prompt greeter to generate greeting prompts based on the time of day. By calling this method, developers can obtain a fully configured instance of IPromptGreeterOptions that reflects the settings specified through the various configuration methods provided by the builder. This allows for a seamless integration of the prompt greeter options into CLI applications, enabling dynamic generation of greeting prompts that enhance the user experience based on the current time.
    /// </summary>
    /// <returns></returns>
    public IPromptGreeterOptions Build() => _options ?? throw new InvalidOperationException("Prompt greeter options have not been configured.");
}