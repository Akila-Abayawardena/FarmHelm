using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Domain.Crops;
using Microsoft.EntityFrameworkCore;

namespace FarmHelm.Infrastructure.Persistence.Repositories;

public sealed class BatchRepository(FarmHelmDbContext context) : IBatchRepository
{
    public Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Batches
            .Include(batch => batch.Plants)
            .Include(batch => batch.MortalityRecords)
            .Include(batch => batch.StageHistory)
            .AsSplitQuery()
            .SingleOrDefaultAsync(batch => batch.Id == id, cancellationToken);

    public async Task AddAsync(Batch batch, CancellationToken cancellationToken = default) =>
        await context.Batches.AddAsync(batch, cancellationToken);
}
