using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Domain.Farms;

namespace FarmHelm.Application.Farms.CreateFarm;

public sealed record CreateFarmRequest(string Name, string? DefaultCurrency = null, string? TimeZone = null, string? Notes = null);
public sealed record CreateFarmResult(Guid FarmId, string FarmCode, string Name);

public sealed class CreateFarmHandler(IFarmRepository farms, IBusinessCodeGenerator codes, IUnitOfWork unitOfWork)
{
    public async Task<CreateFarmResult> HandleAsync(CreateFarmRequest request, CancellationToken cancellationToken = default)
    {
        var code = await codes.GenerateAsync(BusinessCodeType.Farm, cancellationToken);
        var farm = new Farm(Guid.NewGuid(), code, request.Name, request.DefaultCurrency ?? "LKR", request.TimeZone ?? "Asia/Colombo", request.Notes);
        await farms.AddAsync(farm, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateFarmResult(farm.Id, farm.FarmCode, farm.Name);
    }
}
