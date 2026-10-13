using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.MasterData;
using Microsoft.EntityFrameworkCore;

namespace FarmHelm.Infrastructure.Persistence.Repositories;

public sealed class AgriculturalMasterDataReadRepository(FarmHelmDbContext dbContext) : IAgriculturalMasterDataReadRepository
{
    public Task<bool> FarmExistsAsync(Guid farmId, CancellationToken cancellationToken = default) =>
        dbContext.Farms.AsNoTracking().AnyAsync(farm => farm.Id == farmId, cancellationToken);

    public Task<bool> CropExistsAsync(Guid cropId, CancellationToken cancellationToken = default) =>
        dbContext.Crops.AsNoTracking().AnyAsync(crop => crop.Id == cropId, cancellationToken);

    public async Task<IReadOnlyList<FarmReadModel>> ListFarmsAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Farms
            .AsNoTracking()
            .OrderBy(farm => farm.Name)
            .ThenBy(farm => farm.FarmCode)
            .Select(farm => new FarmReadModel(farm.Id, farm.FarmCode, farm.Name, farm.DefaultCurrency, farm.TimeZone, farm.Notes))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<FarmLocationReadModel>> ListFarmLocationsAsync(Guid farmId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.FarmLocations.AsNoTracking().Where(location => location.FarmId == farmId);
        if (!includeInactive) query = query.Where(location => location.IsActive);

        return await query
            .OrderBy(location => location.Name)
            .ThenBy(location => location.LocationCode)
            .Select(location => new FarmLocationReadModel(location.Id, location.FarmId, location.LocationCode, location.Name, location.Description, location.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CropReadModel>> ListCropsAsync(Guid farmId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Crops.AsNoTracking().Where(crop => crop.FarmId == farmId);
        if (!includeInactive) query = query.Where(crop => crop.IsActive);

        return await query
            .OrderBy(crop => crop.Name)
            .ThenBy(crop => crop.CropCode)
            .Select(crop => new CropReadModel(crop.Id, crop.FarmId, crop.CropCode, crop.Name, crop.ScientificName, crop.Description, crop.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VarietyReadModel>> ListVarietiesAsync(Guid cropId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Varieties.AsNoTracking().Where(variety => variety.CropId == cropId);
        if (!includeInactive) query = query.Where(variety => variety.IsActive);

        return await query
            .OrderBy(variety => variety.Name)
            .ThenBy(variety => variety.VarietyCode)
            .Select(variety => new VarietyReadModel(variety.Id, variety.CropId, variety.VarietyCode, variety.Name, variety.Description, variety.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CropStageReadModel>> ListCropStagesAsync(Guid cropId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.CropStages.AsNoTracking().Where(stage => stage.CropId == cropId);
        if (!includeInactive) query = query.Where(stage => stage.IsActive);

        return await query
            .OrderBy(stage => stage.DisplayOrder)
            .ThenBy(stage => stage.Name)
            .Select(stage => new CropStageReadModel(stage.Id, stage.CropId, stage.Name, stage.DisplayOrder, stage.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MortalityReasonReadModel>> ListMortalityReasonsAsync(Guid farmId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.MortalityReasons.AsNoTracking().Where(reason => reason.FarmId == farmId);
        if (!includeInactive) query = query.Where(reason => reason.IsActive);

        return await query
            .OrderBy(reason => reason.DisplayOrder)
            .ThenBy(reason => reason.Name)
            .Select(reason => new MortalityReasonReadModel(reason.Id, reason.FarmId, reason.Name, reason.DisplayOrder, reason.IsActive))
            .ToListAsync(cancellationToken);
    }
}
