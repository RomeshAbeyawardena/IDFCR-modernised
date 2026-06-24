using System.Collections.Concurrent;

namespace IDFCR.Abstractions.Results;

internal class UnitResultBuilder(bool isSuccess,
    UnitAction action,
    Exception? exception,
    FailureReason? failureReason,
    FailureOrigin? failureOrigin) : IUnitResultBuilder
{
    private readonly ConcurrentDictionary<string, object?> _metaProperties = [];
    protected (bool IsSuccess, UnitAction Action, Exception? Exception, FailureReason? FailureReason, FailureOrigin? FailureOrigin) Fields { get; }
        = new(isSuccess, action, exception, failureReason, failureOrigin);

    protected void CopyMetaProperties(IUnitResult result)
    {
        foreach (var (key, value) in result.Meta)
        {
            if (!_metaProperties.TryAdd(key, value))
            {
                _metaProperties[key] = value;
            }
        }
    }

    public UnitResultBuilder(IUnitResult result)
        : this(result.IsSuccess,
               result.Action,
               result.Exception,
               result.FailureReason,
               result.FailureOrigin)
    {
        CopyMetaProperties(result);
    }

    /// <summary>
    /// When inherited by a derived type, attempts to set the modified state of the result by setting the ModifiedState property. Returns true if the value is of the correct type and was set successfully; otherwise, returns false. By default, this method returns false, indicating that the modified state cannot be set for this result type. Derived types that support a modified state should override this method to provide the appropriate logic for setting the modified state based on the specific requirements of that result type.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual bool TrySetState(object? value) => false;

    public IUnitResultBuilder AddMeta(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        _metaProperties.AddOrUpdate(key, value, (_, _) => value);
        return this;
    }

    /// <inheritdoc />
    public virtual IUnitResult<T> As<T>(T? value) => new DefaultUnitResult<T>(value, action, isSuccess, exception);

    /// <inheritdoc />
    public virtual IUnitResultCollection<T> AsCollection<T>(IEnumerable<T>? value) => new UnitResultCollection<T>(value, action, isSuccess, exception);

    public IUnitResult Build()
    {
        return new DefaultUnitResult(
            exception,
            action, isSuccess,
            failureReason,
            failureOrigin);
    }
}

internal sealed class UnitResultBuilder<T>(bool isSuccess,
    UnitAction action,
    Exception? exception,
    FailureReason? failureReason,
    string? namedResult,
    FailureOrigin? failureOrigin,
    T? result)
    : UnitResultBuilder(isSuccess, action, exception, failureReason, failureOrigin), IUnitResultBuilder<T>
{
    private T? Result { get; } = result;

    public UnitResultBuilder(IUnitResult<T> result)
        : this(result.IsSuccess,
               result.Action,
               result.Exception,
               result.FailureReason,
               result.NamedResult,
               result.FailureOrigin,
               result.Result)
    {
        CopyMetaProperties(result);
    }

    /// <summary>
    /// Gets a value indicating whether the state has been modified. This property is used to track whether the original state has been changed or updated, allowing for scenarios where the result may need to be modified after its initial creation. By tracking this state, it enables the implementation of features such as change tracking, auditing, or conditional logic based on whether the result has been modified since it was created.
    /// </summary>
    private bool IsStateModified;

    public T? OriginalState { get; private set; }

    /// <inheritdoc />
    public T? ModifiedState { get; private set; }

    /// <inheritdoc />
    public bool TrySetState(T? result)
    {
        IsStateModified = result is not null;
        ModifiedState = result;
        return IsStateModified;
    }

    /// <summary>
    /// Attempts to set the modified state of the result by setting the ModifiedState property. Returns true if the value is of the correct type and was set successfully; otherwise, returns false.
    /// </summary>
    /// <param name="value">The value to set as the modified state.</param>
    /// <returns>True if the modified state was set successfully; otherwise, false.</returns>
    public override bool TrySetState(object? value)
    {
        if (value is T result)
        {
            return TrySetState(result);
        }

        return false;
    }


    public IUnitResult<T> Build(T? result)
    {
        return new DefaultUnitResult<T>(
            result ?? this.Result,
            Fields.Action, Fields.IsSuccess,
            Fields.Exception,
            Fields.FailureReason,
            namedResult, Fields.FailureOrigin);
    }
}
