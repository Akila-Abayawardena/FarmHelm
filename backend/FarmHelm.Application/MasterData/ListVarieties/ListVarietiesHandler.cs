using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;

namespace FarmHelm.Application.MasterData.ListVarieties;

public sealed class ListVarietiesHandler(IAgriculturalMasterDataReadRepository masterData)
{
    public async Task<IReadOnlyList<VarietyReadModel>> HandleAsync(Guid cropId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        if (!await masterData.CropExistsAsync(cropId, cancellationToken)) throw new NotFoundException("Crop", cropId);
        return await masterData.ListVarietiesAsync(cropId, includeInactive, cancellationToken);
    }
}
