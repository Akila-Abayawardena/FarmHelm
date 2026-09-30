using FarmHelm.Domain.Common;

namespace FarmHelm.Domain.Farms;

public sealed class FarmLocation
{
    public FarmLocation(
        Guid id,
        Guid farmId,
        string locationCode,
        string name,
        string? description = null)
    {
        Id = DomainGuard.Required(id, nameof(id));
        FarmId = DomainGuard.Required(farmId, nameof(farmId));
        LocationCode = DomainGuard.RequiredText(locationCode, nameof(locationCode));
        Name = DomainGuard.RequiredText(name, nameof(name));
        Description = DomainGuard.OptionalText(description);
        IsActive = true;
    }

    public Guid Id { get; }
    public Guid FarmId { get; }
    public string LocationCode { get; }
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
