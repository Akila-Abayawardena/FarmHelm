using FarmHelm.Application.Abstractions.Persistence;

namespace FarmHelm.Infrastructure.Persistence;

public sealed class UnitOfWork(FarmHelmDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
