using IDFCR.Abstractions.Results.Exceptions;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Results;

/// <summary>
/// Factory helpers for unit results.
/// </summary>
public static class UnitResult
{
    /// <summary>
    /// Creates a failed result that represents a missing entity.
    /// </summary>
    public static IUnitResult<T> NotFound<T>(object id, Exception? innerException = null, FailureReason? failureReason = FailureReason.NotFound)
        => Failed<T>(new EntityNotFoundException(typeof(T), id, innerException), UnitAction.None, failureReason);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static IUnitResult<T> Failed<T>(Exception exception, UnitAction action = UnitAction.None, FailureReason? FailureReason = null)
        => new DefaultUnitResult<T>(default, action, false, exception, FailureReason);

    /// <summary>
    /// Creates a unit result from the supplied value.
    /// </summary>
    public static IUnitResult<T> FromResult<T>(T? result, UnitAction action = UnitAction.Get,
        bool isSuccess = true, Exception? exception = null)
    {
        return new DefaultUnitResult<T>(result, action, isSuccess, exception);
    }
}

/// <summary>
/// Base type for unit results that carry metadata.
/// </summary>
/// <param name="Exception">The captured exception.</param>
/// <param name="Action">The associated action.</param>
/// <param name="IsSuccess">A value indicating whether the operation succeeded.</param>
/// <param name="FailureReason">The failure reason.</param>
public abstract record UnitResultBase(Exception? Exception = null, UnitAction Action = UnitAction.None,
    bool IsSuccess = false, FailureReason? FailureReason = null) : IUnitResult
{
    internal readonly ConcurrentDictionary<string, object?> _metaProperties = [];
    /// <inheritdoc />
    public object? this[string key] { get => _metaProperties[key]; }

    /// <inheritdoc />
    public int Count => _metaProperties.Count;

    /// <inheritdoc />
    public IEnumerable<string> Keys => _metaProperties.Keys;
    /// <inheritdoc />
    public IEnumerable<object?> Values => _metaProperties.Values;

    /// <inheritdoc />
    public IUnitResult AddMeta(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        _metaProperties.AddOrUpdate(key, value, (_, _) => value);
        return this;
    }

    /// <inheritdoc />
    public virtual IUnitResult<T> As<T>(T? value) => new DefaultUnitResult<T>(value, Action, IsSuccess, Exception);

    /// <inheritdoc />
    public virtual IUnitResultCollection<T> AsCollection<T>(IEnumerable<T>? value) => new UnitResultCollection<T>(value, Action, IsSuccess, Exception);

    /// <inheritdoc />
    public bool ContainsKey(string key)
    {
        return _metaProperties.ContainsKey(key);
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return _metaProperties.GetEnumerator();
    }

    /// <inheritdoc />
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value)
    {
        return _metaProperties.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

/// <summary>
/// Base type for typed unit results.
/// </summary>
/// <typeparam name="TResult">The result value type.</typeparam>
/// <param name="Result">The result value.</param>
/// <param name="Action">The associated action.</param>
/// <param name="IsSuccess">A value indicating whether the operation succeeded.</param>
/// <param name="Exception">The captured exception.</param>
/// <param name="FailureReason">The failure reason.</param>
public abstract record UnitResultBase<TResult>(TResult? Result = default, UnitAction Action = UnitAction.None,
    bool IsSuccess = true, Exception? Exception = null, FailureReason? FailureReason = null)
    : UnitResultBase(Exception, Action, IsSuccess, FailureReason), IUnitResult<TResult>
{
    /// <inheritdoc />
    public override IUnitResult<T> As<T>(T? value) where T : default
    {
        if (Result is not T result)
        {
            throw new InvalidCastException($"Unable to cast result of type {typeof(TResult)} to {typeof(T)}");
        }

        return base.As(value ?? result);
    }

    /// <inheritdoc />
    public override IUnitResultCollection<T> AsCollection<T>(IEnumerable<T>? value)
    {
        if (Result is not IEnumerable<T> result)
        {
            throw new InvalidCastException($"Unable to cast result of type {typeof(TResult)} to {typeof(T)}");
        }

        return base.AsCollection(value ?? result);
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(Result))]
    public bool HasValue => IsSuccess && Result is not null;
}
