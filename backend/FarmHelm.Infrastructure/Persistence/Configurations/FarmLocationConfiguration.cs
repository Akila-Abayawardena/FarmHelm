using FarmHelm.Domain.Farms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmHelm.Infrastructure.Persistence.Configurations;

public sealed class FarmLocationConfiguration : IEntityTypeConfiguration<FarmLocation>
{
    public void Configure(EntityTypeBuilder<FarmLocation> builder)
    {
        builder.ToTable("farm_location");

        builder.HasKey(location => location.Id);
        builder.Property(location => location.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(location => location.FarmId).HasColumnName("farm_id").HasColumnType("uuid").IsRequired();
        builder.Property(location => location.LocationCode).HasColumnName("location_code").HasMaxLength(32).IsRequired();
        builder.Property(location => location.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(location => location.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(location => location.IsActive).HasColumnName("is_active").IsRequired();

        builder.HasIndex(location => location.FarmId);
        builder.HasIndex(location => location.LocationCode).IsUnique();
        builder.HasIndex(location => new { location.FarmId, location.Name }).IsUnique();
        builder.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(location => location.FarmId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
