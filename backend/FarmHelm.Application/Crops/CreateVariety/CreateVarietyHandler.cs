using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Application.Crops.CreateVariety;

public sealed record CreateVarietyRequest(Guid CropId, string Name, string? Description = null);
public sealed record CreateVarietyResult(Guid VarietyId, string VarietyCode, Guid CropId, string Name);

public sealed class CreateVarietyHandler(ICropRepository crops, IVarietyRepository varieties, IBusinessCodeGenerator codes, IUnitOfWork unitOfWork)
{
    public async Task<CreateVarietyResult> HandleAsync(CreateVarietyRequest request, CancellationToken cancellationToken = default)
    {
        var crop = await crops.GetByIdAsync(request.CropId, cancellationToken) ?? throw new NotFoundException("Crop", request.CropId);
        if (!crop.IsActive) throw new ApplicationValidationException("An inactive Crop cannot be used to create a Variety.");
        var code = await codes.GenerateAsync(BusinessCodeType.Variety, cancellationToken);
        var variety = new Variety(Guid.NewGuid(), crop.Id, code, request.Name, request.Description);
        await varieties.AddAsync(variety, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateVarietyResult(variety.Id, variety.VarietyCode, variety.CropId, variety.Name);
    }
}
