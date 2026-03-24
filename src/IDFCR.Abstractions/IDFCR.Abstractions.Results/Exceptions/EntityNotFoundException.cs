namespace IDFCR.Abstractions.Results.Exceptions;

/// <summary>
/// Exception thrown when an entity cannot be found.
/// </summary>
public class EntityNotFoundException(string entityType,
    object id, Exception? innerException = null)
    : EntityExceptionBase(entityType, $"Unable to find entity of {{entity-type}} with id '{id}'", innerException), IExposableException
{
    /// <summary>
    /// Creates a new not-found exception for the supplied entity type.
    /// </summary>
    public EntityNotFoundException(Type entityType, object id, Exception? innerException = null)
        : this(entityType.Name, id, innerException)
    {

    }

    string IExposableException.Message => FormatMessage("Unable to find entity of {entity-type}");
    string? IExposableException.Details => FormatMessage("Id: '{id}' in '{entity-type}'", b => b.AddOrUpdate("id", id.ToString() ?? string.Empty));
}
