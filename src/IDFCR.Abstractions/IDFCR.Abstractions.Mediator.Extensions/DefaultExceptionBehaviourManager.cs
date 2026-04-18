namespace IDFCR.Abstractions.Mediator.Extensions;

internal sealed class DefaultExceptionBehaviourManager(IDictionary<Type, ExceptionBehaviour>? behaviourDictionary = null,
    ExceptionBehaviour? defaultExceptionBehaviour = null) : IExceptionBehaviourManager
{
    private Dictionary<Type, ExceptionBehaviour> _behaviourDictionary = behaviourDictionary is not null ? new(behaviourDictionary) : [];
    public ExceptionBehaviour? DefaultExceptionBehaviour { get; } = defaultExceptionBehaviour;

    public ExceptionBehaviour? GetExceptionBehaviour<TException>()
    {
        if (_behaviourDictionary.TryGetValue(typeof(TException), out var behaviour))
        {
            return behaviour;
        }

        return null;
    }
}
