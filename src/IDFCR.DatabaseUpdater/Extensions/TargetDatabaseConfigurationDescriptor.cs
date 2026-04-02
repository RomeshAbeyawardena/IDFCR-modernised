using IDFCR.Abstractions.DatabaseUpdater;

namespace IDFCR.DatabaseUpdater.Extensions;

internal record TargetDatabaseConfigurationDescriptor<TDbContextType> : ITargetDatabaseConfiguration
{
    public Type DbContextType => typeof(TDbContextType);
}
