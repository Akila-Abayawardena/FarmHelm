using System.ComponentModel.DataAnnotations;

namespace FarmHelm.Api.Contracts.Farms;

public sealed record CreateFarmRequest(
    [param: Required, StringLength(150)] string Name,
    [param: StringLength(3)] string? DefaultCurrency = null,
    [param: StringLength(100)] string? TimeZone = null,
    string? Notes = null);

public sealed record FarmResponse(Guid FarmId, string FarmCode, string Name);

public sealed record CreateFarmLocationRequest(
    [param: Required, StringLength(150)] string Name,
    string? Description = null);

public sealed record FarmLocationResponse(Guid LocationId, string LocationCode, Guid FarmId, string Name);

public sealed record CreateCropRequest(
    [param: Required, StringLength(150)] string Name,
    [param: StringLength(200)] string? ScientificName = null,
    string? Description = null);

public sealed record CropResponse(Guid CropId, string CropCode, Guid FarmId, string Name);

public sealed record CreateMortalityReasonRequest(
    [param: Required, StringLength(100)] string Name,
    [param: Range(0, int.MaxValue)] int DisplayOrder);

public sealed record MortalityReasonResponse(Guid MortalityReasonId, Guid FarmId, string Name, int DisplayOrder);
