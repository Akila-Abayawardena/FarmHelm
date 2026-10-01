using FarmHelm.Domain.Crops;
using FarmHelm.Domain.Farms;

namespace FarmHelm.Application.Abstractions.Persistence;

public interface IFarmRepository
{
    Task<Farm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Farm farm, CancellationToken cancellationToken = default);
}

public interface IFarmLocationRepository
{
    Task<FarmLocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(FarmLocation location, CancellationToken cancellationToken = default);
}

public interface ICropRepository
{
    Task<Crop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Crop crop, CancellationToken cancellationToken = default);
}

public interface IVarietyRepository
{
    Task<Variety?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Variety variety, CancellationToken cancellationToken = default);
}

public interface ICropStageRepository
{
    Task<CropStage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(CropStage cropStage, CancellationToken cancellationToken = default);
}

public interface IMortalityReasonRepository
{
    Task<MortalityReason?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(MortalityReason mortalityReason, CancellationToken cancellationToken = default);
}

public interface IBatchRepository
{
    /// <summary>Returns the complete Batch aggregate, including Plants, MortalityRecords, and StageHistory.</summary>
    Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Batch batch, CancellationToken cancellationToken = default);
}
