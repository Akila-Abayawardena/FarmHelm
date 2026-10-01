using FarmHelm.Domain.Crops;
using FarmHelm.Domain.Farms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmHelm.Infrastructure.Persistence.Configurations;

public sealed class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.ToTable("batch", table =>
            table.HasCheckConstraint("ck_batch_initial_plant_count_positive", "initial_plant_count > 0"));

        builder.HasKey(batch => batch.Id);
        builder.Property(batch => batch.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(batch => batch.BatchCode).HasColumnName("batch_code").HasMaxLength(32).IsRequired();
        builder.Property(batch => batch.FarmId).HasColumnName("farm_id").HasColumnType("uuid").IsRequired();
        builder.Property(batch => batch.VarietyId).HasColumnName("variety_id").HasColumnType("uuid").IsRequired();
        builder.Property(batch => batch.LocationId).HasColumnName("location_id").HasColumnType("uuid");
        builder.Property(batch => batch.PlantingDate).HasColumnName("planting_date").HasColumnType("date").IsRequired();
        builder.Property(batch => batch.InitialPlantCount).HasColumnName("initial_plant_count").IsRequired();
        builder.Property(batch => batch.CurrentStageId).HasColumnName("current_stage_id").HasColumnType("uuid").IsRequired();
        builder.Property(batch => batch.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();
        builder.Property(batch => batch.IndividualTrackingEnabled).HasColumnName("individual_tracking_enabled").IsRequired();
        builder.Property(batch => batch.RemovedDate).HasColumnName("removed_date").HasColumnType("date");
        builder.Property(batch => batch.Notes).HasColumnName("notes").HasColumnType("text");

        builder.HasIndex(batch => batch.BatchCode).IsUnique();
        builder.HasIndex(batch => batch.FarmId);
        builder.HasIndex(batch => batch.VarietyId);
        builder.HasIndex(batch => batch.LocationId);
        builder.HasIndex(batch => batch.CurrentStageId);
        builder.HasIndex(batch => batch.PlantingDate);
        builder.HasIndex(batch => batch.Status);

        builder.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(batch => batch.FarmId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Variety>()
            .WithMany()
            .HasForeignKey(batch => batch.VarietyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<FarmLocation>()
            .WithMany()
            .HasForeignKey(batch => batch.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CropStage>()
            .WithMany()
            .HasForeignKey(batch => batch.CurrentStageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(batch => batch.Plants)
            .WithOne()
            .HasForeignKey(plant => plant.BatchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(batch => batch.Plants).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(batch => batch.MortalityRecords)
            .WithOne()
            .HasForeignKey(record => record.BatchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(batch => batch.MortalityRecords).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(batch => batch.StageHistory)
            .WithOne()
            .HasForeignKey(history => history.BatchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(batch => batch.StageHistory).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(batch => batch.DeadPlantCount);
        builder.Ignore(batch => batch.AlivePlantCount);
        builder.Ignore(batch => batch.SurvivalRate);
    }
}
