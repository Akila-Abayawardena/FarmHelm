using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Application.Batches.RecordBatchMortality;

public sealed record RecordBatchMortalityRequest(Guid BatchId, DateOnly MortalityDate, int Quantity, Guid MortalityReasonId, string? Notes = null);
public sealed record BatchMortalityResult(Guid BatchId, int DeadPlantCount, int AlivePlantCount, decimal SurvivalRate);

public sealed class RecordBatchMortalityHandler(IBatchRepository batches, IMortalityReasonRepository reasons, IUnitOfWork unitOfWork)
{
    public async Task<BatchMortalityResult> HandleAsync(RecordBatchMortalityRequest request, CancellationToken cancellationToken = default)
    {
        var batch = await batches.GetByIdAsync(request.BatchId, cancellationToken) ?? throw new NotFoundException("Batch", request.BatchId);
        if (batch.Status != BatchStatus.Active) throw new ApplicationValidationException("Mortality cannot be recorded for a removed Batch.");
        if (batch.IndividualTrackingEnabled) throw new ApplicationValidationException("Batch-level mortality is not allowed when individual tracking is enabled.");
        var reason = await reasons.GetByIdAsync(request.MortalityReasonId, cancellationToken) ?? throw new NotFoundException("MortalityReason", request.MortalityReasonId);
        if (!reason.IsActive) throw new ApplicationValidationException("An inactive MortalityReason cannot be used.");
        if (reason.FarmId != batch.FarmId) throw new ApplicationValidationException("The MortalityReason must belong to the Batch Farm.");
        batch.RecordMortality(Guid.NewGuid(), request.MortalityDate, request.Quantity, reason.Id, request.Notes);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new BatchMortalityResult(batch.Id, batch.DeadPlantCount, batch.AlivePlantCount, batch.SurvivalRate);
    }
}
