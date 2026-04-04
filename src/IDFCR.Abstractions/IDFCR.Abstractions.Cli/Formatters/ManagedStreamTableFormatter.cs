using IDFCR.Abstractions.Cli.ManagedStreams;
using System.Collections;

namespace IDFCR.Abstractions.Cli.Formatters;

/// <summary>
/// Represents a formatter that writes formatted output to a managed stream in a tabular format. This class inherits from ManagedStreamFormatterBase, which provides common functionality for managing the output stream and accumulating formatted output in a StringBuilder. The ManagedStreamTableFormatter specifically implements the logic to format collections of data as tables, allowing for structured display of information in the output stream. It checks if the provided value is an IEnumerable and uses reflection to determine the generic type of the collection, invoking the appropriate formatting method to generate the table representation of the data. If the value is not an IEnumerable or does not implement a generic IEnumerable interface, it throws an ArgumentException to indicate that the value cannot be formatted as a table.
/// </summary>
/// <param name="managedStream"></param>
public class ManagedStreamTableFormatter(IManagedStream managedStream) : ManagedStreamFormatterBase(managedStream), IManagedStreamTableFormatter
{
    /// <summary>
    /// Formats a collection of values as a table and accumulates the formatted output in the StringBuilder. This method is intended to be called by the <see cref="FormatAsync{T}(T, CancellationToken)"/> method when the provided value is an IEnumerable. Derived classes can override this method to implement specific logic for formatting tables based on the type of data being formatted.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="cancellationToken">A cancellation token to observe while formatting.</param>
    /// <returns>A task that represents the asynchronous formatting operation.</returns>
    public Task FormatTableAsync<T>(IEnumerable<T> values, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Formats a given value as a table if it is an IEnumerable and accumulates the formatted output in the StringBuilder. This method checks if the provided value is an IEnumerable and uses reflection to determine the generic type of the collection. If the value is an IEnumerable, it invokes the FormatTableAsync method to format the collection as a table. If the value is not an IEnumerable or does not implement a generic IEnumerable interface, it throws an ArgumentException to indicate that the value cannot be formatted as a table.
    /// </summary>
    /// <typeparam name="T">The type of the value to format.</typeparam>
    /// <param name="value">The value to format.</param>
    /// <param name="cancellationToken">A cancellation token to observe while formatting.</param>
    /// <returns>A task that represents the asynchronous formatting operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not an IEnumerable or does not implement a generic IEnumerable interface.</exception>
    public override Task FormatAsync<T>(T value, CancellationToken cancellationToken)
    {
        if(value is not IEnumerable)
        {
            throw new ArgumentException($"Value of type {typeof(T)} is not an IEnumerable and cannot be formatted as a table.");
        }

        var genericType = typeof(T).GetInterfaces().FirstOrDefault(x => x.IsGenericType) ?? throw new ArgumentException($"Value of type {typeof(T)} does not implement a generic IEnumerable interface and cannot be formatted as a table.");
        var generic = genericType.GetGenericArguments().FirstOrDefault() ?? throw new ArgumentException($"Value of type {typeof(T)} does not implement a generic IEnumerable interface and cannot be formatted as a table.");
        var method = typeof(ManagedStreamTableFormatter).GetMethod(nameof(FormatTableAsync)) ?? throw new ArgumentException($"FormatTableAsync method not found in {typeof(ManagedStreamTableFormatter)}.");

        method = method.MakeGenericMethod(generic);

        var result = method.Invoke(this, [value, cancellationToken]);

        return result is Task task ? task : Task.CompletedTask;
    }
}
