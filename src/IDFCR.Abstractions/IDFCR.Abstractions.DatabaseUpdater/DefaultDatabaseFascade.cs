using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.DatabaseUpdater;

internal sealed class DefaultDatabaseFascade(IServiceProvider serviceProvider, ITargetDatabaseConfiguration configurationInstance) : IDatabaseFascade
{
    private DbContext? DbContext
    {
        get
        {
            if (serviceProvider.GetRequiredService(configurationInstance.DbContextType) is DbContext context)
            {
                return context;
            }

            return null;
        }
    }

    private void ThrowIfDbContextIsNull()
    {
        if (DbContext is null)
        {
            throw new InvalidOperationException($"The specified DbContext type '{configurationInstance.DbContextType.FullName}' could not be resolved from the service provider. Please ensure that it is registered correctly.");
        }
    }

    public Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken)
    {
        ThrowIfDbContextIsNull();
        return DbContext!.Database.GetPendingMigrationsAsync(cancellationToken);
    }

    public Task MigrateAsync(CancellationToken cancellationToken)
    {
        ThrowIfDbContextIsNull();
        return DbContext!.Database.MigrateAsync(cancellationToken);
    }
}