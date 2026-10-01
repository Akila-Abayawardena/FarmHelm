using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Domain.Crops;
using Microsoft.EntityFrameworkCore;

namespace FarmHelm.Infrastructure.Persistence.Repositories;

public sealed class MortalityReasonRepository(FarmHelmDbContext context) : IMortalityReasonRepository
{
    public Task<MortalityReason?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.MortalityReasons.SingleOrDefaultAsync(reason => reason.Id == id, cancellationToken);

    public async Task AddAsync(MortalityReason mortalityReason, CancellationToken cancellationToken = default) =>
        await context.MortalityReasons.AddAsync(mortalityReason, cancellationToken);
}
