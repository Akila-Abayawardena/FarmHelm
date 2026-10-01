namespace FarmHelm.Application.Common.Exceptions;

public sealed class ConflictException(string message) : InvalidOperationException(message)
{
}
