using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildTools.Infrastructure.SqlServer.Features.Tags;

public class TagEntityConfiguration : IEntityTypeConfiguration<TagEntity>
{
    public void Configure(EntityTypeBuilder<TagEntity> builder)
    {
        builder.ToTable("Tag", "dbo");

        builder.HasKey(e => e.Id)
            .HasName("PK_Tag");

        builder.Property(e => e.Id)
            .HasColumnName("TagId")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()", "DF_Tag_TagId")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasAlternateKey(e => e.Name)
            .HasName("UQ_Tag_Name");

        builder.Property(e => e.DisplayName)
            .HasMaxLength(2000)
            .IsRequired(false);
    }
}