using FarmHelm.Domain.Crops;
using FarmHelm.Domain.Farms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmHelm.Infrastructure.Persistence.Configurations;

public sealed class CropConfiguration : IEntityTypeConfiguration<Crop>
{
    public void Configure(EntityTypeBuilder<Crop> builder)
    {
        builder.ToTable("crop");

        builder.HasKey(crop => crop.Id);
        builder.Property(crop => crop.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(crop => crop.FarmId).HasColumnName("farm_id").HasColumnType("uuid").IsRequired();
        builder.Property(crop => crop.CropCode).HasColumnName("crop_code").HasMaxLength(32).IsRequired();
        builder.Property(crop => crop.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(crop => crop.ScientificName).HasColumnName("scientific_name").HasMaxLength(200);
        builder.Property(crop => crop.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(crop => crop.IsActive).HasColumnName("is_active").IsRequired();

        builder.HasIndex(crop => crop.FarmId);
        builder.HasIndex(crop => crop.CropCode).IsUnique();
        builder.HasIndex(crop => new { crop.FarmId, crop.Name }).IsUnique();
        builder.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(crop => crop.FarmId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
