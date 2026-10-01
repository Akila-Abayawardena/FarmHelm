using Microsoft.EntityFrameworkCore;
using FarmHelm.Domain.Crops;
using FarmHelm.Domain.Farms;

namespace FarmHelm.Infrastructure.Persistence;

public sealed class FarmHelmDbContext(DbContextOptions<FarmHelmDbContext> options) : DbContext(options)
{
    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<FarmLocation> FarmLocations => Set<FarmLocation>();
    public DbSet<Crop> Crops => Set<Crop>();
    public DbSet<Variety> Varieties => Set<Variety>();
    public DbSet<CropStage> CropStages => Set<CropStage>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<MortalityReason> MortalityReasons => Set<MortalityReason>();
    public DbSet<MortalityRecord> MortalityRecords => Set<MortalityRecord>();
    public DbSet<BatchStageHistory> BatchStageHistories => Set<BatchStageHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FarmHelmDbContext).Assembly);
        modelBuilder.HasSequence<long>("farm_code_seq").StartsAt(1).IncrementsBy(1);
        modelBuilder.HasSequence<long>("farm_location_code_seq").StartsAt(1).IncrementsBy(1);
        modelBuilder.HasSequence<long>("crop_code_seq").StartsAt(1).IncrementsBy(1);
        modelBuilder.HasSequence<long>("variety_code_seq").StartsAt(1).IncrementsBy(1);
        modelBuilder.HasSequence<long>("batch_code_seq").StartsAt(1).IncrementsBy(1);
    }
}
