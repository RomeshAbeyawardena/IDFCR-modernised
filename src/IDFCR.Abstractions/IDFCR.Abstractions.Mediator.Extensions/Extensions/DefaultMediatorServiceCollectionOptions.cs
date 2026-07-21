#pragma warning disable IDE0130 // this is inside a subfolder called extensions, but the namespace is IDFCR.Abstractions.Mediator.Extensions, so we disable this warning
using Microsoft.Extensions.Configuration;

namespace IDFCR.Abstractions.Mediator.Extensions;
#pragma warning restore IDE0130
internal sealed class DefaultMediatorServiceCollectionOptionsBuilder
    : IMediatorServiceCollectionOptionsBuilder
{
    private bool useUnitOfWorkPostPipelineProcessor = true;
    private bool useFluentValidationProcessor;
    private bool registerServicesFromAssemblies = true;
    private string? licenseKey;
    private string? licenseConfigurationKey;

    public IMediatorServiceCollectionOptionsBuilder UseLicense(
        string licenseKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(licenseKey);

        this.licenseKey = licenseKey;
        licenseConfigurationKey = null;

        return this;
    }

    public IMediatorServiceCollectionOptionsBuilder UseLicenseFromConfiguration(
        string configurationKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(configurationKey);

        licenseConfigurationKey = configurationKey;
        licenseKey = null;

        return this;
    }

    public IMediatorServiceCollectionOptionsBuilder RegisterServicesFromAssemblies(
        bool enabled = true)
    {
        registerServicesFromAssemblies = enabled;
        return this;
    }

    public IMediatorServiceCollectionOptionsBuilder UseFluentValidation(
        bool enabled = true)
    {
        useFluentValidationProcessor = enabled;
        return this;
    }

    public IMediatorServiceCollectionOptionsBuilder UseUnitOfWorkPostPipeline(
        bool enabled = true)
    {
        useUnitOfWorkPostPipelineProcessor = enabled;
        return this;
    }

    public MediatorServiceCollectionOptions Build(
        IConfiguration? configuration)
    {
        if (configuration is null &&
            !string.IsNullOrWhiteSpace(licenseConfigurationKey))
        {
            throw new InvalidOperationException(
                "Unable to obtain a MediatR license key from configuration " +
                "without providing an instance of IConfiguration.");
        }

        var resolvedLicenseKey =
            !string.IsNullOrWhiteSpace(licenseConfigurationKey)
                ? configuration?[licenseConfigurationKey]
                : licenseKey;

        return new MediatorServiceCollectionOptions
        {
            RegisterServicesFromAssemblies = registerServicesFromAssemblies,
            UseFluentValidationProcessor = useFluentValidationProcessor,
            UseUnitOfWorkPostPipelineProcessor =
                useUnitOfWorkPostPipelineProcessor,
            MediatrLicenseKey = resolvedLicenseKey
        };
    }
}