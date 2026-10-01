using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Batches.RecordBatchMortality;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Application.Batches.RecordPlantMortality;

public sealed record RecordPlantMortalityRequest(Guid BatchId, Guid PlantId, DateOnly MortalityDate, Guid MortalityReasonId, string? Notes = null);

public sealed class RecordPlantMortalityHandler(IBatchRepository batches, IMortalityReasonRepository reasons, IUnitOfWork unitOfWork)
{
    public async Task<BatchMortalityResult> HandleAsync(RecordPlantMortalityRequest request, CancellationToken cancellationToken = default)
    {
        var batch = await batches.GetByIdAsync(request.BatchId, cancellationToken) ?? throw new NotFoundException("Batch", request.BatchId);
        if (batch.Status != BatchStatus.Active) throw new ApplicationValidationException("Mortality cannot be recorded for a removed Batch.");
        if (!batch.IndividualTrackingEnabled) throw new ApplicationValidationException("Individual plant mortality requires individual tracking.");
        var reason = await reasons.GetByIdAsync(request.MortalityReasonId, cancellationToken) ?? throw new NotFoundException("MortalityReason", request.MortalityReasonId);
        if (!reason.IsActive) throw new ApplicationValidationException("An inactive MortalityReason cannot be used.");
        if (reason.FarmId != batch.FarmId) throw new ApplicationValidationException("The MortalityReason must belong to the Batch Farm.");
        batch.RecordPlantMortality(Guid.NewGuid(), request.PlantId, request.MortalityDate, reason.Id, request.Notes);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new BatchMortalityResult(batch.Id, batch.DeadPlantCount, batch.AlivePlantCount, batch.SurvivalRate);
    }
}
