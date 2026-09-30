using FarmHelm.Domain.Common;

namespace FarmHelm.Domain.Crops;

public sealed class Batch
{
    private readonly List<Plant> _plants = [];
    private readonly List<MortalityRecord> _mortalityRecords = [];
    private readonly List<BatchStageHistory> _stageHistory = [];

    private Batch(
        Guid id,
        string batchCode,
        Guid farmId,
        Guid varietyId,
        Guid? locationId,
        DateOnly plantingDate,
        int initialPlantCount,
        Guid currentStageId,
        bool individualTrackingEnabled,
        string? notes)
    {
        Id = DomainGuard.Required(id, nameof(id));
        BatchCode = DomainGuard.RequiredText(batchCode, nameof(batchCode));
        FarmId = DomainGuard.Required(farmId, nameof(farmId));
        VarietyId = DomainGuard.Required(varietyId, nameof(varietyId));
        LocationId = locationId.HasValue
            ? DomainGuard.Required(locationId.Value, nameof(locationId))
            : null;
        PlantingDate = DomainGuard.RequiredDate(plantingDate, nameof(plantingDate));
        InitialPlantCount = DomainGuard.Positive(initialPlantCount, nameof(initialPlantCount));
        CurrentStageId = DomainGuard.Required(currentStageId, nameof(currentStageId));
        IndividualTrackingEnabled = individualTrackingEnabled;
        Notes = DomainGuard.OptionalText(notes);
        Status = BatchStatus.Active;

        _stageHistory.Add(new BatchStageHistory(Guid.NewGuid(), Id, CurrentStageId, PlantingDate, null));

        if (IndividualTrackingEnabled)
        {
            for (var plantNumber = 1; plantNumber <= InitialPlantCount; plantNumber++)
            {
                _plants.Add(new Plant(
                    Guid.NewGuid(),
                    Id,
                    plantNumber,
                    $"{BatchCode}-P{plantNumber:D3}"));
            }
        }
    }

    public Guid Id { get; }
    public string BatchCode { get; }
    public Guid FarmId { get; }
    public Guid VarietyId { get; }
    public Guid? LocationId { get; }
    public DateOnly PlantingDate { get; }
    public int InitialPlantCount { get; }
    public Guid CurrentStageId { get; private set; }
    public BatchStatus Status { get; private set; }
    public bool IndividualTrackingEnabled { get; }
    public DateOnly? RemovedDate { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyList<Plant> Plants => _plants.AsReadOnly();
    public IReadOnlyList<MortalityRecord> MortalityRecords => _mortalityRecords.AsReadOnly();
    public IReadOnlyList<BatchStageHistory> StageHistory => _stageHistory.AsReadOnly();
    public int DeadPlantCount => _mortalityRecords.Sum(record => record.Quantity);
    public int AlivePlantCount => InitialPlantCount - DeadPlantCount;
    public decimal SurvivalRate => decimal.Round((decimal)AlivePlantCount / InitialPlantCount * 100m, 2);

    public static Batch Create(
        Guid id,
        string batchCode,
        Guid farmId,
        Guid varietyId,
        Guid? locationId,
        DateOnly plantingDate,
        int initialPlantCount,
        Guid currentStageId,
        bool individualTrackingEnabled,
        string? notes = null) =>
        new(
            id,
            batchCode,
            farmId,
            varietyId,
            locationId,
            plantingDate,
            initialPlantCount,
            currentStageId,
            individualTrackingEnabled,
            notes);

    public void UpdateNotes(string? notes) => Notes = DomainGuard.OptionalText(notes);

    public MortalityRecord RecordMortality(
        Guid mortalityRecordId,
        DateOnly mortalityDate,
        int quantity,
        Guid reasonId,
        string? notes = null)
    {
        if (IndividualTrackingEnabled)
        {
            throw new DomainException("Quantity-based mortality is not allowed when individual tracking is enabled.");
        }

        ValidateMortality(mortalityRecordId, mortalityDate, quantity, reasonId);

        if (DeadPlantCount + quantity > InitialPlantCount)
        {
            throw new DomainException("Mortality quantity cannot exceed the living plant count.");
        }

        var mortalityRecord = new MortalityRecord(
            mortalityRecordId,
            Id,
            null,
            mortalityDate,
            quantity,
            reasonId,
            notes);
        _mortalityRecords.Add(mortalityRecord);

        return mortalityRecord;
    }

    public MortalityRecord RecordPlantMortality(
        Guid mortalityRecordId,
        Guid plantId,
        DateOnly mortalityDate,
        Guid reasonId,
        string? notes = null)
    {
        if (!IndividualTrackingEnabled)
        {
            throw new DomainException("Individual plant mortality is not allowed when individual tracking is disabled.");
        }

        DomainGuard.Required(plantId, nameof(plantId));
        ValidateMortality(mortalityRecordId, mortalityDate, 1, reasonId);

        var plant = _plants.SingleOrDefault(candidate => candidate.Id == plantId)
            ?? throw new DomainException("Plant does not belong to this batch.");

        plant.MarkDead();

        var mortalityRecord = new MortalityRecord(
            mortalityRecordId,
            Id,
            plantId,
            mortalityDate,
            1,
            reasonId,
            notes);
        _mortalityRecords.Add(mortalityRecord);

        return mortalityRecord;
    }

    public void ChangeStage(Guid stageId, DateOnly effectiveDate, string? notes = null)
    {
        DomainGuard.Required(stageId, nameof(stageId));
        DomainGuard.RequiredDate(effectiveDate, nameof(effectiveDate));

        if (stageId == CurrentStageId)
        {
            throw new DomainException("The requested stage is already current.");
        }

        // The Application layer will verify that the selected stage belongs to the Batch variety's Crop.
        CurrentStageId = stageId;
        _stageHistory.Add(new BatchStageHistory(Guid.NewGuid(), Id, stageId, effectiveDate, notes));
    }

    public void Remove(DateOnly removedDate)
    {
        if (Status == BatchStatus.Removed)
        {
            throw new DomainException("The batch has already been removed.");
        }

        RemovedDate = DomainGuard.RequiredDate(removedDate, nameof(removedDate));
        Status = BatchStatus.Removed;
    }

    private static void ValidateMortality(Guid mortalityRecordId, DateOnly mortalityDate, int quantity, Guid reasonId)
    {
        DomainGuard.Required(mortalityRecordId, nameof(mortalityRecordId));
        DomainGuard.RequiredDate(mortalityDate, nameof(mortalityDate));
        DomainGuard.Positive(quantity, nameof(quantity));
        DomainGuard.Required(reasonId, nameof(reasonId));
    }
}
