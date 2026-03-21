namespace IDFCR.Abstractions.Interceptors;

public interface IEntityInterceptorContext
{
    EntityContextBehaviorStage Stage { get; }
    EntityContextBehavior Behavior { get; }
    object? Model { get; }
}
