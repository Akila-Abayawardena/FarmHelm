using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Common.Exceptions;

namespace FarmHelm.Application.Batches.RemoveBatch;

public sealed record RemoveBatchRequest(Guid BatchId, DateOnly RemovedDate);

public sealed class RemoveBatchHandler(IBatchRepository batches, IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(RemoveBatchRequest request, CancellationToken cancellationToken = default)
    {
        var batch = await batches.GetByIdAsync(request.BatchId, cancellationToken) ?? throw new NotFoundException("Batch", request.BatchId);
        batch.Remove(request.RemovedDate);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
