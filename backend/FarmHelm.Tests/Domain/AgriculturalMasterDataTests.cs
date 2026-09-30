using FarmHelm.Domain.Common;
using FarmHelm.Domain.Crops;
using FarmHelm.Domain.Farms;

namespace FarmHelm.Tests.Domain;

public sealed class AgriculturalMasterDataTests
{
    [Fact]
    public void FarmRequiresIdentityCodeAndName()
    {
        Assert.Throws<DomainException>(() => new Farm(Guid.Empty, "FARM-0001", "Main Farm", "LKR", "Asia/Colombo", null));
        Assert.Throws<DomainException>(() => new Farm(Guid.NewGuid(), " ", "Main Farm", "LKR", "Asia/Colombo", null));
        Assert.Throws<DomainException>(() => new Farm(Guid.NewGuid(), "FARM-0001", " ", "LKR", "Asia/Colombo", null));
    }

    [Fact]
    public void FarmLocationRequiresFarmIdentityCodeAndName()
    {
        Assert.Throws<DomainException>(() => new FarmLocation(Guid.NewGuid(), Guid.Empty, "LOC-0001", "Field A"));
        Assert.Throws<DomainException>(() => new FarmLocation(Guid.NewGuid(), Guid.NewGuid(), " ", "Field A"));
        Assert.Throws<DomainException>(() => new FarmLocation(Guid.NewGuid(), Guid.NewGuid(), "LOC-0001", " "));
    }

    [Fact]
    public void CropRequiresFarmIdentityCodeAndName()
    {
        Assert.Throws<DomainException>(() => new Crop(Guid.NewGuid(), Guid.Empty, "CROP-0001", "Pepper"));
        Assert.Throws<DomainException>(() => new Crop(Guid.NewGuid(), Guid.NewGuid(), " ", "Pepper"));
        Assert.Throws<DomainException>(() => new Crop(Guid.NewGuid(), Guid.NewGuid(), "CROP-0001", " "));
    }

    [Fact]
    public void VarietyRequiresCropIdentityCodeAndName()
    {
        Assert.Throws<DomainException>(() => new Variety(Guid.NewGuid(), Guid.Empty, "VAR-0001", "Scotch Bonnet"));
        Assert.Throws<DomainException>(() => new Variety(Guid.NewGuid(), Guid.NewGuid(), " ", "Scotch Bonnet"));
        Assert.Throws<DomainException>(() => new Variety(Guid.NewGuid(), Guid.NewGuid(), "VAR-0001", " "));
    }

    [Fact]
    public void CropStageRequiresCropIdentityNameAndNonNegativeDisplayOrder()
    {
        Assert.Throws<DomainException>(() => new CropStage(Guid.NewGuid(), Guid.Empty, "Vegetative", 0));
        Assert.Throws<DomainException>(() => new CropStage(Guid.NewGuid(), Guid.NewGuid(), " ", 0));
        Assert.Throws<DomainException>(() => new CropStage(Guid.NewGuid(), Guid.NewGuid(), "Vegetative", -1));
    }

    [Fact]
    public void MortalityReasonRequiresFarmIdentityNameAndNonNegativeDisplayOrder()
    {
        Assert.Throws<DomainException>(() => new MortalityReason(Guid.NewGuid(), Guid.Empty, "Disease", 0));
        Assert.Throws<DomainException>(() => new MortalityReason(Guid.NewGuid(), Guid.NewGuid(), " ", 0));
        Assert.Throws<DomainException>(() => new MortalityReason(Guid.NewGuid(), Guid.NewGuid(), "Disease", -1));
    }

    [Fact]
    public void DeactivationRetainsMasterDataIdentity()
    {
        var cropId = Guid.NewGuid();
        var crop = new Crop(cropId, Guid.NewGuid(), "CROP-0001", "Pepper");

        crop.Deactivate();

        Assert.False(crop.IsActive);
        Assert.Equal(cropId, crop.Id);
        Assert.Equal("CROP-0001", crop.CropCode);

        crop.Activate();

        Assert.True(crop.IsActive);
    }
}
