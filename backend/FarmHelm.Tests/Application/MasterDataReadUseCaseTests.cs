using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Application.MasterData;
using FarmHelm.Application.MasterData.ListCrops;
using FarmHelm.Application.MasterData.ListCropStages;
using FarmHelm.Application.MasterData.ListFarmLocations;
using FarmHelm.Application.MasterData.ListFarms;
using FarmHelm.Application.MasterData.ListMortalityReasons;
using FarmHelm.Application.MasterData.ListVarieties;

namespace FarmHelm.Tests.Application;

public sealed class MasterDataReadUseCaseTests
{
    [Fact]
    public async Task ListFarmsReturnsRepositoryResults()
    {
        var repository = new AgriculturalMasterDataReadRepositoryFake();
        var expected = new[] { new FarmReadModel(Guid.NewGuid(), "FARM-0001", "Main Farm", "LKR", "Asia/Colombo", null) };
        repository.Farms = expected;

        var result = await new ListFarmsHandler(repository).HandleAsync();

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task FarmScopedHandlersRejectMissingFarm()
    {
        var repository = new AgriculturalMasterDataReadRepositoryFake();
        var farmId = Guid.NewGuid();

        await Assert.ThrowsAsync<NotFoundException>(() => new ListFarmLocationsHandler(repository).HandleAsync(farmId, false));
        await Assert.ThrowsAsync<NotFoundException>(() => new ListCropsHandler(repository).HandleAsync(farmId, false));
        await Assert.ThrowsAsync<NotFoundException>(() => new ListMortalityReasonsHandler(repository).HandleAsync(farmId, false));
    }

    [Fact]
    public async Task FarmScopedHandlersReturnResultsAndForwardIncludeInactive()
    {
        var repository = new AgriculturalMasterDataReadRepositoryFake();
        var farmId = Guid.NewGuid();
        repository.FarmIds.Add(farmId);
        repository.Locations = [new FarmLocationReadModel(Guid.NewGuid(), farmId, "LOC-0001", "Field", null, false)];
        repository.Crops = [new CropReadModel(Guid.NewGuid(), farmId, "CROP-0001", "Pepper", null, null, false)];
        repository.Reasons = [new MortalityReasonReadModel(Guid.NewGuid(), farmId, "Disease", 1, false)];

        Assert.Single(await new ListFarmLocationsHandler(repository).HandleAsync(farmId, true));
        Assert.Equal((farmId, true), repository.LastFarmScopedRequest);
        Assert.Single(await new ListCropsHandler(repository).HandleAsync(farmId, true));
        Assert.Equal((farmId, true), repository.LastFarmScopedRequest);
        Assert.Single(await new ListMortalityReasonsHandler(repository).HandleAsync(farmId, true));
        Assert.Equal((farmId, true), repository.LastFarmScopedRequest);
    }

    [Fact]
    public async Task CropScopedHandlersRejectMissingCrop()
    {
        var repository = new AgriculturalMasterDataReadRepositoryFake();
        var cropId = Guid.NewGuid();

        await Assert.ThrowsAsync<NotFoundException>(() => new ListVarietiesHandler(repository).HandleAsync(cropId, false));
        await Assert.ThrowsAsync<NotFoundException>(() => new ListCropStagesHandler(repository).HandleAsync(cropId, false));
    }

    [Fact]
    public async Task CropScopedHandlersReturnResultsAndForwardIncludeInactive()
    {
        var repository = new AgriculturalMasterDataReadRepositoryFake();
        var cropId = Guid.NewGuid();
        repository.CropIds.Add(cropId);
        repository.Varieties = [new VarietyReadModel(Guid.NewGuid(), cropId, "VAR-0001", "Variety", null, false)];
        repository.Stages = [new CropStageReadModel(Guid.NewGuid(), cropId, "Vegetative", 1, false)];

        Assert.Single(await new ListVarietiesHandler(repository).HandleAsync(cropId, true));
        Assert.Equal((cropId, true), repository.LastCropScopedRequest);
        Assert.Single(await new ListCropStagesHandler(repository).HandleAsync(cropId, true));
        Assert.Equal((cropId, true), repository.LastCropScopedRequest);
    }
}
