using FarmHelm.Domain.Common;

namespace FarmHelm.Domain.Crops;

public sealed class Variety
{
    public Variety(Guid id, Guid cropId, string varietyCode, string name, string? description = null)
    {
        Id = DomainGuard.Required(id, nameof(id));
        CropId = DomainGuard.Required(cropId, nameof(cropId));
        VarietyCode = DomainGuard.RequiredText(varietyCode, nameof(varietyCode));
        Name = DomainGuard.RequiredText(name, nameof(name));
        Description = DomainGuard.OptionalText(description);
        IsActive = true;
    }

    public Guid Id { get; }
    public Guid CropId { get; }
    public string VarietyCode { get; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public void UpdateDetails(string name, string? description)
    {
        Name = DomainGuard.RequiredText(name, nameof(name));
        Description = DomainGuard.OptionalText(description);
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
