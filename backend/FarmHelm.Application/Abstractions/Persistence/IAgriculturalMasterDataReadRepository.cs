using FarmHelm.Application.MasterData;

namespace FarmHelm.Application.Abstractions.Persistence;

/// <summary>
/// Provides read-only, selector-oriented Agricultural Core master data.
/// </summary>
public interface IAgriculturalMasterDataReadRepository
{
    Task<bool> FarmExistsAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<bool> CropExistsAsync(Guid cropId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FarmReadModel>> ListFarmsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FarmLocationReadModel>> ListFarmLocationsAsync(Guid farmId, bool includeInactive, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CropReadModel>> ListCropsAsync(Guid farmId, bool includeInactive, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VarietyReadModel>> ListVarietiesAsync(Guid cropId, bool includeInactive, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CropStageReadModel>> ListCropStagesAsync(Guid cropId, bool includeInactive, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MortalityReasonReadModel>> ListMortalityReasonsAsync(Guid farmId, bool includeInactive, CancellationToken cancellationToken = default);
}
