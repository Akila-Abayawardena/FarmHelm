using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Domain.Farms;
using Microsoft.EntityFrameworkCore;

namespace FarmHelm.Infrastructure.Persistence.Repositories;

public sealed class FarmRepository(FarmHelmDbContext context) : IFarmRepository
{
    public Task<Farm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Farms.SingleOrDefaultAsync(farm => farm.Id == id, cancellationToken);

    public async Task AddAsync(Farm farm, CancellationToken cancellationToken = default) =>
        await context.Farms.AddAsync(farm, cancellationToken);
}
