using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Application.Crops.CreateCrop;
using FarmHelm.Application.Crops.CreateCropStage;
using FarmHelm.Application.Crops.CreateMortalityReason;
using FarmHelm.Application.Crops.CreateVariety;
using FarmHelm.Application.Farms.CreateFarm;
using FarmHelm.Application.Farms.CreateFarmLocation;

namespace FarmHelm.Tests.Application;

public sealed class MasterDataUseCaseTests
{
    [Fact]
    public async Task CreateFarmUsesDefaultsGeneratedCodeAndSavesOnce()
    {
        var farms = new FarmRepositoryFake(); var codes = new BusinessCodeGeneratorFake(); var unitOfWork = new UnitOfWorkFake();
        var result = await new CreateFarmHandler(farms, codes, unitOfWork).HandleAsync(new CreateFarmRequest("Main Farm"));
        var farm = Assert.Single(farms.Added);
        Assert.Equal("LKR", farm.DefaultCurrency); Assert.Equal("Asia/Colombo", farm.TimeZone); Assert.Equal("FARM-0001", result.FarmCode);
        Assert.Equal([BusinessCodeType.Farm], codes.Requested); Assert.Equal(1, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task CreateLocationRejectsMissingFarmAndDoesNotSave()
    {
        var unitOfWork = new UnitOfWorkFake();
        var handler = new CreateFarmLocationHandler(new FarmRepositoryFake(), new FarmLocationRepositoryFake(), new BusinessCodeGeneratorFake(), unitOfWork);
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(new CreateFarmLocationRequest(Guid.NewGuid(), "Field")));
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task CreateLocationCreatesForExistingFarm()
    {
        var farm = ApplicationFixture.Farm(); var farms = new FarmRepositoryFake(); farms.Items[farm.Id] = farm; var locations = new FarmLocationRepositoryFake(); var uow = new UnitOfWorkFake();
        var result = await new CreateFarmLocationHandler(farms, locations, new BusinessCodeGeneratorFake(), uow).HandleAsync(new CreateFarmLocationRequest(farm.Id, "Field A"));
        Assert.Equal("LOC-0001", result.LocationCode); Assert.Single(locations.Added); Assert.Equal(1, uow.SaveCount);
    }

    [Fact]
    public async Task CreateCropRejectsMissingFarm()
    {
        var uow = new UnitOfWorkFake();
        var handler = new CreateCropHandler(new FarmRepositoryFake(), new CropRepositoryFake(), new BusinessCodeGeneratorFake(), uow);
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(new CreateCropRequest(Guid.NewGuid(), "Pepper")));
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task CreateCropCreatesForExistingFarm()
    {
        var farm = ApplicationFixture.Farm(); var farms = new FarmRepositoryFake(); farms.Items[farm.Id] = farm; var crops = new CropRepositoryFake(); var uow = new UnitOfWorkFake();
        var result = await new CreateCropHandler(farms, crops, new BusinessCodeGeneratorFake(), uow).HandleAsync(new CreateCropRequest(farm.Id, "Pepper"));
        Assert.Equal("CROP-0001", result.CropCode); Assert.Single(crops.Added); Assert.Equal(1, uow.SaveCount);
    }

    [Fact]
    public async Task CreateVarietyRejectsMissingOrInactiveCrop()
    {
        var crops = new CropRepositoryFake(); var uow = new UnitOfWorkFake(); var handler = new CreateVarietyHandler(crops, new VarietyRepositoryFake(), new BusinessCodeGeneratorFake(), uow);
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(new CreateVarietyRequest(Guid.NewGuid(), "Variety")));
        var crop = ApplicationFixture.Crop(Guid.NewGuid()); crop.Deactivate(); crops.Items[crop.Id] = crop;
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new CreateVarietyRequest(crop.Id, "Variety")));
        Assert.Equal(0, uow.SaveCount);
    }

    [Fact]
    public async Task CreateVarietyCreatesForActiveCrop()
    {
        var crop = ApplicationFixture.Crop(Guid.NewGuid()); var crops = new CropRepositoryFake(); crops.Items[crop.Id] = crop; var varieties = new VarietyRepositoryFake(); var uow = new UnitOfWorkFake();
        var result = await new CreateVarietyHandler(crops, varieties, new BusinessCodeGeneratorFake(), uow).HandleAsync(new CreateVarietyRequest(crop.Id, "Scotch Bonnet"));
        Assert.Equal("VAR-0001", result.VarietyCode); Assert.Single(varieties.Added); Assert.Equal(1, uow.SaveCount);
    }

    [Fact]
    public async Task CreateStageRejectsMissingOrInactiveCropAndCreatesForActiveCrop()
    {
        var crops = new CropRepositoryFake(); var stages = new CropStageRepositoryFake(); var uow = new UnitOfWorkFake(); var handler = new CreateCropStageHandler(crops, stages, uow);
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(new CreateCropStageRequest(Guid.NewGuid(), "Vegetative", 1)));
        var crop = ApplicationFixture.Crop(Guid.NewGuid()); crop.Deactivate(); crops.Items[crop.Id] = crop;
        await Assert.ThrowsAsync<ApplicationValidationException>(() => handler.HandleAsync(new CreateCropStageRequest(crop.Id, "Vegetative", 1)));
        crop.Activate();
        var result = await handler.HandleAsync(new CreateCropStageRequest(crop.Id, "Vegetative", 1));
        Assert.Equal(crop.Id, result.CropId); Assert.Single(stages.Added); Assert.Equal(1, uow.SaveCount);
    }

    [Fact]
    public async Task CreateMortalityReasonRejectsMissingFarmAndCreatesForExistingFarm()
    {
        var farms = new FarmRepositoryFake(); var reasons = new MortalityReasonRepositoryFake(); var uow = new UnitOfWorkFake(); var handler = new CreateMortalityReasonHandler(farms, reasons, uow);
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(new CreateMortalityReasonRequest(Guid.NewGuid(), "Disease", 1)));
        var farm = ApplicationFixture.Farm(); farms.Items[farm.Id] = farm;
        var result = await handler.HandleAsync(new CreateMortalityReasonRequest(farm.Id, "Disease", 1));
        Assert.Equal(farm.Id, result.FarmId); Assert.Single(reasons.Added); Assert.Equal(1, uow.SaveCount);
    }
}
