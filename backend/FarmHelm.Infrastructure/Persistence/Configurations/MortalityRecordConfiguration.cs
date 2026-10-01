using FarmHelm.Domain.Crops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmHelm.Infrastructure.Persistence.Configurations;

public sealed class MortalityRecordConfiguration : IEntityTypeConfiguration<MortalityRecord>
{
    public void Configure(EntityTypeBuilder<MortalityRecord> builder)
    {
        builder.ToTable("mortality_record", table =>
            table.HasCheckConstraint("ck_mortality_record_quantity_positive", "quantity > 0"));

        builder.HasKey(record => record.Id);
        builder.Property(record => record.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(record => record.BatchId).HasColumnName("batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(record => record.PlantId).HasColumnName("plant_id").HasColumnType("uuid");
        builder.Property(record => record.MortalityDate).HasColumnName("mortality_date").HasColumnType("date").IsRequired();
        builder.Property(record => record.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(record => record.ReasonId).HasColumnName("reason_id").HasColumnType("uuid").IsRequired();
        builder.Property(record => record.Notes).HasColumnName("notes").HasColumnType("text");

        builder.HasIndex(record => record.BatchId);
        builder.HasIndex(record => record.MortalityDate);
        builder.HasIndex(record => record.ReasonId);
        builder.HasIndex(record => record.PlantId).IsUnique();
        builder.HasOne<MortalityReason>()
            .WithMany()
            .HasForeignKey(record => record.ReasonId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Plant>()
            .WithMany()
            .HasForeignKey(record => new { record.PlantId, record.BatchId })
            .HasPrincipalKey(plant => new { plant.Id, plant.BatchId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
