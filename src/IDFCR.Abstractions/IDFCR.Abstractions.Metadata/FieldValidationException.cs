namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a custom exception that is thrown when one or more fields specified in a sorting request are not supported. This exception is used to indicate that the requested fields for sorting do not match the list of supported fields defined in the implementation of the <see cref="StructuredOrderedRequestBase"/> class. By throwing this exception, developers can provide clear feedback to clients or users of the API when they attempt to sort by fields that are not recognized or allowed, helping to prevent errors and ensure that sorting operations are performed only on valid and supported fields within the application.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public sealed class FieldValidationException(string message, Exception? innerException = null) : Exception(message, innerException)
{
    
}
