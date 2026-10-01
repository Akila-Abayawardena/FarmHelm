using FarmHelm.Domain.Crops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmHelm.Infrastructure.Persistence.Configurations;

public sealed class CropStageConfiguration : IEntityTypeConfiguration<CropStage>
{
    public void Configure(EntityTypeBuilder<CropStage> builder)
    {
        builder.ToTable("crop_stage", table =>
            table.HasCheckConstraint("ck_crop_stage_display_order_non_negative", "display_order >= 0"));

        builder.HasKey(stage => stage.Id);
        builder.Property(stage => stage.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(stage => stage.CropId).HasColumnName("crop_id").HasColumnType("uuid").IsRequired();
        builder.Property(stage => stage.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(stage => stage.DisplayOrder).HasColumnName("display_order").IsRequired();
        builder.Property(stage => stage.IsActive).HasColumnName("is_active").IsRequired();

        builder.HasIndex(stage => stage.CropId);
        builder.HasIndex(stage => new { stage.CropId, stage.Name }).IsUnique();
        builder.HasOne<Crop>()
            .WithMany()
            .HasForeignKey(stage => stage.CropId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
