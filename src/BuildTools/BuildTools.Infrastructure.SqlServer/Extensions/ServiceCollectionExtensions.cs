using IDFCR.Abstractions.Filters.Extensions;
using IDFCR.Abstractions.Interceptors.DependencyInjection.Extensions;
using IDFCR.Persistence.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace BuildTools.Infrastructure.SqlServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, DbSettings settings)
    {
        var currentAssembly = typeof(PackageManagerDbContext).Assembly;

        if (!settings.ConnectionStrings.TryGetValue(settings.DefaultConnectionStringName
            ?? throw new InvalidOperationException("Default connection string not specified")
            , out var connectionString))
        {
            throw new InvalidOperationException("Connection string unavailable");
        }


        DbConnectionStringBuilder connectionStringBuilder = new()
        {
            ConnectionString = connectionString
        };


        if (!string.IsNullOrWhiteSpace(settings.Server))
        {
            connectionStringBuilder.Add("Server", settings.Server);
        }

        if (!string.IsNullOrWhiteSpace(settings.InitialCatalog))
        {
            connectionStringBuilder.Add("Initial Catalog", settings.InitialCatalog);
        }

        if (!string.IsNullOrWhiteSpace(settings.UserId))
        {
            connectionStringBuilder.Add("User Id", settings.UserId);
        }

        if (!string.IsNullOrWhiteSpace(settings.Password))
        {
            connectionStringBuilder.Add("Password", settings.Password);
        }

        return services
            .AddRepositories(currentAssembly)
            .AddDbContextPool<PackageManagerDbContext>(opt => opt.UseSqlServer(connectionStringBuilder.ToString())
            .EnableDetailedErrors(settings.EnableDetailedErrors))
            .AddInterceptors(currentAssembly)
            .ScanFilters(currentAssembly);
        
    }
}
