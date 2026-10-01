using FarmHelm.Api.Contracts.Crops;
using Microsoft.AspNetCore.Mvc;
using CreateCropStageHandler = FarmHelm.Application.Crops.CreateCropStage.CreateCropStageHandler;
using CreateVarietyHandler = FarmHelm.Application.Crops.CreateVariety.CreateVarietyHandler;

namespace FarmHelm.Api.Controllers;

[ApiController]
[Route("api/crops")]
public sealed class CropsController(CreateVarietyHandler createVariety, CreateCropStageHandler createStage) : ControllerBase
{
    [HttpPost("{cropId:guid}/varieties")]
    [ProducesResponseType<VarietyResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VarietyResponse>> CreateVarietyAsync(Guid cropId, CreateVarietyRequest request, CancellationToken cancellationToken)
    {
        var result = await createVariety.HandleAsync(new FarmHelm.Application.Crops.CreateVariety.CreateVarietyRequest(cropId, request.Name, request.Description), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new VarietyResponse(result.VarietyId, result.VarietyCode, result.CropId, result.Name));
    }

    [HttpPost("{cropId:guid}/stages")]
    [ProducesResponseType<CropStageResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CropStageResponse>> CreateStageAsync(Guid cropId, CreateCropStageRequest request, CancellationToken cancellationToken)
    {
        var result = await createStage.HandleAsync(new FarmHelm.Application.Crops.CreateCropStage.CreateCropStageRequest(cropId, request.Name, request.DisplayOrder), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new CropStageResponse(result.CropStageId, result.CropId, result.Name, result.DisplayOrder));
    }
}
