using IDFCR.Abstractions.Results;
using System.Collections.Concurrent;

namespace IDFCR.Abstractions.Mediator.Extensions;

/// <inheritdoc/>
public class ExceptionBehaviourManagerBuilder : IExceptionBehaviourManagerBuilder
{
    private ConcurrentDictionary<Type, ExceptionBehaviour> behaviourDictionary = [];
    private ExceptionBehaviour? defaultExceptionBehaviour = Default;

    private static readonly ExceptionBehaviour Default = new ExceptionBehaviour(UnitAction.None, FailureReason.Unknown);

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
