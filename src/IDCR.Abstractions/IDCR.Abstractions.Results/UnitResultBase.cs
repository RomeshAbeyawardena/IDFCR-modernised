using IDCR.Abstractions.Results.Exceptions;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace IDCR.Abstractions.Results;

public static class UnitResult
{
    public static IUnitResult<T> NotFound<T>(object id, Exception? innerException = null, FailureReason? failureReason = FailureReason.NotFound)
        => Failed<T>(new EntityNotFoundException(typeof(T), id, innerException), UnitAction.None, failureReason);

    public static IUnitResult<T> Failed<T>(Exception exception, UnitAction action = UnitAction.None, FailureReason? FailureReason = null)
        => new DefaultUnitResult<T>(default, action, false, exception, FailureReason);

    public static IUnitResult<T> FromResult<T>(T? result, UnitAction action = UnitAction.Get,
        bool isSuccess = true, Exception? exception = null)
    {
        return new DefaultUnitResult<T>(result, action, isSuccess, exception);
    }
}

public abstract record UnitResultBase(Exception? Exception = null, UnitAction Action = UnitAction.None,
    bool IsSuccess = false, FailureReason? FailureReason = null) : IUnitResult
{
    internal readonly ConcurrentDictionary<string, object?> _metaProperties = [];
    public object? this[string key] { get => _metaProperties[key]; }

    public int Count => _metaProperties.Count;

    public IEnumerable<string> Keys => _metaProperties.Keys;
    public IEnumerable<object?> Values => _metaProperties.Values;

    public IUnitResult AddMeta(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        _metaProperties.AddOrUpdate(key, value, (_, _) => value);
        return this;
    }

    public virtual IUnitResult<T> As<T>(T? value) => new DefaultUnitResult<T>(value, Action, IsSuccess, Exception);

    public virtual IUnitResultCollection<T> AsCollection<T>(IEnumerable<T>? value) => new UnitResultCollection<T>(value, Action, IsSuccess, Exception);

    public bool ContainsKey(string key)
    {
        return _metaProperties.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return _metaProperties.GetEnumerator();
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value)
    {
        return _metaProperties.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public abstract record UnitResultBase<TResult>(TResult? Result = default, UnitAction Action = UnitAction.None,
    bool IsSuccess = true, Exception? Exception = null, FailureReason? FailureReason = null)
    : UnitResultBase(Exception, Action, IsSuccess, FailureReason), IUnitResult<TResult>
{
    public override IUnitResult<T> As<T>(T? value) where T : default
    {
        if (Result is not T result)
        {
            throw new InvalidCastException($"Unable to cast result of type {typeof(TResult)} to {typeof(T)}");
        }

        return base.As(value ?? result);
    }

    public override IUnitResultCollection<T> AsCollection<T>(IEnumerable<T>? value)
    {
        if (Result is not IEnumerable<T> result)
        {
            throw new InvalidCastException($"Unable to cast result of type {typeof(TResult)} to {typeof(T)}");
        }

        return base.AsCollection(value ?? result);
    }

    [MemberNotNullWhen(true, nameof(Result))]
    public bool HasValue => IsSuccess && Result is not null;
}