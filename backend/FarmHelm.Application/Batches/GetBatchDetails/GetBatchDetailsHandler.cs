using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Batches;
using FarmHelm.Application.Common.Exceptions;

namespace FarmHelm.Application.Batches.GetBatchDetails;

public sealed class GetBatchDetailsHandler(IBatchReadRepository batches)
{
    public async Task<BatchDetails> HandleAsync(Guid batchId, CancellationToken cancellationToken = default) =>
        await batches.GetDetailsByIdAsync(batchId, cancellationToken) ?? throw new NotFoundException("Batch", batchId);
}
