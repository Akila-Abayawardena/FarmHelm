namespace FarmHelm.Application.Common.Exceptions;

public sealed class NotFoundException(string resourceName, Guid id)
    : InvalidOperationException($"{resourceName} '{id}' was not found.")
{
}
