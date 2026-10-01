using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Domain.Farms;
using Microsoft.EntityFrameworkCore;

namespace FarmHelm.Infrastructure.Persistence.Repositories;

public sealed class FarmLocationRepository(FarmHelmDbContext context) : IFarmLocationRepository
{
    public Task<FarmLocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.FarmLocations.SingleOrDefaultAsync(location => location.Id == id, cancellationToken);

    public async Task AddAsync(FarmLocation location, CancellationToken cancellationToken = default) =>
        await context.FarmLocations.AddAsync(location, cancellationToken);
}
