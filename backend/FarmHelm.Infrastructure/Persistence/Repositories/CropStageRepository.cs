using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Domain.Crops;
using Microsoft.EntityFrameworkCore;

namespace FarmHelm.Infrastructure.Persistence.Repositories;

public sealed class CropStageRepository(FarmHelmDbContext context) : ICropStageRepository
{
    public Task<CropStage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.CropStages.SingleOrDefaultAsync(stage => stage.Id == id, cancellationToken);

    public async Task AddAsync(CropStage cropStage, CancellationToken cancellationToken = default) =>
        await context.CropStages.AddAsync(cropStage, cancellationToken);
}
