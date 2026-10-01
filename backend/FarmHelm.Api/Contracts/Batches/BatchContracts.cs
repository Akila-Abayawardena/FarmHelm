using System.ComponentModel.DataAnnotations;

namespace FarmHelm.Api.Contracts.Batches;

public sealed record CreateBatchRequest(
    Guid FarmId,
    Guid VarietyId,
    Guid? LocationId,
    DateOnly PlantingDate,
    [param: Range(1, int.MaxValue)] int InitialPlantCount,
    Guid InitialStageId,
    bool IndividualTrackingEnabled,
    string? Notes = null);

public sealed record BatchCreatedResponse(Guid BatchId, string BatchCode, int InitialPlantCount, bool IndividualTrackingEnabled);

public sealed record RecordBatchMortalityRequest(
    DateOnly MortalityDate,
    [param: Range(1, int.MaxValue)] int Quantity,
    Guid MortalityReasonId,
    string? Notes = null);

public sealed record RecordPlantMortalityRequest(DateOnly MortalityDate, Guid MortalityReasonId, string? Notes = null);
public sealed record ChangeBatchStageRequest(Guid StageId, DateOnly EffectiveDate, string? Notes = null);
public sealed record RemoveBatchRequest(DateOnly RemovedDate);
public sealed record BatchMortalityResponse(Guid BatchId, int DeadPlantCount, int AlivePlantCount, decimal SurvivalRate);

public sealed record BatchSummaryResponse(Guid BatchId, string BatchCode, Guid FarmId, Guid CropId, Guid VarietyId, string VarietyName, DateOnly PlantingDate, string Status);
public sealed record PlantResponse(Guid PlantId, int PlantNumber, string PlantCode, string Status, string? Notes);
public sealed record MortalityRecordResponse(Guid MortalityRecordId, Guid? PlantId, DateOnly MortalityDate, int Quantity, Guid ReasonId, string? Notes);
public sealed record BatchStageHistoryResponse(Guid HistoryId, Guid StageId, string? StageName, DateOnly EffectiveDate, string? Notes);

public sealed record BatchDetailsResponse(
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
    string Status,
    DateOnly? RemovedDate,
    string? Notes,
    IReadOnlyList<PlantResponse> Plants,
    IReadOnlyList<MortalityRecordResponse> MortalityRecords,
    IReadOnlyList<BatchStageHistoryResponse> StageHistory);
