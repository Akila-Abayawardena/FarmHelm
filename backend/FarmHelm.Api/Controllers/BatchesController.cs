using FarmHelm.Api.Contracts.Batches;
using FarmHelm.Application.Batches;
using FarmHelm.Domain.Crops;
using Microsoft.AspNetCore.Mvc;
using ChangeBatchStageHandler = FarmHelm.Application.Batches.ChangeBatchStage.ChangeBatchStageHandler;
using CreateBatchHandler = FarmHelm.Application.Batches.CreateBatch.CreateBatchHandler;
using GetBatchDetailsHandler = FarmHelm.Application.Batches.GetBatchDetails.GetBatchDetailsHandler;
using ListBatchesHandler = FarmHelm.Application.Batches.ListBatches.ListBatchesHandler;
using RecordPlantMortalityHandler = FarmHelm.Application.Batches.RecordPlantMortality.RecordPlantMortalityHandler;
using RecordBatchMortalityHandler = FarmHelm.Application.Batches.RecordBatchMortality.RecordBatchMortalityHandler;
using BatchMortalityResult = FarmHelm.Application.Batches.RecordBatchMortality.BatchMortalityResult;
using RemoveBatchHandler = FarmHelm.Application.Batches.RemoveBatch.RemoveBatchHandler;

namespace FarmHelm.Api.Controllers;

[ApiController]
[Route("api/batches")]
public sealed class BatchesController(
    CreateBatchHandler createBatch,
    RecordBatchMortalityHandler recordBatchMortality,
    RecordPlantMortalityHandler recordPlantMortality,
    ChangeBatchStageHandler changeStage,
    RemoveBatchHandler removeBatch,
    GetBatchDetailsHandler getBatchDetails,
    ListBatchesHandler listBatches) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<BatchCreatedResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BatchCreatedResponse>> CreateAsync(CreateBatchRequest request, CancellationToken cancellationToken)
    {
        var result = await createBatch.HandleAsync(new FarmHelm.Application.Batches.CreateBatch.CreateBatchRequest(request.FarmId, request.VarietyId, request.LocationId, request.PlantingDate, request.InitialPlantCount, request.InitialStageId, request.IndividualTrackingEnabled, request.Notes), cancellationToken);
        var response = new BatchCreatedResponse(result.BatchId, result.BatchCode, result.InitialPlantCount, result.IndividualTrackingEnabled);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("{batchId:guid}/mortality")]
    [ProducesResponseType<BatchMortalityResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BatchMortalityResponse>> RecordBatchMortalityAsync(Guid batchId, RecordBatchMortalityRequest request, CancellationToken cancellationToken)
    {
        var result = await recordBatchMortality.HandleAsync(new FarmHelm.Application.Batches.RecordBatchMortality.RecordBatchMortalityRequest(batchId, request.MortalityDate, request.Quantity, request.MortalityReasonId, request.Notes), cancellationToken);
        return Ok(ToResponse(result));
    }

    [HttpPost("{batchId:guid}/plants/{plantId:guid}/mortality")]
    [ProducesResponseType<BatchMortalityResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BatchMortalityResponse>> RecordPlantMortalityAsync(Guid batchId, Guid plantId, RecordPlantMortalityRequest request, CancellationToken cancellationToken)
    {
        var result = await recordPlantMortality.HandleAsync(new FarmHelm.Application.Batches.RecordPlantMortality.RecordPlantMortalityRequest(batchId, plantId, request.MortalityDate, request.MortalityReasonId, request.Notes), cancellationToken);
        return Ok(ToResponse(result));
    }

    [HttpPost("{batchId:guid}/stage-changes")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangeStageAsync(Guid batchId, ChangeBatchStageRequest request, CancellationToken cancellationToken)
    {
        await changeStage.HandleAsync(new FarmHelm.Application.Batches.ChangeBatchStage.ChangeBatchStageRequest(batchId, request.StageId, request.EffectiveDate, request.Notes), cancellationToken);
        return NoContent();
    }

    [HttpPost("{batchId:guid}/removal")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveAsync(Guid batchId, RemoveBatchRequest request, CancellationToken cancellationToken)
    {
        await removeBatch.HandleAsync(new FarmHelm.Application.Batches.RemoveBatch.RemoveBatchRequest(batchId, request.RemovedDate), cancellationToken);
        return NoContent();
    }

    [HttpGet("{batchId:guid}")]
    [ProducesResponseType<BatchDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchDetailsResponse>> GetByIdAsync(Guid batchId, CancellationToken cancellationToken)
    {
        var result = await getBatchDetails.HandleAsync(batchId, cancellationToken);
        return Ok(ToResponse(result));
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<BatchSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BatchSummaryResponse>>> ListAsync(Guid? farmId, Guid? cropId, Guid? varietyId, string? status, CancellationToken cancellationToken)
    {
        BatchStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<BatchStatus>(status, ignoreCase: true, out var parsed) || !Enum.IsDefined(parsed))
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid batch status.", detail: "Status must be Active or Removed.");
            }

            parsedStatus = parsed;
        }

        var results = await listBatches.HandleAsync(new BatchListFilter(farmId, cropId, varietyId, parsedStatus), cancellationToken);
        return Ok(results.Select(ToResponse).ToArray());
    }

    private static BatchMortalityResponse ToResponse(BatchMortalityResult result) =>
        new(result.BatchId, result.DeadPlantCount, result.AlivePlantCount, result.SurvivalRate);

    private static BatchSummaryResponse ToResponse(BatchSummary result) =>
        new(result.BatchId, result.BatchCode, result.FarmId, result.CropId, result.VarietyId, result.VarietyName, result.PlantingDate, result.Status.ToString());

    private static BatchDetailsResponse ToResponse(BatchDetails result) =>
        new(
            result.BatchId, result.BatchCode, result.CropId, result.CropName, result.VarietyId, result.VarietyName,
            result.FarmId, result.FarmName, result.LocationId, result.LocationName, result.PlantingDate,
            result.InitialPlantCount, result.AlivePlantCount, result.DeadPlantCount, result.SurvivalRate,
            result.IndividualTrackingEnabled, result.CurrentStageId, result.CurrentStageName, result.Status.ToString(),
            result.RemovedDate, result.Notes,
            result.Plants.Select(plant => new PlantResponse(plant.PlantId, plant.PlantNumber, plant.PlantCode, plant.Status.ToString(), plant.Notes)).ToArray(),
            result.MortalityRecords.Select(record => new MortalityRecordResponse(record.MortalityRecordId, record.PlantId, record.MortalityDate, record.Quantity, record.ReasonId, record.Notes)).ToArray(),
            result.StageHistory.Select(history => new BatchStageHistoryResponse(history.HistoryId, history.StageId, history.StageName, history.EffectiveDate, history.Notes)).ToArray());
}
