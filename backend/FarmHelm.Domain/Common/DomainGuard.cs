namespace FarmHelm.Domain.Common;

internal static class DomainGuard
{
    public static Guid Required(Guid value, string name)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{name} is required.");
        }

        return value;
    }

    public static string RequiredText(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{name} is required.");
        }

        return value.Trim();
    }

    public static int Positive(int value, string name)
    {
        if (value <= 0)
        {
            throw new DomainException($"{name} must be greater than zero.");
        }

        return value;
    }

    public static int NonNegative(int value, string name)
    {
        if (value < 0)
        {
            throw new DomainException($"{name} cannot be negative.");
        }

        return value;
    }

    public static DateOnly RequiredDate(DateOnly value, string name)
    {
        if (value == DateOnly.MinValue)
        {
            throw new DomainException($"{name} is required.");
        }

        return value;
    }

    public static string? OptionalText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
