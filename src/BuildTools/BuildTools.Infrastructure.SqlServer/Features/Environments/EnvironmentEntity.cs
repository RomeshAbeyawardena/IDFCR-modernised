using BuildTools.Shared.Features.Environments;
using IDFCR.Abstractions.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildTools.Infrastructure.SqlServer.Features.Environments;

public class EnvironmentEntityConfiguration : IEntityTypeConfiguration<EnvironmentEntity>
{
    public void Configure(EntityTypeBuilder<EnvironmentEntity> builder)
    {
        builder.ToTable("Environment", "SYSTEM_CONFIG");

        builder.HasKey(e => e.Id)
            .HasName("PK_EnvironmentId");

        builder.Property(e => e.Id)
            .HasColumnName("EnvironmentId")
            .HasDefaultValueSql("NEWSEQUENTIALID()", "DF_EnvironmentId")
            .IsRequired();

        builder.Property(e => e.ExternalReference)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(e => e.ExternalReference)
            .IsUnique()
            .HasDatabaseName("UQ_SystemConfig_Environment_ExternalReference");

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("UQ_SystemConfig_Environment_Name");

        builder.Property(e => e.DisplayName)
            .HasMaxLength(200)
            .IsRequired(false);
    }
}

public class EnvironmentEntity : MapperBase<IEnvironment>, IEnvironment
{
    object? IEnvironment.Id => Id;
    public Guid Id { get; set; }
    public string ExternalReference { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }

    public override void Map(IEnvironment source)
    {
        if (source.Id is not null && source.Id is Guid id)
        {
            Id = id;
        }

        ExternalReference = source.ExternalReference;
        Name = source.Name;
        DisplayName = source.DisplayName;
    }
}