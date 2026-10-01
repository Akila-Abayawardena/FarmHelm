using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Application.Crops.CreateCrop;

public sealed record CreateCropRequest(Guid FarmId, string Name, string? ScientificName = null, string? Description = null);
public sealed record CreateCropResult(Guid CropId, string CropCode, Guid FarmId, string Name);

public sealed class CreateCropHandler(IFarmRepository farms, ICropRepository crops, IBusinessCodeGenerator codes, IUnitOfWork unitOfWork)
{
    public async Task<CreateCropResult> HandleAsync(CreateCropRequest request, CancellationToken cancellationToken = default)
    {
        _ = await farms.GetByIdAsync(request.FarmId, cancellationToken) ?? throw new NotFoundException("Farm", request.FarmId);
        var code = await codes.GenerateAsync(BusinessCodeType.Crop, cancellationToken);
        var crop = new Crop(Guid.NewGuid(), request.FarmId, code, request.Name, request.ScientificName, request.Description);
        await crops.AddAsync(crop, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateCropResult(crop.Id, crop.CropCode, crop.FarmId, crop.Name);
    }
}
