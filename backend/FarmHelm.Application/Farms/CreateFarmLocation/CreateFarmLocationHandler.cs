using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Farms;

namespace FarmHelm.Application.Farms.CreateFarmLocation;

public sealed record CreateFarmLocationRequest(Guid FarmId, string Name, string? Description = null);
public sealed record CreateFarmLocationResult(Guid LocationId, string LocationCode, Guid FarmId, string Name);

public sealed class CreateFarmLocationHandler(IFarmRepository farms, IFarmLocationRepository locations, IBusinessCodeGenerator codes, IUnitOfWork unitOfWork)
{
    public async Task<CreateFarmLocationResult> HandleAsync(CreateFarmLocationRequest request, CancellationToken cancellationToken = default)
    {
        _ = await farms.GetByIdAsync(request.FarmId, cancellationToken) ?? throw new NotFoundException("Farm", request.FarmId);
        var code = await codes.GenerateAsync(BusinessCodeType.FarmLocation, cancellationToken);
        var location = new FarmLocation(Guid.NewGuid(), request.FarmId, code, request.Name, request.Description);
        await locations.AddAsync(location, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateFarmLocationResult(location.Id, location.LocationCode, location.FarmId, location.Name);
    }
}
