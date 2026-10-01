using System.Globalization;
using FarmHelm.Application.Abstractions.Codes;

namespace FarmHelm.Infrastructure.Persistence.Codes;

internal sealed record BusinessCodeDefinition(string SequenceName, string Prefix)
{
    internal string Format(long value) => $"{Prefix}-{value.ToString("D4", CultureInfo.InvariantCulture)}";
}

internal static class BusinessCodeMetadata
{
    internal static BusinessCodeDefinition Get(BusinessCodeType codeType) => codeType switch
    {
        BusinessCodeType.Farm => new("farm_code_seq", "FARM"),
        BusinessCodeType.FarmLocation => new("farm_location_code_seq", "LOC"),
        BusinessCodeType.Crop => new("crop_code_seq", "CROP"),
        BusinessCodeType.Variety => new("variety_code_seq", "VAR"),
        BusinessCodeType.Batch => new("batch_code_seq", "BAT"),
        _ => throw new ArgumentOutOfRangeException(nameof(codeType), codeType, "Unsupported business code type.")
    };
}
