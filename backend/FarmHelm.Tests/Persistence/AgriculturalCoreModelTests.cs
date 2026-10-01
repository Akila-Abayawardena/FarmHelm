using FarmHelm.Domain.Crops;
using FarmHelm.Domain.Farms;
using FarmHelm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FarmHelm.Tests.Persistence;

public sealed class AgriculturalCoreModelTests
{
    [Theory]
    [InlineData(typeof(Farm), "farm")]
    [InlineData(typeof(FarmLocation), "farm_location")]
    [InlineData(typeof(Crop), "crop")]
    [InlineData(typeof(Variety), "variety")]
    [InlineData(typeof(CropStage), "crop_stage")]
    [InlineData(typeof(Batch), "batch")]
    [InlineData(typeof(Plant), "plant")]
    [InlineData(typeof(MortalityReason), "mortality_reason")]
    [InlineData(typeof(MortalityRecord), "mortality_record")]
    [InlineData(typeof(BatchStageHistory), "batch_stage_history")]
    public void AgriculturalCoreEntityIsMappedToExpectedTable(Type entityType, string expectedTableName)
    {
        using var context = CreateContext();
        var entity = context.Model.FindEntityType(entityType);

        Assert.NotNull(entity);
        Assert.Equal(expectedTableName, entity.GetTableName());
        var primaryKey = entity.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Equal(nameof(Farm.Id), Assert.Single(primaryKey.Properties).Name);
        Assert.Equal(ValueGenerated.Never, Assert.Single(primaryKey.Properties).ValueGenerated);
    }

    [Fact]
    public void BatchDerivedValuesAreNotMappedAndStatusUsesStringConversion()
    {
        using var context = CreateContext();
        var batch = context.Model.FindEntityType(typeof(Batch))!;
        var status = batch.FindProperty(nameof(Batch.Status))!;

        Assert.Null(batch.FindProperty(nameof(Batch.DeadPlantCount)));
        Assert.Null(batch.FindProperty(nameof(Batch.AlivePlantCount)));
        Assert.Null(batch.FindProperty(nameof(Batch.SurvivalRate)));
        Assert.Equal(typeof(string), status.GetTypeMapping().Converter!.ProviderClrType);
    }

    [Fact]
    public void PlantStatusUsesStringConversion()
    {
        using var context = CreateContext();
        var plant = context.Model.FindEntityType(typeof(Plant))!;
        var status = plant.FindProperty(nameof(Plant.Status))!;

        Assert.Equal(typeof(string), status.GetTypeMapping().Converter!.ProviderClrType);
    }

    [Fact]
    public void ImportantConstraintsIndexesAndForeignKeysArePresent()
    {
        using var context = CreateContext();
        var model = context.GetService<IDesignTimeModel>().Model;
        var batch = model.FindEntityType(typeof(Batch))!;
        var mortalityRecord = model.FindEntityType(typeof(MortalityRecord))!;

        Assert.Contains(
            batch.GetCheckConstraints(),
            constraint => constraint.Name == "ck_batch_initial_plant_count_positive" &&
                          constraint.Sql == "initial_plant_count > 0");
        Assert.True(HasUniqueIndex(batch, nameof(Batch.BatchCode)));
        Assert.True(HasUniqueIndex(model.FindEntityType(typeof(Farm))!, nameof(Farm.FarmCode)));
        Assert.True(HasUniqueIndex(model.FindEntityType(typeof(Plant))!, nameof(Plant.PlantCode)));
        Assert.True(HasUniqueIndex(mortalityRecord, nameof(MortalityRecord.PlantId)));

        Assert.Contains(
            batch.GetForeignKeys(),
            foreignKey => foreignKey.Properties.Select(property => property.Name).SequenceEqual([nameof(Batch.FarmId)]) &&
                          foreignKey.PrincipalEntityType.ClrType == typeof(Farm) &&
                          foreignKey.DeleteBehavior == DeleteBehavior.Restrict);
        Assert.Contains(
            mortalityRecord.GetForeignKeys(),
            foreignKey => foreignKey.Properties.Select(property => property.Name).SequenceEqual([nameof(MortalityRecord.PlantId), nameof(MortalityRecord.BatchId)]) &&
                          foreignKey.PrincipalEntityType.ClrType == typeof(Plant) &&
                          foreignKey.DeleteBehavior == DeleteBehavior.Restrict);
        Assert.DoesNotContain(
            model.GetEntityTypes().SelectMany(entity => entity.GetForeignKeys()),
            foreignKey => foreignKey.DeleteBehavior == DeleteBehavior.Cascade);
    }

    [Fact]
    public void AddingBatchDiscoversFieldBackedAggregateChildren()
    {
        using var context = CreateContext();
        var batch = Batch.Create(
            Guid.NewGuid(),
            "BAT-0001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            new DateOnly(2026, 1, 1),
            3,
            Guid.NewGuid(),
            individualTrackingEnabled: true);

        context.Add(batch);

        Assert.Equal(EntityState.Added, context.Entry(batch).State);
        Assert.Equal(3, context.ChangeTracker.Entries<Plant>().Count());
        Assert.Single(context.ChangeTracker.Entries<BatchStageHistory>());
    }

    private static FarmHelmDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FarmHelmDbContext>()
            .UseNpgsql("Host=example.invalid;Database=farmhelm_model_test")
            .Options;

        return new FarmHelmDbContext(options);
    }

    private static bool HasUniqueIndex(IReadOnlyEntityType entityType, params string[] propertyNames) =>
        entityType.GetIndexes().Any(index =>
            index.IsUnique &&
            index.Properties.Select(property => property.Name).SequenceEqual(propertyNames));
}
