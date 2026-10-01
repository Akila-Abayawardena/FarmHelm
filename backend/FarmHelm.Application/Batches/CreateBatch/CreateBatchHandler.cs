using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Application.Batches.CreateBatch;

public sealed record CreateBatchRequest(Guid FarmId, Guid VarietyId, Guid? LocationId, DateOnly PlantingDate, int InitialPlantCount, Guid InitialStageId, bool IndividualTrackingEnabled, string? Notes = null);
public sealed record CreateBatchResult(Guid BatchId, string BatchCode, int InitialPlantCount, bool IndividualTrackingEnabled);

public sealed class CreateBatchHandler(
    IFarmRepository farms,
    IVarietyRepository varieties,
    ICropRepository crops,
    ICropStageRepository stages,
    IFarmLocationRepository locations,
    IBatchRepository batches,
    IBusinessCodeGenerator codes,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateBatchResult> HandleAsync(CreateBatchRequest request, CancellationToken cancellationToken = default)
    {
        _ = await farms.GetByIdAsync(request.FarmId, cancellationToken) ?? throw new NotFoundException("Farm", request.FarmId);
        var variety = await varieties.GetByIdAsync(request.VarietyId, cancellationToken) ?? throw new NotFoundException("Variety", request.VarietyId);
        if (!variety.IsActive) throw new ApplicationValidationException("An inactive Variety cannot be used for a Batch.");
        var crop = await crops.GetByIdAsync(variety.CropId, cancellationToken) ?? throw new NotFoundException("Crop", variety.CropId);
        if (!crop.IsActive) throw new ApplicationValidationException("An inactive Crop cannot be used for a Batch.");
        if (crop.FarmId != request.FarmId) throw new ApplicationValidationException("The Variety Crop must belong to the Batch Farm.");
        var stage = await stages.GetByIdAsync(request.InitialStageId, cancellationToken) ?? throw new NotFoundException("CropStage", request.InitialStageId);
        if (!stage.IsActive) throw new ApplicationValidationException("An inactive CropStage cannot be assigned to a Batch.");
        if (stage.CropId != crop.Id) throw new ApplicationValidationException("The CropStage must belong to the Batch Variety Crop.");

        if (request.LocationId.HasValue)
        {
            var location = await locations.GetByIdAsync(request.LocationId.Value, cancellationToken) ?? throw new NotFoundException("FarmLocation", request.LocationId.Value);
            if (!location.IsActive) throw new ApplicationValidationException("An inactive FarmLocation cannot be assigned to a Batch.");
            if (location.FarmId != request.FarmId) throw new ApplicationValidationException("The FarmLocation must belong to the Batch Farm.");
        }

        var code = await codes.GenerateAsync(BusinessCodeType.Batch, cancellationToken);
        var batch = Batch.Create(Guid.NewGuid(), code, request.FarmId, variety.Id, request.LocationId, request.PlantingDate, request.InitialPlantCount, stage.Id, request.IndividualTrackingEnabled, request.Notes);
        await batches.AddAsync(batch, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateBatchResult(batch.Id, batch.BatchCode, batch.InitialPlantCount, batch.IndividualTrackingEnabled);
    }
}
