using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Domain.Crops;
using Microsoft.EntityFrameworkCore;

namespace FarmHelm.Infrastructure.Persistence.Repositories;

public sealed class CropRepository(FarmHelmDbContext context) : ICropRepository
{
    public Task<Crop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Crops.SingleOrDefaultAsync(crop => crop.Id == id, cancellationToken);

    public async Task AddAsync(Crop crop, CancellationToken cancellationToken = default) =>
        await context.Crops.AddAsync(crop, cancellationToken);
}
