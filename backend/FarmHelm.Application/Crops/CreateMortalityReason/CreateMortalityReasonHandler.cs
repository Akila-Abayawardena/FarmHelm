using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Application.Crops.CreateMortalityReason;

public sealed record CreateMortalityReasonRequest(Guid FarmId, string Name, int DisplayOrder);
public sealed record CreateMortalityReasonResult(Guid MortalityReasonId, Guid FarmId, string Name, int DisplayOrder);

public sealed class CreateMortalityReasonHandler(IFarmRepository farms, IMortalityReasonRepository reasons, IUnitOfWork unitOfWork)
{
    public async Task<CreateMortalityReasonResult> HandleAsync(CreateMortalityReasonRequest request, CancellationToken cancellationToken = default)
    {
        _ = await farms.GetByIdAsync(request.FarmId, cancellationToken) ?? throw new NotFoundException("Farm", request.FarmId);
        var reason = new MortalityReason(Guid.NewGuid(), request.FarmId, request.Name, request.DisplayOrder);
        await reasons.AddAsync(reason, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateMortalityReasonResult(reason.Id, reason.FarmId, reason.Name, reason.DisplayOrder);
    }
}
