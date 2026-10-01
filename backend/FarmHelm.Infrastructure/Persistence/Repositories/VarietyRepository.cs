using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Domain.Crops;
using Microsoft.EntityFrameworkCore;

namespace FarmHelm.Infrastructure.Persistence.Repositories;

public sealed class VarietyRepository(FarmHelmDbContext context) : IVarietyRepository
{
    public Task<Variety?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Varieties.SingleOrDefaultAsync(variety => variety.Id == id, cancellationToken);

    public async Task AddAsync(Variety variety, CancellationToken cancellationToken = default) =>
        await context.Varieties.AddAsync(variety, cancellationToken);
}
