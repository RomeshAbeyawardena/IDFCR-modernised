namespace IDFCR.GRPC.Client.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="object"/> class.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Returns the value if it is not null or whitespace; otherwise, returns the specified default value.
    /// <code>
    /// StoryId = storyId.UseValueOrDefault(element.StoryId),
    /// </code>
    /// <para>If storyId is null or whitespace, the default value from element.StoryId will be used.</para>
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="defaultValue">The default value to return if the value is null or whitespace.</param>
    /// <returns>The original value if it is not null or whitespace; otherwise, the default value.</returns>
    public static object? UseValueOrDefault(this object? value, object? defaultValue)
    {
        if (value is null)
        {
            return defaultValue;
        }

        if (string.IsNullOrWhiteSpace(value.ToString()))
        {
            return defaultValue;
        }

        return value;
    }
}
