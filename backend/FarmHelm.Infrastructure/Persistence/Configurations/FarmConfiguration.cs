using FarmHelm.Domain.Farms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmHelm.Infrastructure.Persistence.Configurations;

public sealed class FarmConfiguration : IEntityTypeConfiguration<Farm>
{
    public void Configure(EntityTypeBuilder<Farm> builder)
    {
        builder.ToTable("farm");

        builder.HasKey(farm => farm.Id);
        builder.Property(farm => farm.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(farm => farm.FarmCode).HasColumnName("farm_code").HasMaxLength(32).IsRequired();
        builder.Property(farm => farm.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(farm => farm.DefaultCurrency).HasColumnName("default_currency").HasMaxLength(3).IsRequired();
        builder.Property(farm => farm.TimeZone).HasColumnName("time_zone").HasMaxLength(100).IsRequired();
        builder.Property(farm => farm.Notes).HasColumnName("notes").HasColumnType("text");

        builder.HasIndex(farm => farm.FarmCode).IsUnique();
    }
}
