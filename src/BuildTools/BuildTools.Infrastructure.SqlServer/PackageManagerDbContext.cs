using BuildTools.Infrastructure.SqlServer.Features.Environments;
using BuildTools.Infrastructure.SqlServer.Features.Packages;
using BuildTools.Infrastructure.SqlServer.Features.Packages.Version;
using BuildTools.Infrastructure.SqlServer.Features.Settings;
using BuildTools.Infrastructure.SqlServer.Features.Tags;
using IDFCR.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer;

public class PackageManagerDbContext(DbContextOptions<PackageManagerDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<EnvironmentEntity> Environments { get; set; }
    public DbSet<PackageEntity> Packages { get; set; }

    public DbSet<PackageTagEntity> PackageTags { get; set; }
    public DbSet<PackageVersionEntity> PackageVersions { get; set; }
    public DbSet<SettingEntity> Settings { get; set; }

    public DbSet<SettingAuditEntity> SettingAudits { get; set; }
    public DbSet<TagEntity> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PackageManagerDbContext).Assembly);
    }
}
