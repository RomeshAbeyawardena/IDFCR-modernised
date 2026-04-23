namespace IDFCR.Abstractions.Mediator.Extensions.Extensions;

/// <summary>
/// Represents configuration options for adding MediatR services and pipelines to the dependency injection container. This record allows you to specify whether to register services from assemblies and whether to use a unit of work post-pipeline processor. By configuring these options, you can control the behavior of the MediatR service registration and the inclusion of specific pipelines, such as the UnitOfWorkPostPipelineProcessor, which handles unit of work operations after request processing. This provides flexibility in how MediatR services are set up and how they interact with other components in your application.
/// </summary>
public record MediatorServiceCollectionOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to register MediatR services from the provided assemblies. If set to true, the AddMediatR method will automatically scan the specified assemblies for MediatR handlers and register them with the dependency injection container. If set to false, you will need to manually register your MediatR handlers and services. The default value is true, which means that services will be registered from the provided assemblies by default.
    /// </summary>
    public bool RegisterServicesFromAssemblies { get; init; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether to use the UnitOfWorkPostPipelineProcessor in the MediatR pipeline. If set to true, the UnitOfWorkPostPipelineProcessor will be added as an open request post-processor, allowing it to handle unit of work operations after request processing. If set to false, the UnitOfWorkPostPipelineProcessor will not be included in the pipeline, and you will need to handle unit of work operations manually if needed. The default value is true, which means that the UnitOfWorkPostPipelineProcessor will be used by default.
    /// </summary>
    public bool UseUnitOfWorkPostPipelineProcessor { get; init; } = true;
}
