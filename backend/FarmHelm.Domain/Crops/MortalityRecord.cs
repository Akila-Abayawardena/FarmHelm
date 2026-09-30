using FarmHelm.Domain.Common;

namespace FarmHelm.Domain.Crops;

public sealed class MortalityRecord
{
    internal MortalityRecord(
        Guid id,
        Guid batchId,
        Guid? plantId,
        DateOnly mortalityDate,
        int quantity,
        Guid reasonId,
        string? notes)
    {
        Id = DomainGuard.Required(id, nameof(id));
        BatchId = DomainGuard.Required(batchId, nameof(batchId));
        PlantId = plantId.HasValue
            ? DomainGuard.Required(plantId.Value, nameof(plantId))
            : null;
        MortalityDate = DomainGuard.RequiredDate(mortalityDate, nameof(mortalityDate));
        Quantity = DomainGuard.Positive(quantity, nameof(quantity));
        ReasonId = DomainGuard.Required(reasonId, nameof(reasonId));
        Notes = DomainGuard.OptionalText(notes);
    }

    public Guid Id { get; }
    public Guid BatchId { get; }
    public Guid? PlantId { get; }
    public DateOnly MortalityDate { get; }
    public int Quantity { get; }
    public Guid ReasonId { get; }
    public string? Notes { get; }
}
