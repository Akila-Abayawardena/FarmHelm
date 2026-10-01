using FarmHelm.Domain.Crops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmHelm.Infrastructure.Persistence.Configurations;

public sealed class PlantConfiguration : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.ToTable("plant", table =>
            table.HasCheckConstraint("ck_plant_plant_number_positive", "plant_number > 0"));

        builder.HasKey(plant => plant.Id);
        builder.Property(plant => plant.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(plant => plant.BatchId).HasColumnName("batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(plant => plant.PlantNumber).HasColumnName("plant_number").IsRequired();
        builder.Property(plant => plant.PlantCode).HasColumnName("plant_code").HasMaxLength(48).IsRequired();
        builder.Property(plant => plant.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();
        builder.Property(plant => plant.Notes).HasColumnName("notes").HasColumnType("text");

        builder.HasAlternateKey(plant => new { plant.Id, plant.BatchId });
        builder.HasIndex(plant => plant.BatchId);
        builder.HasIndex(plant => new { plant.BatchId, plant.PlantNumber }).IsUnique();
        builder.HasIndex(plant => plant.PlantCode).IsUnique();
    }
}
