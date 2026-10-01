using FarmHelm.Domain.Crops;

namespace FarmHelm.Application.Batches;

public sealed record BatchListFilter(Guid? FarmId = null, Guid? CropId = null, Guid? VarietyId = null, BatchStatus? Status = null);

public sealed record BatchSummary(Guid BatchId, string BatchCode, Guid FarmId, Guid CropId, Guid VarietyId, string VarietyName, DateOnly PlantingDate, BatchStatus Status);

public sealed record PlantDetails(Guid PlantId, int PlantNumber, string PlantCode, PlantStatus Status, string? Notes);
public sealed record MortalityRecordDetails(Guid MortalityRecordId, Guid? PlantId, DateOnly MortalityDate, int Quantity, Guid ReasonId, string? Notes);
public sealed record BatchStageHistoryDetails(Guid HistoryId, Guid StageId, string? StageName, DateOnly EffectiveDate, string? Notes);

public sealed record BatchDetails(
    Guid BatchId,
    string BatchCode,
    Guid CropId,
    string CropName,
    Guid VarietyId,
    string VarietyName,
    Guid FarmId,
    string FarmName,
    Guid? LocationId,
    string? LocationName,
    DateOnly PlantingDate,
    int InitialPlantCount,
    int AlivePlantCount,
    int DeadPlantCount,
    decimal SurvivalRate,
    bool IndividualTrackingEnabled,
    Guid CurrentStageId,
    string CurrentStageName,
    BatchStatus Status,
    DateOnly? RemovedDate,
    string? Notes,
    IReadOnlyList<PlantDetails> Plants,
    IReadOnlyList<MortalityRecordDetails> MortalityRecords,
    IReadOnlyList<BatchStageHistoryDetails> StageHistory);

public interface IBatchReadRepository
{
    Task<BatchDetails?> GetDetailsByIdAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BatchSummary>> ListAsync(BatchListFilter filter, CancellationToken cancellationToken = default);
}
