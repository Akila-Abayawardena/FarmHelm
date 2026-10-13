using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;

namespace FarmHelm.Application.MasterData.ListMortalityReasons;

public sealed class ListMortalityReasonsHandler(IAgriculturalMasterDataReadRepository masterData)
{
    public async Task<IReadOnlyList<MortalityReasonReadModel>> HandleAsync(Guid farmId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        if (!await masterData.FarmExistsAsync(farmId, cancellationToken)) throw new NotFoundException("Farm", farmId);
        return await masterData.ListMortalityReasonsAsync(farmId, includeInactive, cancellationToken);
    }
}
