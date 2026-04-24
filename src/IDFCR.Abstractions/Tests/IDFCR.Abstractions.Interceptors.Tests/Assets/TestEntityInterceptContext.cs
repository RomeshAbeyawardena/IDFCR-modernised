namespace IDFCR.Abstractions.Interceptors.Tests.Assets;

internal record TestEntityInterceptContext(EntityContextBehaviorStage Stage,
    EntityContextBehavior Behavior,
    object? Model) : IEntityInterceptorContext
{
    public Dictionary<string, object> Dictionary { get; init; } = [];

    public IReadOnlyDictionary<string, object> Data => Dictionary.AsReadOnly();
}
