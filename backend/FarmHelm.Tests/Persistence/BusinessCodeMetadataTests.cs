using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Infrastructure.Persistence.Codes;

namespace FarmHelm.Tests.Persistence;

public sealed class BusinessCodeMetadataTests
{
    [Theory]
    [InlineData(BusinessCodeType.Farm, "farm_code_seq", "FARM")]
    [InlineData(BusinessCodeType.FarmLocation, "farm_location_code_seq", "LOC")]
    [InlineData(BusinessCodeType.Crop, "crop_code_seq", "CROP")]
    [InlineData(BusinessCodeType.Variety, "variety_code_seq", "VAR")]
    [InlineData(BusinessCodeType.Batch, "batch_code_seq", "BAT")]
    public void KnownCodeTypesHaveStableSequenceAndPrefix(BusinessCodeType codeType, string sequenceName, string prefix)
    {
        var definition = BusinessCodeMetadata.Get(codeType);

        Assert.Equal(sequenceName, definition.SequenceName);
        Assert.Equal(prefix, definition.Prefix);
    }

    [Theory]
    [InlineData(1, "XXXX-0001")]
    [InlineData(12, "XXXX-0012")]
    [InlineData(9999, "XXXX-9999")]
    [InlineData(10000, "XXXX-10000")]
    public void BusinessCodesUseInvariantMinimumFourDigitFormatting(long value, string expected)
    {
        var definition = new BusinessCodeDefinition("test_code_seq", "XXXX");

        Assert.Equal(expected, definition.Format(value));
    }

    [Fact]
    public void UnsupportedCodeTypeFailsClearly()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BusinessCodeMetadata.Get((BusinessCodeType)999));
    }
}
