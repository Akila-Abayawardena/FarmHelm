using FarmHelm.Domain.Common;

namespace FarmHelm.Domain.Farms;

public sealed class Farm
{
    public Farm(
        Guid id,
        string farmCode,
        string name,
        string defaultCurrency,
        string timeZone,
        string? notes = null)
    {
        Id = DomainGuard.Required(id, nameof(id));
        FarmCode = DomainGuard.RequiredText(farmCode, nameof(farmCode));
        Name = DomainGuard.RequiredText(name, nameof(name));
        DefaultCurrency = DomainGuard.RequiredText(defaultCurrency, nameof(defaultCurrency));
        TimeZone = DomainGuard.RequiredText(timeZone, nameof(timeZone));
        Notes = DomainGuard.OptionalText(notes);
    }

    public Guid Id { get; }
    public string FarmCode { get; }
    public string Name { get; private set; }
    public string DefaultCurrency { get; private set; }
    public string TimeZone { get; private set; }
    public string? Notes { get; private set; }

    public void UpdateDetails(string name, string defaultCurrency, string timeZone, string? notes)
    {
        Name = DomainGuard.RequiredText(name, nameof(name));
        DefaultCurrency = DomainGuard.RequiredText(defaultCurrency, nameof(defaultCurrency));
        TimeZone = DomainGuard.RequiredText(timeZone, nameof(timeZone));
        Notes = DomainGuard.OptionalText(notes);
    }
}
