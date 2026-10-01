using FarmHelm.Domain.Crops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmHelm.Infrastructure.Persistence.Configurations;

public sealed class VarietyConfiguration : IEntityTypeConfiguration<Variety>
{
    public void Configure(EntityTypeBuilder<Variety> builder)
    {
        builder.ToTable("variety");

        builder.HasKey(variety => variety.Id);
        builder.Property(variety => variety.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(variety => variety.CropId).HasColumnName("crop_id").HasColumnType("uuid").IsRequired();
        builder.Property(variety => variety.VarietyCode).HasColumnName("variety_code").HasMaxLength(32).IsRequired();
        builder.Property(variety => variety.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(variety => variety.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(variety => variety.IsActive).HasColumnName("is_active").IsRequired();

        builder.HasIndex(variety => variety.CropId);
        builder.HasIndex(variety => variety.VarietyCode).IsUnique();
        builder.HasIndex(variety => new { variety.CropId, variety.Name }).IsUnique();
        builder.HasOne<Crop>()
            .WithMany()
            .HasForeignKey(variety => variety.CropId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
