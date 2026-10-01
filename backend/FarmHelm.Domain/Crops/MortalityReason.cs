using FarmHelm.Domain.Common;

namespace FarmHelm.Domain.Crops;

public sealed class MortalityReason
{
    public MortalityReason(Guid id, Guid farmId, string name, int displayOrder)
    {
        Id = DomainGuard.Required(id, nameof(id));
        FarmId = DomainGuard.Required(farmId, nameof(farmId));
        Name = DomainGuard.RequiredText(name, nameof(name));
        DisplayOrder = DomainGuard.NonNegative(displayOrder, nameof(displayOrder));
        IsActive = true;
    }

    public Guid Id { get; }
    public Guid FarmId { get; }
    public string Name { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    public void UpdateDisplayOrder(int displayOrder) =>
        DisplayOrder = DomainGuard.NonNegative(displayOrder, nameof(displayOrder));

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
