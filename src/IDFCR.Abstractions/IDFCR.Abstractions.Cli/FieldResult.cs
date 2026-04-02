using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Cli;

/// <summary>
/// Represents the result of a field operation with a strongly-typed value.
/// </summary>
/// <typeparam name="T">The type of the value contained in the result.</typeparam>
/// <param name="HasValue">Indicates whether the result contains a valid value.</param>
/// <param name="IsParameter">Indicates whether the field originated from a parameter.</param>
/// <param name="Value">The value of the field, if available.</param>
/// <param name="Exception">The exception that occurred during field processing, if any.</param>
public readonly record struct FieldResult<T>(
    [property: MemberNotNullWhen(true, nameof(FieldResult.Value))] bool HasValue,
    bool IsParameter,
    T? Value,
    Exception? Exception = null)
{
    /// <summary>
    /// Returns the value or default as a tuple without the parameter flag.
    /// </summary>
    /// <returns>A tuple containing the HasValue flag, the Value, and any Exception.</returns>
    public (bool, T?, Exception?) AsValueOrDefault()
    {
        return AsValueOrDefault(out _);
    }

    /// <summary>
    /// Returns the value or default as a tuple with the parameter flag.
    /// </summary>
    /// <param name="isParameter">When this method returns, contains a value indicating whether the field is a parameter.</param>
    /// <returns>A tuple containing the HasValue flag, the Value, and any Exception.</returns>
    public (bool, T?, Exception?) AsValueOrDefault(out bool isParameter)
    {
        isParameter = IsParameter;
        return (HasValue, Value, Exception);
    }

    /// <summary>
    /// Renders an error message to the managed stream if an exception occurred.
    /// </summary>
    /// <param name="managedStream">The managed stream to write error messages to.</param>
    /// <param name="genericError">The generic error message to display.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if an exception was rendered; otherwise, <c>false</c>.</returns>
    public async Task<bool> RenderOnException(IManagedStream managedStream, string genericError, CancellationToken cancellationToken)
    {
        bool result = !HasValue && Exception is not null;
        if (result)
        {
            await managedStream.Error.WriteLineAsync(genericError, cancellationToken);

#if DEBUG
            if (Exception is not null)
            {
                await managedStream.Error.WriteLineAsync(Exception.Message, cancellationToken);
            }
#endif
        }

        return result;
    }
}

/// <summary>
/// Represents the result of a field operation with a string value.
/// </summary>
/// <param name="HasValue">Indicates whether the result contains a valid value.</param>
/// <param name="IsParameter">Indicates whether the field originated from a parameter.</param>
/// <param name="Value">The string value of the field, if available.</param>
public readonly record struct FieldResult(
    [property: MemberNotNullWhen(true, nameof(FieldResult.Value))] bool HasValue,
    bool IsParameter,
    string? Value)
{
    /// <summary>
    /// Returns the value or default as a tuple without the parameter flag.
    /// </summary>
    /// <returns>A tuple containing the HasValue flag and the Value.</returns>
    public (bool, string?) AsValueOrDefault()
    {
        return AsValueOrDefault(out _);
    }

    /// <summary>
    /// Returns the value or default as a tuple with the parameter flag.
    /// </summary>
    /// <param name="isParameter">When this method returns, contains a value indicating whether the field is a parameter.</param>
    /// <returns>A tuple containing the HasValue flag and the Value.</returns>
    public (bool, string?) AsValueOrDefault(out bool isParameter)
    {
        isParameter = IsParameter;
        return (HasValue, Value);
    }
}
