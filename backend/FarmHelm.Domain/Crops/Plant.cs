using FarmHelm.Domain.Common;

namespace FarmHelm.Domain.Crops;

public sealed class Plant
{
    internal Plant(Guid id, Guid batchId, int plantNumber, string plantCode, string? notes = null)
    {
        Id = DomainGuard.Required(id, nameof(id));
        BatchId = DomainGuard.Required(batchId, nameof(batchId));
        PlantNumber = DomainGuard.Positive(plantNumber, nameof(plantNumber));
        PlantCode = DomainGuard.RequiredText(plantCode, nameof(plantCode));
        Notes = DomainGuard.OptionalText(notes);
        Status = PlantStatus.Active;
    }

    public Guid Id { get; }
    public Guid BatchId { get; }
    public int PlantNumber { get; }
    public string PlantCode { get; }
    public PlantStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public void UpdateNotes(string? notes) => Notes = DomainGuard.OptionalText(notes);

    internal void MarkDead()
    {
        if (Status != PlantStatus.Active)
        {
            throw new DomainException("Only an active plant can be recorded as dead.");
        }

        Status = PlantStatus.Dead;
    }
}
