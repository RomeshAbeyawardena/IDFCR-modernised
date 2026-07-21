#pragma warning disable IDE0130
namespace IDFCR.Abstractions.Mediator.Extensions;
#pragma warning restore IDE0130
/// <summary>
/// Represents a builder interface for configuring options related to adding MediatR services and pipelines to the dependency injection container. This interface allows you to specify whether to use a unit of work post-pipeline processor and whether to use FluentValidation for request validation. By implementing this interface, you can customize the behavior of the MediatR service registration and pipeline configuration, providing flexibility in how MediatR services are set up and how they interact with other components in your application.
/// </summary>
public interface IMediatorServiceCollectionOptionsBuilder
{
    /// <summary>
    /// Configures whether to register MediatR services from the provided assemblies. If set to true, the AddMediatR method will automatically scan the specified assemblies for MediatR handlers and register them with the dependency injection container. If set to false, you will need to manually register your MediatR handlers and services. The default value is true, which means that services will be registered from the provided assemblies by default.
    /// </summary>
    /// <param name="registerServicesFromAssemblies">A boolean value indicating whether to register MediatR services from the provided assemblies.</param>
    /// <returns>The current instance of the builder for chaining.</returns>
    IMediatorServiceCollectionOptionsBuilder RegisterServices(bool registerServicesFromAssemblies = true);
    /// <summary>
    /// Configures whether to use the UnitOfWorkPostPipelineProcessor in the MediatR pipeline. If set to true, the UnitOfWorkPostPipelineProcessor will be added as an open request post-processor, allowing it to handle unit of work operations after request processing. If set to false, the UnitOfWorkPostPipelineProcessor will not be included in the pipeline, and you will need to handle unit of work operations manually if needed. The default value is true, which means that the UnitOfWorkPostPipelineProcessor will be used by default.
    /// </summary>
    /// <param name="useUnitOfWorkPostPipelineProcessor">A boolean value indicating whether to use the UnitOfWorkPostPipelineProcessor.</param>
    /// <returns>The current instance of the builder for chaining.</returns>
    IMediatorServiceCollectionOptionsBuilder UseUnitOfWorkPostPipeline(bool useUnitOfWorkPostPipelineProcessor = true);
    /// <summary>
    /// Configures whether to use the FluentValidationProcessor in the MediatR pipeline. If set to true, the FluentValidationProcessor will be added as an open request pre-processor, allowing it to handle validation of requests using FluentValidation before request processing. If set to false, the FluentValidationProcessor will not be included in the pipeline, and you will need to handle request validation manually if needed. The default value is false, which means that the FluentValidationProcessor will not be used by default.
    /// </summary>
    /// <param name="useFluentValidationProcessor">A boolean value indicating whether to use the FluentValidationProcessor.</param>
    /// <returns>The current instance of the builder for chaining.</returns>
    IMediatorServiceCollectionOptionsBuilder UseFluentValidation(bool useFluentValidationProcessor = true);

    /// <summary>
    /// Builds and returns an instance of MediatorServiceCollectionOptions based on the configured options. This method finalizes the configuration and provides a concrete representation of the options that can be used for adding MediatR services and pipelines to the dependency injection container. The returned MediatorServiceCollectionOptions instance contains the specified settings for registering services from assemblies, using the UnitOfWorkPostPipelineProcessor, and using the FluentValidationProcessor.
    /// </summary>
    /// <returns>An instance of MediatorServiceCollectionOptions with the configured settings.</returns>
    MediatorServiceCollectionOptions Build();
}
