using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Batches;

namespace FarmHelm.Application.Batches.ListBatches;

public sealed class ListBatchesHandler(IBatchReadRepository batches)
{
    public Task<IReadOnlyList<BatchSummary>> HandleAsync(BatchListFilter filter, CancellationToken cancellationToken = default) =>
        batches.ListAsync(filter, cancellationToken);
}
