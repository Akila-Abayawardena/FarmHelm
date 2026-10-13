using FarmHelm.Api.Contracts.Crops;
using Microsoft.AspNetCore.Mvc;
using CreateCropStageHandler = FarmHelm.Application.Crops.CreateCropStage.CreateCropStageHandler;
using CreateVarietyHandler = FarmHelm.Application.Crops.CreateVariety.CreateVarietyHandler;
using ListCropStagesHandler = FarmHelm.Application.MasterData.ListCropStages.ListCropStagesHandler;
using ListVarietiesHandler = FarmHelm.Application.MasterData.ListVarieties.ListVarietiesHandler;

namespace FarmHelm.Api.Controllers;

[ApiController]
[Route("api/crops")]
public sealed class CropsController(
    CreateVarietyHandler createVariety,
    CreateCropStageHandler createStage,
    ListVarietiesHandler listVarieties,
    ListCropStagesHandler listStages) : ControllerBase
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

    [HttpGet("{cropId:guid}/varieties")]
    [ProducesResponseType<IReadOnlyList<VarietyReadResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<VarietyReadResponse>>> ListVarietiesAsync(Guid cropId, [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var varieties = await listVarieties.HandleAsync(cropId, includeInactive, cancellationToken);
        return Ok(varieties.Select(variety => new VarietyReadResponse(variety.Id, variety.CropId, variety.VarietyCode, variety.Name, variety.Description, variety.IsActive)).ToArray());
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

    [HttpGet("{cropId:guid}/stages")]
    [ProducesResponseType<IReadOnlyList<CropStageReadResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CropStageReadResponse>>> ListStagesAsync(Guid cropId, [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var stages = await listStages.HandleAsync(cropId, includeInactive, cancellationToken);
        return Ok(stages.Select(stage => new CropStageReadResponse(stage.Id, stage.CropId, stage.Name, stage.DisplayOrder, stage.IsActive)).ToArray());
    }
}
