using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using System.Diagnostics.CodeAnalysis;

namespace BuildTools.Cli;

public readonly record struct FieldResult<T>(
    [property: MemberNotNullWhen(true, nameof(FieldResult.Value))] bool HasValue,
    bool IsParameter,
    T? Value,
    Exception? Exception = null)
{

    public (bool, T?, Exception?) AsValueOrDefault()
    {
        return AsValueOrDefault(out _);
    }

    public (bool, T?, Exception?) AsValueOrDefault(out bool isParameter)
    {
        isParameter = IsParameter;
        return (HasValue, Value, Exception);
    }

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

public readonly record struct FieldResult(
    [property: MemberNotNullWhen(true, nameof(FieldResult.Value))] bool HasValue,
    bool IsParameter,
    string? Value)
{
    public (bool, string?) AsValueOrDefault()
    {
        return AsValueOrDefault(out _);
    }

    public (bool, string?) AsValueOrDefault(out bool isParameter)
    {
        isParameter = IsParameter;
        return (HasValue, Value);
    }
}
