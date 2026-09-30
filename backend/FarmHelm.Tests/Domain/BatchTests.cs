using FarmHelm.Domain.Common;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Tests.Domain;

public sealed class BatchTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateRequiresPositiveInitialPlantCount(int initialPlantCount)
    {
        Assert.Throws<DomainException>(() => CreateBatch(initialPlantCount: initialPlantCount));
    }

    [Fact]
    public void CreateRetainsInitialCountSetsInitialStageAndCreatesHistory()
    {
        var initialStageId = Guid.NewGuid();
        var batch = CreateBatch(initialPlantCount: 10, initialStageId: initialStageId);

        Assert.Equal(10, batch.InitialPlantCount);
        Assert.Equal(initialStageId, batch.CurrentStageId);
        Assert.Equal(BatchStatus.Active, batch.Status);
        var history = Assert.Single(batch.StageHistory);
        Assert.Equal(initialStageId, history.StageId);
        Assert.Equal(batch.PlantingDate, history.EffectiveDate);
    }

    [Fact]
    public void CalculatedCountsStartAtFullSurvival()
    {
        var batch = CreateBatch(initialPlantCount: 10);

        Assert.Equal(0, batch.DeadPlantCount);
        Assert.Equal(10, batch.AlivePlantCount);
        Assert.Equal(100m, batch.SurvivalRate);
    }

    [Fact]
    public void QuantityMortalityUpdatesCalculatedCounts()
    {
        var batch = CreateBatch(initialPlantCount: 10);
        var record = batch.RecordMortality(Guid.NewGuid(), new DateOnly(2026, 1, 2), 2, Guid.NewGuid());

        Assert.Equal(2, record.Quantity);
        Assert.Null(record.PlantId);
        Assert.Equal(2, batch.DeadPlantCount);
        Assert.Equal(8, batch.AlivePlantCount);
        Assert.Equal(80m, batch.SurvivalRate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void QuantityMortalityRequiresPositiveQuantity(int quantity)
    {
        var batch = CreateBatch();

        Assert.Throws<DomainException>(() =>
            batch.RecordMortality(Guid.NewGuid(), new DateOnly(2026, 1, 2), quantity, Guid.NewGuid()));
    }

    [Fact]
    public void QuantityMortalityCannotExceedLivingPlantCountAndDoesNotMutateOnFailure()
    {
        var batch = CreateBatch(initialPlantCount: 10);
        batch.RecordMortality(Guid.NewGuid(), new DateOnly(2026, 1, 2), 2, Guid.NewGuid());

        Assert.Throws<DomainException>(() =>
            batch.RecordMortality(Guid.NewGuid(), new DateOnly(2026, 1, 3), 9, Guid.NewGuid()));

        Assert.Equal(2, batch.DeadPlantCount);
        Assert.Equal(8, batch.AlivePlantCount);
        Assert.Single(batch.MortalityRecords);
    }

    [Fact]
    public void IndividuallyTrackedBatchCreatesExactlyOneUniquePlantPerInitialCount()
    {
        var batch = CreateBatch(individualTrackingEnabled: true, initialPlantCount: 3);

        Assert.Equal(3, batch.Plants.Count);
        Assert.Equal(3, batch.Plants.Select(plant => plant.PlantNumber).Distinct().Count());
        Assert.Equal(3, batch.Plants.Select(plant => plant.PlantCode).Distinct().Count());
    }

    [Fact]
    public void IndividualPlantMortalityMarksPlantDeadAndCreatesRecord()
    {
        var batch = CreateBatch(individualTrackingEnabled: true, initialPlantCount: 2);
        var plant = batch.Plants[0];

        var record = batch.RecordPlantMortality(
            Guid.NewGuid(),
            plant.Id,
            new DateOnly(2026, 1, 2),
            Guid.NewGuid());

        Assert.Equal(PlantStatus.Dead, plant.Status);
        Assert.Equal(plant.Id, record.PlantId);
        Assert.Equal(1, record.Quantity);
        Assert.Equal(1, batch.DeadPlantCount);
        Assert.Single(batch.MortalityRecords);
    }

    [Fact]
    public void IndividualPlantCannotBeRecordedDeadTwice()
    {
        var batch = CreateBatch(individualTrackingEnabled: true);
        var plant = batch.Plants[0];
        batch.RecordPlantMortality(Guid.NewGuid(), plant.Id, new DateOnly(2026, 1, 2), Guid.NewGuid());

        Assert.Throws<DomainException>(() =>
            batch.RecordPlantMortality(Guid.NewGuid(), plant.Id, new DateOnly(2026, 1, 3), Guid.NewGuid()));
        Assert.Single(batch.MortalityRecords);
    }

    [Fact]
    public void IndividualPlantMortalityRejectsPlantFromAnotherBatch()
    {
        var batch = CreateBatch(individualTrackingEnabled: true);
        var otherBatch = CreateBatch(individualTrackingEnabled: true);

        Assert.Throws<DomainException>(() =>
            batch.RecordPlantMortality(Guid.NewGuid(), otherBatch.Plants[0].Id, new DateOnly(2026, 1, 2), Guid.NewGuid()));
        Assert.Empty(batch.MortalityRecords);
    }

    [Fact]
    public void QuantityMortalityIsRejectedWhenIndividualTrackingIsEnabled()
    {
        var batch = CreateBatch(individualTrackingEnabled: true);

        Assert.Throws<DomainException>(() =>
            batch.RecordMortality(Guid.NewGuid(), new DateOnly(2026, 1, 2), 1, Guid.NewGuid()));
    }

    [Fact]
    public void IndividualPlantMortalityIsRejectedWhenIndividualTrackingIsDisabled()
    {
        var batch = CreateBatch(individualTrackingEnabled: false);

        Assert.Throws<DomainException>(() =>
            batch.RecordPlantMortality(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 2), Guid.NewGuid()));
    }

    [Fact]
    public void ChangeStageUpdatesCurrentStageAndAppendsHistory()
    {
        var initialStageId = Guid.NewGuid();
        var nextStageId = Guid.NewGuid();
        var batch = CreateBatch(initialStageId: initialStageId);

        batch.ChangeStage(nextStageId, new DateOnly(2026, 2, 1), "Flowering started");

        Assert.Equal(nextStageId, batch.CurrentStageId);
        Assert.Equal(2, batch.StageHistory.Count);
        Assert.Equal(initialStageId, batch.StageHistory[0].StageId);
        Assert.Equal(nextStageId, batch.StageHistory[1].StageId);
    }

    [Fact]
    public void ChangeStageRejectsCurrentStage()
    {
        var initialStageId = Guid.NewGuid();
        var batch = CreateBatch(initialStageId: initialStageId);

        Assert.Throws<DomainException>(() => batch.ChangeStage(initialStageId, new DateOnly(2026, 2, 1)));
        Assert.Single(batch.StageHistory);
    }

    [Fact]
    public void RemovePreservesBatchAndRejectsSecondRemoval()
    {
        var batch = CreateBatch();
        var removedDate = new DateOnly(2026, 2, 1);

        batch.Remove(removedDate);

        Assert.Equal(BatchStatus.Removed, batch.Status);
        Assert.Equal(removedDate, batch.RemovedDate);
        Assert.Throws<DomainException>(() => batch.Remove(new DateOnly(2026, 2, 2)));
    }

    private static Batch CreateBatch(
        bool individualTrackingEnabled = false,
        int initialPlantCount = 10,
        Guid? initialStageId = null) =>
        Batch.Create(
            Guid.NewGuid(),
            "BAT-0001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            new DateOnly(2026, 1, 1),
            initialPlantCount,
            initialStageId ?? Guid.NewGuid(),
            individualTrackingEnabled);
}
