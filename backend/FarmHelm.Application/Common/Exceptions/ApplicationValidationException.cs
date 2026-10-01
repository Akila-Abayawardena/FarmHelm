namespace FarmHelm.Application.Common.Exceptions;

public sealed class ApplicationValidationException(string message) : InvalidOperationException(message)
{
}
