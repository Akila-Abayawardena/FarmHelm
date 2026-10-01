using FarmHelm.Application.Batches;
using FarmHelm.Application.Batches.GetBatchDetails;
using FarmHelm.Application.Batches.ListBatches;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Tests.Application;

public sealed class BatchQueryUseCaseTests
{
    [Fact]
    public async Task GetBatchDetailsThrowsWhenTheBatchIsNotFound()
    {
        var handler = new GetBatchDetailsHandler(new BatchReadRepositoryFake());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetBatchDetailsReturnsTheReadModelUnchanged()
    {
        var readRepository = new BatchReadRepositoryFake();
        var details = CreateDetails();
        readRepository.Details = details;

        var result = await new GetBatchDetailsHandler(readRepository).HandleAsync(details.BatchId);

        Assert.Same(details, result);
    }

    [Fact]
    public async Task ListBatchesForwardsTheRequestedFilter()
    {
        var readRepository = new BatchReadRepositoryFake();
        var filter = new BatchListFilter(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), BatchStatus.Active);
        var expected = new[] { new BatchSummary(Guid.NewGuid(), "BAT-0001", filter.FarmId!.Value, filter.CropId!.Value, filter.VarietyId!.Value, "Scotch Bonnet", new DateOnly(2026, 1, 1), BatchStatus.Active) };
        readRepository.Summaries = expected;

        var result = await new ListBatchesHandler(readRepository).HandleAsync(filter);

        Assert.Same(expected, result);
        Assert.Equal(filter, readRepository.LastFilter);
    }

    private static BatchDetails CreateDetails() => new(
        Guid.NewGuid(), "BAT-0001", Guid.NewGuid(), "Pepper", Guid.NewGuid(), "Scotch Bonnet", Guid.NewGuid(), "Main Farm", null, null,
        new DateOnly(2026, 1, 1), 10, 10, 0, 100m, false, Guid.NewGuid(), "Vegetative", BatchStatus.Active, null, null,
        Array.Empty<PlantDetails>(), Array.Empty<MortalityRecordDetails>(), Array.Empty<BatchStageHistoryDetails>());
}
