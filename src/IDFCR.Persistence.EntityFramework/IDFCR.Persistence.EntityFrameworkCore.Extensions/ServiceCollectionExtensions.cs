using IDFCR.Persistence.EntityFrameworkCore.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Persistence.EntityFrameworkCore.Extensions;

/// <summary>
/// Defines extension methods for registering repositories in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Defines an extension method for registering repositories in the dependency injection container. It scans the specified assemblies for classes decorated with the <see cref="RegisteredRepositoryAttribute"/> and registers them as their implemented interfaces with a scoped lifetime.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the repositories to.</param>
    /// <param name="assemblies">The assemblies to scan for repository classes.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.WithAttribute<RegisteredRepositoryAttribute>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
    }
}