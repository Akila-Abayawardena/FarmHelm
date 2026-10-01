using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Batches.ChangeBatchStage;
using FarmHelm.Application.Batches.CreateBatch;
using FarmHelm.Application.Batches.RecordBatchMortality;
using FarmHelm.Application.Batches.RecordPlantMortality;
using FarmHelm.Application.Batches.RemoveBatch;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Common;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Tests.Application;

public sealed class BatchUseCaseTests
{
    [Fact]
    public async Task CreateBatchValidGraphUsesGeneratedCodeAndCreatesTrackedPlants()
    {
        var setup = SetupGraph(); var uow = new UnitOfWorkFake(); var batches = new BatchRepositoryFake(); var codes = new BusinessCodeGeneratorFake();
        var handler = CreateHandler(setup, batches, codes, uow);
        var result = await handler.HandleAsync(new CreateBatchRequest(setup.Farm.Id, setup.Variety.Id, setup.Location.Id, new DateOnly(2026, 1, 1), 3, setup.Stage.Id, true));
        var batch = Assert.Single(batches.Added);
        Assert.Equal("BAT-0001", result.BatchCode); Assert.Equal(3, batch.Plants.Count); Assert.Equal([BusinessCodeType.Batch], codes.Requested); Assert.Equal(1, uow.SaveCount);
    }

    [Fact]
    public async Task CreateBatchRejectsCrossAggregateFailuresWithoutSaving()
    {
        var setup = SetupGraph(); var uow = new UnitOfWorkFake(); var batches = new BatchRepositoryFake(); var handler = CreateHandler(setup, batches, new BusinessCodeGeneratorFake(), uow);
        var otherFarm = ApplicationFixture.Farm(); setup.Crops.Items[setup.Crop.Id] = ApplicationFixture.Crop(otherFarm.Id, setup.Crop.Id);
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new CreateBatchRequest(setup.Farm.Id, setup.Variety.Id, null, new DateOnly(2026, 1, 1), 3, setup.Stage.Id, false)));
        Assert.Empty(batches.Added); Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task CreateBatchRejectsInactiveVarietyCropStageAndWrongLocation()
    {
        var setup = SetupGraph(); var uow = new UnitOfWorkFake(); var handler = CreateHandler(setup, new BatchRepositoryFake(), new BusinessCodeGeneratorFake(), uow);
        setup.Variety.Deactivate();
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(Request(setup)));
        setup.Variety.Activate(); setup.Stage.Deactivate();
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(Request(setup)));
        setup.Stage.Activate(); var wrongLocation = ApplicationFixture.Location(Guid.NewGuid()); setup.Locations.Items[wrongLocation.Id] = wrongLocation;
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(Request(setup, wrongLocation.Id)));
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task CreateBatchRejectsInactiveCropAndInvalidStageReferencesWithoutSaving()
    {
        var setup = SetupGraph(); var uow = new UnitOfWorkFake(); var handler = CreateHandler(setup, new BatchRepositoryFake(), new BusinessCodeGeneratorFake(), uow);
        setup.Crop.Deactivate();
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(Request(setup)));
        setup.Crop.Activate();
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(new CreateBatchRequest(setup.Farm.Id, setup.Variety.Id, null, new DateOnly(2026, 1, 1), 3, Guid.NewGuid(), false)));
        var otherCropStage = ApplicationFixture.Stage(Guid.NewGuid()); setup.Stages.Items[otherCropStage.Id] = otherCropStage;
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new CreateBatchRequest(setup.Farm.Id, setup.Variety.Id, null, new DateOnly(2026, 1, 1), 3, otherCropStage.Id, false)));
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task BatchMortalityValidatesReasonAndTrackingBeforeSaving()
    {
        var farm = ApplicationFixture.Farm(); var crop = ApplicationFixture.Crop(farm.Id); var variety = ApplicationFixture.Variety(crop.Id); var stage = ApplicationFixture.Stage(crop.Id); var batch = ApplicationFixture.Batch(farm.Id, variety.Id, stage.Id); var batches = new BatchRepositoryFake(); batches.Items[batch.Id] = batch;
        var reasons = new MortalityReasonRepositoryFake(); var reason = ApplicationFixture.Reason(farm.Id); reasons.Items[reason.Id] = reason; var uow = new UnitOfWorkFake(); var handler = new RecordBatchMortalityHandler(batches, reasons, uow);
        var result = await handler.HandleAsync(new RecordBatchMortalityRequest(batch.Id, new DateOnly(2026, 1, 2), 1, reason.Id));
        Assert.Equal(1, result.DeadPlantCount); Assert.Equal(1, uow.SaveCount);
        var wrongReason = ApplicationFixture.Reason(Guid.NewGuid()); reasons.Items[wrongReason.Id] = wrongReason;
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new RecordBatchMortalityRequest(batch.Id, new DateOnly(2026, 1, 3), 1, wrongReason.Id)));
        Assert.Equal(1, uow.SaveCount);
    }

    [Fact]
    public async Task BatchMortalityRejectsInactiveRemovedAndTrackedBatchesWithoutSaving()
    {
        var farm = ApplicationFixture.Farm(); var crop = ApplicationFixture.Crop(farm.Id); var variety = ApplicationFixture.Variety(crop.Id); var stage = ApplicationFixture.Stage(crop.Id); var reason = ApplicationFixture.Reason(farm.Id);
        var batches = new BatchRepositoryFake(); var reasons = new MortalityReasonRepositoryFake(); reasons.Items[reason.Id] = reason; var uow = new UnitOfWorkFake(); var handler = new RecordBatchMortalityHandler(batches, reasons, uow);
        var tracked = ApplicationFixture.Batch(farm.Id, variety.Id, stage.Id, true); batches.Items[tracked.Id] = tracked;
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new RecordBatchMortalityRequest(tracked.Id, new DateOnly(2026, 1, 2), 1, reason.Id)));
        var removed = ApplicationFixture.Batch(farm.Id, variety.Id, stage.Id); removed.Remove(new DateOnly(2026, 1, 2)); batches.Items[removed.Id] = removed;
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new RecordBatchMortalityRequest(removed.Id, new DateOnly(2026, 1, 3), 1, reason.Id)));
        reason.Deactivate(); var normal = ApplicationFixture.Batch(farm.Id, variety.Id, stage.Id); batches.Items[normal.Id] = normal;
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new RecordBatchMortalityRequest(normal.Id, new DateOnly(2026, 1, 3), 1, reason.Id)));
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task PlantMortalityUsesDomainForInvalidPlantAndSavesOnSuccess()
    {
        var farm = ApplicationFixture.Farm(); var crop = ApplicationFixture.Crop(farm.Id); var variety = ApplicationFixture.Variety(crop.Id); var stage = ApplicationFixture.Stage(crop.Id); var batch = ApplicationFixture.Batch(farm.Id, variety.Id, stage.Id, true); var reason = ApplicationFixture.Reason(farm.Id);
        var batches = new BatchRepositoryFake(); batches.Items[batch.Id] = batch; var reasons = new MortalityReasonRepositoryFake(); reasons.Items[reason.Id] = reason; var uow = new UnitOfWorkFake(); var handler = new RecordPlantMortalityHandler(batches, reasons, uow);
        await Assert.ThrowsAsync<DomainException>(() => handler.HandleAsync(new RecordPlantMortalityRequest(batch.Id, Guid.NewGuid(), new DateOnly(2026, 1, 2), reason.Id)));
        Assert.Equal(0, uow.SaveCount);
        var result = await handler.HandleAsync(new RecordPlantMortalityRequest(batch.Id, batch.Plants[0].Id, new DateOnly(2026, 1, 2), reason.Id));
        Assert.Equal(1, result.DeadPlantCount); Assert.Equal(1, uow.SaveCount);
    }

    [Fact]
    public async Task PlantMortalityRejectsNonIndividualBatchWithoutSaving()
    {
        var farm = ApplicationFixture.Farm(); var crop = ApplicationFixture.Crop(farm.Id); var variety = ApplicationFixture.Variety(crop.Id); var stage = ApplicationFixture.Stage(crop.Id); var batch = ApplicationFixture.Batch(farm.Id, variety.Id, stage.Id); var batches = new BatchRepositoryFake(); batches.Items[batch.Id] = batch; var reasons = new MortalityReasonRepositoryFake(); var reason = ApplicationFixture.Reason(Guid.NewGuid()); reasons.Items[reason.Id] = reason; var uow = new UnitOfWorkFake(); var handler = new RecordPlantMortalityHandler(batches, reasons, uow);
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new RecordPlantMortalityRequest(batch.Id, Guid.NewGuid(), new DateOnly(2026, 1, 2), reason.Id)));
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task PlantMortalityRejectsWrongFarmReasonWithoutSaving()
    {
        var farm = ApplicationFixture.Farm(); var crop = ApplicationFixture.Crop(farm.Id); var variety = ApplicationFixture.Variety(crop.Id); var stage = ApplicationFixture.Stage(crop.Id); var batch = ApplicationFixture.Batch(farm.Id, variety.Id, stage.Id, true);
        var batches = new BatchRepositoryFake(); batches.Items[batch.Id] = batch; var reasons = new MortalityReasonRepositoryFake(); var wrongReason = ApplicationFixture.Reason(Guid.NewGuid()); reasons.Items[wrongReason.Id] = wrongReason; var uow = new UnitOfWorkFake();

        await Assert.ThrowsAsync<ApplicationValidationException>(() => new RecordPlantMortalityHandler(batches, reasons, uow).HandleAsync(new RecordPlantMortalityRequest(batch.Id, batch.Plants[0].Id, new DateOnly(2026, 1, 2), wrongReason.Id)));

        Assert.Equal(0, uow.SaveCount);
        Assert.Equal(PlantStatus.Active, batch.Plants[0].Status);
    }

    [Fact]
    public async Task ChangeStageValidatesCropAndPreservesDomainDuplicateRule()
    {
        var setup = SetupGraph(); var batch = ApplicationFixture.Batch(setup.Farm.Id, setup.Variety.Id, setup.Stage.Id); var batches = new BatchRepositoryFake(); batches.Items[batch.Id] = batch; var uow = new UnitOfWorkFake(); var handler = new ChangeBatchStageHandler(batches, setup.Varieties, setup.Crops, setup.Stages, uow);
        var differentStage = ApplicationFixture.Stage(Guid.NewGuid()); setup.Stages.Items[differentStage.Id] = differentStage;
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new ChangeBatchStageRequest(batch.Id, differentStage.Id, new DateOnly(2026, 2, 1))));
        await Assert.ThrowsAsync<DomainException>(() => handler.HandleAsync(new ChangeBatchStageRequest(batch.Id, setup.Stage.Id, new DateOnly(2026, 2, 1))));
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task ChangeStageRejectsInactiveAndRemovedBatchesWithoutSaving()
    {
        var setup = SetupGraph(); var batch = ApplicationFixture.Batch(setup.Farm.Id, setup.Variety.Id, setup.Stage.Id); var batches = new BatchRepositoryFake(); batches.Items[batch.Id] = batch; var uow = new UnitOfWorkFake(); var handler = new ChangeBatchStageHandler(batches, setup.Varieties, setup.Crops, setup.Stages, uow);
        var nextStage = new CropStage(Guid.NewGuid(), setup.Crop.Id, "Flowering", 2); nextStage.Deactivate(); setup.Stages.Items[nextStage.Id] = nextStage;
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new ChangeBatchStageRequest(batch.Id, nextStage.Id, new DateOnly(2026, 2, 1))));
        batch.Remove(new DateOnly(2026, 2, 1));
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new ChangeBatchStageRequest(batch.Id, setup.Stage.Id, new DateOnly(2026, 2, 2))));
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task ChangeStageAndRemoveSaveOnceAndDomainRejectsRepeatedRemoval()
    {
        var setup = SetupGraph(); var nextStage = new CropStage(Guid.NewGuid(), setup.Crop.Id, "Flowering", 2); setup.Stages.Items[nextStage.Id] = nextStage; var batch = ApplicationFixture.Batch(setup.Farm.Id, setup.Variety.Id, setup.Stage.Id); var batches = new BatchRepositoryFake(); batches.Items[batch.Id] = batch; var uow = new UnitOfWorkFake();
        await new ChangeBatchStageHandler(batches, setup.Varieties, setup.Crops, setup.Stages, uow).HandleAsync(new ChangeBatchStageRequest(batch.Id, nextStage.Id, new DateOnly(2026, 2, 1)));
        await new RemoveBatchHandler(batches, uow).HandleAsync(new RemoveBatchRequest(batch.Id, new DateOnly(2026, 2, 2)));
        await Assert.ThrowsAsync<DomainException>(() => new RemoveBatchHandler(batches, uow).HandleAsync(new RemoveBatchRequest(batch.Id, new DateOnly(2026, 2, 3))));
        Assert.Equal(2, uow.SaveCount);
    }

    [Fact]
    public async Task RemoveBatchRejectsMissingBatchWithoutSaving()
    {
        var unitOfWork = new UnitOfWorkFake();
        var handler = new RemoveBatchHandler(new BatchRepositoryFake(), unitOfWork);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(new RemoveBatchRequest(Guid.NewGuid(), new DateOnly(2026, 2, 2))));

        Assert.Equal(0, unitOfWork.SaveCount);
    }

    private static CreateBatchRequest Request(Graph graph, Guid? locationId = null) => new(graph.Farm.Id, graph.Variety.Id, locationId ?? graph.Location.Id, new DateOnly(2026, 1, 1), 3, graph.Stage.Id, false);
    private static CreateBatchHandler CreateHandler(Graph graph, BatchRepositoryFake batches, BusinessCodeGeneratorFake codes, UnitOfWorkFake uow) => new(graph.Farms, graph.Varieties, graph.Crops, graph.Stages, graph.Locations, batches, codes, uow);
    private static Graph SetupGraph()
    {
        var farm = ApplicationFixture.Farm(); var crop = ApplicationFixture.Crop(farm.Id); var variety = ApplicationFixture.Variety(crop.Id); var stage = ApplicationFixture.Stage(crop.Id); var location = ApplicationFixture.Location(farm.Id);
        var farms = new FarmRepositoryFake(); farms.Items[farm.Id] = farm; var crops = new CropRepositoryFake(); crops.Items[crop.Id] = crop; var varieties = new VarietyRepositoryFake(); varieties.Items[variety.Id] = variety; var stages = new CropStageRepositoryFake(); stages.Items[stage.Id] = stage; var locations = new FarmLocationRepositoryFake(); locations.Items[location.Id] = location;
        return new Graph(farm, crop, variety, stage, location, farms, crops, varieties, stages, locations);
    }
    private sealed record Graph(FarmHelm.Domain.Farms.Farm Farm, Crop Crop, Variety Variety, CropStage Stage, FarmHelm.Domain.Farms.FarmLocation Location, FarmRepositoryFake Farms, CropRepositoryFake Crops, VarietyRepositoryFake Varieties, CropStageRepositoryFake Stages, FarmLocationRepositoryFake Locations);
}
