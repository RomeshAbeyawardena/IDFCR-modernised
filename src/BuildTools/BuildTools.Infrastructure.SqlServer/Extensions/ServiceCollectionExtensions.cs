using IDFCR.Abstractions.Filters.Extensions;
using IDFCR.Abstractions.Interceptors.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildTools.Infrastructure.SqlServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, DbSettings settings)
    {
        var currentAssembly = typeof(PackageManagerDbContext).Assembly;

        if (!settings.ConnectionStrings.TryGetValue(settings.DefaultConnectionString
            ?? throw new InvalidOperationException("Default connection string not specified")
            , out var connectionString))
        {
            throw new InvalidOperationException("Connection string unavailable");
        }

        return services
            .AddDbContextPool<PackageManagerDbContext>(opt => opt.UseSqlServer(connectionString))
            .AddInterceptors(currentAssembly)
            .ScanFilters(currentAssembly)
            .Scan(s => s.FromAssemblies(currentAssembly)
                .AddClasses(c => c.WithAttribute<RegisteredRepositoryAttribute>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        
    }
}
