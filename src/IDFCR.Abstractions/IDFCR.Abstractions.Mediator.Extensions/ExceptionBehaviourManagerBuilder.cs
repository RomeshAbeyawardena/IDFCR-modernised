using IDFCR.Abstractions.Results;
using System.Collections.Concurrent;

namespace IDFCR.Abstractions.Mediator.Extensions;

/// <inheritdoc/>
public class ExceptionBehaviourManagerBuilder : IExceptionBehaviourManagerBuilder
{
    private readonly ConcurrentDictionary<Type, ExceptionBehaviour> behaviourDictionary = [];
    private ExceptionBehaviour? defaultExceptionBehaviour = Default;

    /// <summary>
    /// Defines the default exception behaviour to be used when no specific behaviour is set for an exception type.
    /// </summary>
    public static readonly ExceptionBehaviour Default = new(UnitAction.None, FailureReason.Unknown);

    /// <inheritdoc/>
    public IExceptionBehaviourManager Build()
    {
        return new DefaultExceptionBehaviourManager(behaviourDictionary, defaultExceptionBehaviour);
    }

    /// <inheritdoc/>
    public IExceptionBehaviourManagerBuilder Set<TException>(ExceptionBehaviour exceptionBehaviour)
    {
        behaviourDictionary.AddOrUpdate(typeof(TException), exceptionBehaviour, (exceptionType, oldValue) =>
        {
            behaviourDictionary[exceptionType] = exceptionBehaviour;
            return oldValue;
        });

        return this;
    }

    /// <inheritdoc/>
    public IExceptionBehaviourManagerBuilder SetDefault(ExceptionBehaviour exceptionBehaviour)
    {
        defaultExceptionBehaviour = exceptionBehaviour;
        return this;
    }
}
