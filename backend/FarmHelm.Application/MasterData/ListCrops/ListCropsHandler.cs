using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;

namespace FarmHelm.Application.MasterData.ListCrops;

public sealed class ListCropsHandler(IAgriculturalMasterDataReadRepository masterData)
{
    public async Task<IReadOnlyList<CropReadModel>> HandleAsync(Guid farmId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        if (!await masterData.FarmExistsAsync(farmId, cancellationToken)) throw new NotFoundException("Farm", farmId);
        return await masterData.ListCropsAsync(farmId, includeInactive, cancellationToken);
    }
}
