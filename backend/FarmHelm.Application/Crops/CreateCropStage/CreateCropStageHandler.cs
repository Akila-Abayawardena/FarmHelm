using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Application.Crops.CreateCropStage;

public sealed record CreateCropStageRequest(Guid CropId, string Name, int DisplayOrder);
public sealed record CreateCropStageResult(Guid CropStageId, Guid CropId, string Name, int DisplayOrder);

public sealed class CreateCropStageHandler(ICropRepository crops, ICropStageRepository stages, IUnitOfWork unitOfWork)
{
    public async Task<CreateCropStageResult> HandleAsync(CreateCropStageRequest request, CancellationToken cancellationToken = default)
    {
        var crop = await crops.GetByIdAsync(request.CropId, cancellationToken) ?? throw new NotFoundException("Crop", request.CropId);
        if (!crop.IsActive) throw new ApplicationValidationException("An inactive Crop cannot be used to create a CropStage.");
        var stage = new CropStage(Guid.NewGuid(), crop.Id, request.Name, request.DisplayOrder);
        await stages.AddAsync(stage, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateCropStageResult(stage.Id, stage.CropId, stage.Name, stage.DisplayOrder);
    }
}
