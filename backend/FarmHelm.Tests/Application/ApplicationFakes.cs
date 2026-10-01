using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Batches;
using FarmHelm.Domain.Crops;
using FarmHelm.Domain.Farms;

namespace FarmHelm.Tests.Application;

internal sealed class FarmRepositoryFake : IFarmRepository
{
    internal Dictionary<Guid, Farm> Items { get; } = [];
    internal List<Farm> Added { get; } = [];
    public Task<Farm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.GetValueOrDefault(id));
    public Task AddAsync(Farm farm, CancellationToken cancellationToken = default) { Added.Add(farm); Items[farm.Id] = farm; return Task.CompletedTask; }
}
internal sealed class FarmLocationRepositoryFake : IFarmLocationRepository
{
    internal Dictionary<Guid, FarmLocation> Items { get; } = [];
    internal List<FarmLocation> Added { get; } = [];
    public Task<FarmLocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.GetValueOrDefault(id));
    public Task AddAsync(FarmLocation item, CancellationToken cancellationToken = default) { Added.Add(item); Items[item.Id] = item; return Task.CompletedTask; }
}
internal sealed class CropRepositoryFake : ICropRepository
{
    internal Dictionary<Guid, Crop> Items { get; } = [];
    internal List<Crop> Added { get; } = [];
    public Task<Crop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.GetValueOrDefault(id));
    public Task AddAsync(Crop item, CancellationToken cancellationToken = default) { Added.Add(item); Items[item.Id] = item; return Task.CompletedTask; }
}
internal sealed class VarietyRepositoryFake : IVarietyRepository
{
    internal Dictionary<Guid, Variety> Items { get; } = [];
    internal List<Variety> Added { get; } = [];
    public Task<Variety?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.GetValueOrDefault(id));
    public Task AddAsync(Variety item, CancellationToken cancellationToken = default) { Added.Add(item); Items[item.Id] = item; return Task.CompletedTask; }
}
internal sealed class CropStageRepositoryFake : ICropStageRepository
{
    internal Dictionary<Guid, CropStage> Items { get; } = [];
    internal List<CropStage> Added { get; } = [];
    public Task<CropStage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.GetValueOrDefault(id));
    public Task AddAsync(CropStage item, CancellationToken cancellationToken = default) { Added.Add(item); Items[item.Id] = item; return Task.CompletedTask; }
}
internal sealed class MortalityReasonRepositoryFake : IMortalityReasonRepository
{
    internal Dictionary<Guid, MortalityReason> Items { get; } = [];
    internal List<MortalityReason> Added { get; } = [];
    public Task<MortalityReason?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.GetValueOrDefault(id));
    public Task AddAsync(MortalityReason item, CancellationToken cancellationToken = default) { Added.Add(item); Items[item.Id] = item; return Task.CompletedTask; }
}
internal sealed class BatchRepositoryFake : IBatchRepository
{
    internal Dictionary<Guid, Batch> Items { get; } = [];
    internal List<Batch> Added { get; } = [];
    public Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.GetValueOrDefault(id));
    public Task AddAsync(Batch item, CancellationToken cancellationToken = default) { Added.Add(item); Items[item.Id] = item; return Task.CompletedTask; }
}
internal sealed class UnitOfWorkFake : IUnitOfWork
{
    internal int SaveCount { get; private set; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) { SaveCount++; return Task.FromResult(1); }
}
internal sealed class BusinessCodeGeneratorFake : IBusinessCodeGenerator
{
    internal List<BusinessCodeType> Requested { get; } = [];
    public Task<string> GenerateAsync(BusinessCodeType codeType, CancellationToken cancellationToken = default)
    {
        Requested.Add(codeType);
        return Task.FromResult(codeType switch { BusinessCodeType.Farm => "FARM-0001", BusinessCodeType.FarmLocation => "LOC-0001", BusinessCodeType.Crop => "CROP-0001", BusinessCodeType.Variety => "VAR-0001", BusinessCodeType.Batch => "BAT-0001", _ => throw new ArgumentOutOfRangeException(nameof(codeType)) });
    }
}
internal sealed class BatchReadRepositoryFake : IBatchReadRepository
{
    internal BatchDetails? Details { get; set; }
    internal IReadOnlyList<BatchSummary> Summaries { get; set; } = [];
    internal BatchListFilter? LastFilter { get; private set; }
    public Task<BatchDetails?> GetDetailsByIdAsync(Guid batchId, CancellationToken cancellationToken = default) => Task.FromResult(Details);
    public Task<IReadOnlyList<BatchSummary>> ListAsync(BatchListFilter filter, CancellationToken cancellationToken = default) { LastFilter = filter; return Task.FromResult(Summaries); }
}
internal static class ApplicationFixture
{
    internal static Farm Farm(Guid? id = null) => new(id ?? Guid.NewGuid(), "FARM-0001", "Farm", "LKR", "Asia/Colombo");
    internal static Crop Crop(Guid farmId, Guid? id = null) => new(id ?? Guid.NewGuid(), farmId, "CROP-0001", "Pepper");
    internal static Variety Variety(Guid cropId, Guid? id = null) => new(id ?? Guid.NewGuid(), cropId, "VAR-0001", "Scotch Bonnet");
    internal static CropStage Stage(Guid cropId, Guid? id = null) => new(id ?? Guid.NewGuid(), cropId, "Vegetative", 1);
    internal static FarmLocation Location(Guid farmId, Guid? id = null) => new(id ?? Guid.NewGuid(), farmId, "LOC-0001", "Field A");
    internal static MortalityReason Reason(Guid farmId, Guid? id = null) => new(id ?? Guid.NewGuid(), farmId, "Disease", 1);
    internal static Batch Batch(Guid farmId, Guid varietyId, Guid stageId, bool tracked = false, Guid? id = null) => FarmHelm.Domain.Crops.Batch.Create(id ?? Guid.NewGuid(), "BAT-0001", farmId, varietyId, null, new DateOnly(2026, 1, 1), 3, stageId, tracked);
}
