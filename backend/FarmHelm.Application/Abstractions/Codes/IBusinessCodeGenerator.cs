namespace FarmHelm.Application.Abstractions.Codes;

public interface IBusinessCodeGenerator
{
    Task<string> GenerateAsync(BusinessCodeType codeType, CancellationToken cancellationToken = default);
}
