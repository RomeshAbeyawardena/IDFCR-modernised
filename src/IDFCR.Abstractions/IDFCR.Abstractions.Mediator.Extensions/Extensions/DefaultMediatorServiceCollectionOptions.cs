#pragma warning disable IDE0130
namespace IDFCR.Abstractions.Mediator.Extensions;
#pragma warning restore IDE0130
internal class DefaultMediatorServiceCollectionOptions : IMediatorServiceCollectionOptionsBuilder
{
    private bool useUnitOfWorkPostPipelineProcessor = true;
    private bool useFluentValidationProcessor = false;
    private bool registerServicesFromAssemblies = true;

    public MediatorServiceCollectionOptions Build()
    {
        return new MediatorServiceCollectionOptions()
        {
            RegisterServicesFromAssemblies = registerServicesFromAssemblies,
            UseFluentValidationProcessor = useFluentValidationProcessor,
            UseUnitOfWorkPostPipelineProcessor = useUnitOfWorkPostPipelineProcessor
        };
    }

    public IMediatorServiceCollectionOptionsBuilder RegisterServices(bool registerServicesFromAssemblies = true)
    {
        this.registerServicesFromAssemblies = registerServicesFromAssemblies;
        return this;
    }

    public IMediatorServiceCollectionOptionsBuilder UseFluentValidation(bool useFluentValidationProcessor = true)
    {
        this.useFluentValidationProcessor = useFluentValidationProcessor;
        return this;
    }

    public IMediatorServiceCollectionOptionsBuilder UseUnitOfWorkPostPipeline(bool useUnitOfWorkPostPipelineProcessor = true)
    {
        this.useUnitOfWorkPostPipelineProcessor = useUnitOfWorkPostPipelineProcessor;
        return this;
    }
}
