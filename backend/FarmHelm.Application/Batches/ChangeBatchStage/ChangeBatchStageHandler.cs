using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Application.Batches.ChangeBatchStage;

public sealed record ChangeBatchStageRequest(Guid BatchId, Guid StageId, DateOnly EffectiveDate, string? Notes = null);

public sealed class ChangeBatchStageHandler(IBatchRepository batches, IVarietyRepository varieties, ICropRepository crops, ICropStageRepository stages, IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(ChangeBatchStageRequest request, CancellationToken cancellationToken = default)
    {
        var batch = await batches.GetByIdAsync(request.BatchId, cancellationToken) ?? throw new NotFoundException("Batch", request.BatchId);
        if (batch.Status != BatchStatus.Active) throw new ApplicationValidationException("The stage of a removed Batch cannot be changed.");
        var variety = await varieties.GetByIdAsync(batch.VarietyId, cancellationToken) ?? throw new NotFoundException("Variety", batch.VarietyId);
        var crop = await crops.GetByIdAsync(variety.CropId, cancellationToken) ?? throw new NotFoundException("Crop", variety.CropId);
        var stage = await stages.GetByIdAsync(request.StageId, cancellationToken) ?? throw new NotFoundException("CropStage", request.StageId);
        if (!stage.IsActive) throw new ApplicationValidationException("An inactive CropStage cannot be assigned to a Batch.");
        if (stage.CropId != crop.Id) throw new ApplicationValidationException("The CropStage must belong to the Batch Variety Crop.");
        batch.ChangeStage(stage.Id, request.EffectiveDate, request.Notes);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
