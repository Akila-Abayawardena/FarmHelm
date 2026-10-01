using FarmHelm.Domain.Crops;
using FarmHelm.Domain.Farms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmHelm.Infrastructure.Persistence.Configurations;

public sealed class MortalityReasonConfiguration : IEntityTypeConfiguration<MortalityReason>
{
    public void Configure(EntityTypeBuilder<MortalityReason> builder)
    {
        builder.ToTable("mortality_reason", table =>
            table.HasCheckConstraint("ck_mortality_reason_display_order_non_negative", "display_order >= 0"));

        builder.HasKey(reason => reason.Id);
        builder.Property(reason => reason.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(reason => reason.FarmId).HasColumnName("farm_id").HasColumnType("uuid").IsRequired();
        builder.Property(reason => reason.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(reason => reason.DisplayOrder).HasColumnName("display_order").IsRequired();
        builder.Property(reason => reason.IsActive).HasColumnName("is_active").IsRequired();

        builder.HasIndex(reason => reason.FarmId);
        builder.HasIndex(reason => new { reason.FarmId, reason.Name }).IsUnique();
        builder.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(reason => reason.FarmId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
