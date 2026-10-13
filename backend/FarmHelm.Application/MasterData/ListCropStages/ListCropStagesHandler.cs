using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;

namespace FarmHelm.Application.MasterData.ListCropStages;

public sealed class ListCropStagesHandler(IAgriculturalMasterDataReadRepository masterData)
{
    public async Task<IReadOnlyList<CropStageReadModel>> HandleAsync(Guid cropId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        if (!await masterData.CropExistsAsync(cropId, cancellationToken)) throw new NotFoundException("Crop", cropId);
        return await masterData.ListCropStagesAsync(cropId, includeInactive, cancellationToken);
    }
}
