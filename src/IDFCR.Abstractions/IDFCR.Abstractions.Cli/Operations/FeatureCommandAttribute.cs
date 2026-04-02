namespace IDFCR.Abstractions.Cli.Operations;

/// <summary>
/// Represents an attribute that can be applied to a class to indicate
/// that the class is associated with a specific feature command.
/// </summary>
/// <param name="prefix">The prefix of the feature command.</param>
/// <param name="key">The key of the feature command.</param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class FeatureCommandAttribute(string prefix, string key) : Attribute
{
    /// <summary>
    /// Gets the prefix of the feature command. This is a required property that must be provided when creating an instance of FeatureCommandAttribute. The prefix is used to group related feature commands together and can be used to identify the category or context of the command.
    /// </summary>
    public string Prefix { get; } = prefix;
    /// <summary>
    /// Gets the key of the feature command. This is a required property that must be provided when creating an instance of FeatureCommandAttribute. The key is used to uniquely identify the specific feature command within the context of the prefix and can be used to execute or reference the command in a CLI application.
    /// </summary>
    public string Key { get; } = key;

    /// <summary>
    /// Defines a string representation of the FeatureCommandAttribute, which combines the Prefix and Key properties in the format "Prefix-Key". This method is overridden from the base Object class to provide a meaningful string representation of the attribute, which can be useful for logging, debugging, or displaying information about the feature command in a CLI application.
    /// </summary>
    /// <returns>A string representation of the FeatureCommandAttribute.</returns>
    public override string ToString()
    {
        return $"{Prefix}-{Key}";
    }
}
