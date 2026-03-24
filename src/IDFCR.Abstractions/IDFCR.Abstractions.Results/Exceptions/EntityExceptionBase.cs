using IDFCR.Abstractions.Builders;
using IDFCR.Abstractions.Results.Extensions;

namespace IDFCR.Abstractions.Results.Exceptions;

/// <summary>
/// Base type for entity-related exceptions that support formatted messages.
/// </summary>
public abstract class EntityExceptionBase : Exception
{
    /// <summary>
    /// Builds a dictionary of template values.
    /// </summary>
    public static IReadOnlyDictionary<string, string> ConfigureKeyValues(Action<IDictionaryBuilder<string, string>> action)
    {
        var builder = new DictionaryBuilder<string, string>();
        action(builder);
        return builder.Build();
    }

    private static string Format(string sourceMessage, IReadOnlyDictionary<string, string> values)
    {
        var message = sourceMessage;
        foreach (var (key, value) in values)
        {
            var k = key.ToKebabCase();
            message = message.Replace($"{{{k}}}", value);
        }
        return message;
    }

    /// <summary>
    /// Formats a message using the entity type and optional replacement values.
    /// </summary>
    protected static string FormatMessage(string? sourceMessage, string entityType, IReadOnlyDictionary<string, string>? values = null)
    {
        if (string.IsNullOrWhiteSpace(sourceMessage))
        {
            return string.Empty;
        }

        var vals = values == null
            ? []
            : new Dictionary<string, string>(values);

        vals.TryAdd("entity-type", entityType);

        return Format(sourceMessage, vals);
    }

    private readonly string _message;

    /// <summary>
    /// Creates a new entity exception.
    /// </summary>
    protected EntityExceptionBase(string entityType, string message, Exception? innerException, params string[] affixesToRemove) : base(null, innerException)
    {
        EntityType = entityType.ReplaceAll(string.Empty, StringComparison.InvariantCultureIgnoreCase, [.. affixesToRemove.Prepend("dto")
            .Prepend("db").Distinct()]);
        _message = FormatMessage(message, EntityType);
    }

    /// <summary>
    /// Formats a message using this exception's entity type.
    /// </summary>
    protected string FormatMessage(string? sourceMessage, IReadOnlyDictionary<string, string>? values = null)
    {
        return FormatMessage(sourceMessage, EntityType, values);
    }

    /// <summary>
    /// Formats a message using values configured through a dictionary builder.
    /// </summary>
    protected string FormatMessage(string? sourceMessage, Action<IDictionaryBuilder<string, string>> action)
    {
        return FormatMessage(sourceMessage, EntityType, ConfigureKeyValues(action));
    }

    /// <summary>
    /// Creates a new entity exception from the supplied entity type.
    /// </summary>
    protected EntityExceptionBase(Type entityType, string message, Exception innerException, params string[] affixesToRemove)
        : this(entityType.Name, message, innerException, affixesToRemove)
    {
    }

    /// <summary>
    /// Gets the entity type associated with the exception.
    /// </summary>
    public string EntityType { get; }

    /// <summary>
    /// Gets the formatted message.
    /// </summary>
    public override string Message => _message;
}
