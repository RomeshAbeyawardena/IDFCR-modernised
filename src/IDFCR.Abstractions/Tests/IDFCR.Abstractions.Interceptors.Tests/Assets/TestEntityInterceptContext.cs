namespace IDFCR.Abstractions.Interceptors.Tests.Assets;

internal record TestEntityInterceptContext(EntityContextBehaviorStage Stage,
    EntityContextBehavior Behavior,
    object? Model) : IEntityInterceptorContext;
