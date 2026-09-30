using FarmHelm.Domain.Common;

namespace FarmHelm.Domain.Crops;

public sealed class BatchStageHistory
{
    internal BatchStageHistory(Guid id, Guid batchId, Guid stageId, DateOnly effectiveDate, string? notes)
    {
        Id = DomainGuard.Required(id, nameof(id));
        BatchId = DomainGuard.Required(batchId, nameof(batchId));
        StageId = DomainGuard.Required(stageId, nameof(stageId));
        EffectiveDate = DomainGuard.RequiredDate(effectiveDate, nameof(effectiveDate));
        Notes = DomainGuard.OptionalText(notes);
    }

    public Guid Id { get; }
    public Guid BatchId { get; }
    public Guid StageId { get; }
    public DateOnly EffectiveDate { get; }
    public string? Notes { get; }
}
