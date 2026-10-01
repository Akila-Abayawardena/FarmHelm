using FarmHelm.Domain.Common;

namespace FarmHelm.Domain.Crops;

public sealed class Crop
{
    public Crop(
        Guid id,
        Guid farmId,
        string cropCode,
        string name,
        string? scientificName = null,
        string? description = null)
    {
        Id = DomainGuard.Required(id, nameof(id));
        FarmId = DomainGuard.Required(farmId, nameof(farmId));
        CropCode = DomainGuard.RequiredText(cropCode, nameof(cropCode));
        Name = DomainGuard.RequiredText(name, nameof(name));
        ScientificName = DomainGuard.OptionalText(scientificName);
        Description = DomainGuard.OptionalText(description);
        IsActive = true;
    }

    public Guid Id { get; }
    public Guid FarmId { get; }
    public string CropCode { get; }
    public string Name { get; private set; }
    public string? ScientificName { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public void UpdateDetails(string name, string? scientificName, string? description)
    {
        Name = DomainGuard.RequiredText(name, nameof(name));
        ScientificName = DomainGuard.OptionalText(scientificName);
        Description = DomainGuard.OptionalText(description);
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
