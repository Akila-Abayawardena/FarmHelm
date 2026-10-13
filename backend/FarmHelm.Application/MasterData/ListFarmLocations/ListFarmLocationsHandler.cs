using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;

namespace FarmHelm.Application.MasterData.ListFarmLocations;

public sealed class ListFarmLocationsHandler(IAgriculturalMasterDataReadRepository masterData)
{
    public async Task<IReadOnlyList<FarmLocationReadModel>> HandleAsync(Guid farmId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        if (!await masterData.FarmExistsAsync(farmId, cancellationToken)) throw new NotFoundException("Farm", farmId);
        return await masterData.ListFarmLocationsAsync(farmId, includeInactive, cancellationToken);
    }
}
