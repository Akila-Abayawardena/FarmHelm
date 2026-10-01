using System.ComponentModel.DataAnnotations;

namespace FarmHelm.Api.Contracts.Crops;

public sealed record CreateVarietyRequest(
    [param: Required, StringLength(150)] string Name,
    string? Description = null);

public sealed record VarietyResponse(Guid VarietyId, string VarietyCode, Guid CropId, string Name);

public sealed record CreateCropStageRequest(
    [param: Required, StringLength(100)] string Name,
    [param: Range(0, int.MaxValue)] int DisplayOrder);

public sealed record CropStageResponse(Guid CropStageId, Guid CropId, string Name, int DisplayOrder);
