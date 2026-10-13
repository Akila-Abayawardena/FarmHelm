using FarmHelm.Application.Abstractions.Persistence;

namespace FarmHelm.Application.MasterData.ListFarms;

public sealed class ListFarmsHandler(IAgriculturalMasterDataReadRepository masterData)
{
    public Task<IReadOnlyList<FarmReadModel>> HandleAsync(CancellationToken cancellationToken = default) =>
        masterData.ListFarmsAsync(cancellationToken);
}
