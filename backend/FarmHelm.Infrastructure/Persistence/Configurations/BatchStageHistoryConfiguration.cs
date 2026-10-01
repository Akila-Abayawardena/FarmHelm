using FarmHelm.Domain.Crops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmHelm.Infrastructure.Persistence.Configurations;

public sealed class BatchStageHistoryConfiguration : IEntityTypeConfiguration<BatchStageHistory>
{
    public void Configure(EntityTypeBuilder<BatchStageHistory> builder)
    {
        builder.ToTable("batch_stage_history");

        builder.HasKey(history => history.Id);
        builder.Property(history => history.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(history => history.BatchId).HasColumnName("batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(history => history.StageId).HasColumnName("stage_id").HasColumnType("uuid").IsRequired();
        builder.Property(history => history.EffectiveDate).HasColumnName("effective_date").HasColumnType("date").IsRequired();
        builder.Property(history => history.Notes).HasColumnName("notes").HasColumnType("text");

        builder.HasIndex(history => history.BatchId);
        builder.HasIndex(history => history.StageId);
        builder.HasIndex(history => history.EffectiveDate);
        builder.HasOne<CropStage>()
            .WithMany()
            .HasForeignKey(history => history.StageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
