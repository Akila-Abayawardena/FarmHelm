using FarmHelm.Api.Contracts.Farms;
using Microsoft.AspNetCore.Mvc;
using CreateCropHandler = FarmHelm.Application.Crops.CreateCrop.CreateCropHandler;
using CreateMortalityReasonHandler = FarmHelm.Application.Crops.CreateMortalityReason.CreateMortalityReasonHandler;
using CreateFarmHandler = FarmHelm.Application.Farms.CreateFarm.CreateFarmHandler;
using CreateFarmLocationHandler = FarmHelm.Application.Farms.CreateFarmLocation.CreateFarmLocationHandler;

namespace FarmHelm.Api.Controllers;

[ApiController]
[Route("api/farms")]
public sealed class FarmsController(
    CreateFarmHandler createFarm,
    CreateFarmLocationHandler createLocation,
    CreateCropHandler createCrop,
    CreateMortalityReasonHandler createMortalityReason) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<FarmResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FarmResponse>> CreateAsync(CreateFarmRequest request, CancellationToken cancellationToken)
    {
        var result = await createFarm.HandleAsync(new FarmHelm.Application.Farms.CreateFarm.CreateFarmRequest(request.Name, request.DefaultCurrency, request.TimeZone, request.Notes), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new FarmResponse(result.FarmId, result.FarmCode, result.Name));
    }

    [HttpPost("{farmId:guid}/locations")]
    [ProducesResponseType<FarmLocationResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FarmLocationResponse>> CreateLocationAsync(Guid farmId, CreateFarmLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await createLocation.HandleAsync(new FarmHelm.Application.Farms.CreateFarmLocation.CreateFarmLocationRequest(farmId, request.Name, request.Description), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new FarmLocationResponse(result.LocationId, result.LocationCode, result.FarmId, result.Name));
    }

    [HttpPost("{farmId:guid}/crops")]
    [ProducesResponseType<CropResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CropResponse>> CreateCropAsync(Guid farmId, CreateCropRequest request, CancellationToken cancellationToken)
    {
        var result = await createCrop.HandleAsync(new FarmHelm.Application.Crops.CreateCrop.CreateCropRequest(farmId, request.Name, request.ScientificName, request.Description), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new CropResponse(result.CropId, result.CropCode, result.FarmId, result.Name));
    }

    [HttpPost("{farmId:guid}/mortality-reasons")]
    [ProducesResponseType<MortalityReasonResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MortalityReasonResponse>> CreateMortalityReasonAsync(Guid farmId, CreateMortalityReasonRequest request, CancellationToken cancellationToken)
    {
        var result = await createMortalityReason.HandleAsync(new FarmHelm.Application.Crops.CreateMortalityReason.CreateMortalityReasonRequest(farmId, request.Name, request.DisplayOrder), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new MortalityReasonResponse(result.MortalityReasonId, result.FarmId, result.Name, result.DisplayOrder));
    }
}
