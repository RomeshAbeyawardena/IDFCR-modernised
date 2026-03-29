using BuildTools.Infrastructure.SqlServer.Features.Settings;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer;

public class PackageManagerDbContext(DbContextOptions<PackageManagerDbContext> options) : DbContext(options)
{
    public DbSet<SettingEntity> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PackageManagerDbContext).Assembly);
    }
}
