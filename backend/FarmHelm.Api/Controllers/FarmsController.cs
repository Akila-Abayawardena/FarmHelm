using FarmHelm.Api.Contracts.Farms;
using Microsoft.AspNetCore.Mvc;
using CreateCropHandler = FarmHelm.Application.Crops.CreateCrop.CreateCropHandler;
using CreateMortalityReasonHandler = FarmHelm.Application.Crops.CreateMortalityReason.CreateMortalityReasonHandler;
using CreateFarmHandler = FarmHelm.Application.Farms.CreateFarm.CreateFarmHandler;
using CreateFarmLocationHandler = FarmHelm.Application.Farms.CreateFarmLocation.CreateFarmLocationHandler;
using ListCropsHandler = FarmHelm.Application.MasterData.ListCrops.ListCropsHandler;
using ListFarmLocationsHandler = FarmHelm.Application.MasterData.ListFarmLocations.ListFarmLocationsHandler;
using ListFarmsHandler = FarmHelm.Application.MasterData.ListFarms.ListFarmsHandler;
using ListMortalityReasonsHandler = FarmHelm.Application.MasterData.ListMortalityReasons.ListMortalityReasonsHandler;

namespace FarmHelm.Api.Controllers;

[ApiController]
[Route("api/farms")]
public sealed class FarmsController(
    CreateFarmHandler createFarm,
    CreateFarmLocationHandler createLocation,
    CreateCropHandler createCrop,
    CreateMortalityReasonHandler createMortalityReason,
    ListFarmsHandler listFarms,
    ListFarmLocationsHandler listLocations,
    ListCropsHandler listCrops,
    ListMortalityReasonsHandler listMortalityReasons) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<FarmReadResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FarmReadResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var farms = await listFarms.HandleAsync(cancellationToken);
        return Ok(farms.Select(farm => new FarmReadResponse(farm.Id, farm.FarmCode, farm.Name, farm.DefaultCurrency, farm.TimeZone, farm.Notes)).ToArray());
    }

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

    [HttpGet("{farmId:guid}/locations")]
    [ProducesResponseType<IReadOnlyList<FarmLocationReadResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<FarmLocationReadResponse>>> ListLocationsAsync(Guid farmId, [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var locations = await listLocations.HandleAsync(farmId, includeInactive, cancellationToken);
        return Ok(locations.Select(location => new FarmLocationReadResponse(location.Id, location.FarmId, location.LocationCode, location.Name, location.Description, location.IsActive)).ToArray());
    }

    [HttpPost("{farmId:guid}/crops")]
    [ProducesResponseType<CropResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CropResponse>> CreateCropAsync(Guid farmId, CreateCropRequest request, CancellationToken cancellationToken)
    {
        var result = await createCrop.HandleAsync(new FarmHelm.Application.Crops.CreateCrop.CreateCropRequest(farmId, request.Name, request.ScientificName, request.Description), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new CropResponse(result.CropId, result.CropCode, result.FarmId, result.Name));
    }

    [HttpGet("{farmId:guid}/crops")]
    [ProducesResponseType<IReadOnlyList<CropReadResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CropReadResponse>>> ListCropsAsync(Guid farmId, [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var crops = await listCrops.HandleAsync(farmId, includeInactive, cancellationToken);
        return Ok(crops.Select(crop => new CropReadResponse(crop.Id, crop.FarmId, crop.CropCode, crop.Name, crop.ScientificName, crop.Description, crop.IsActive)).ToArray());
    }

    [HttpPost("{farmId:guid}/mortality-reasons")]
    [ProducesResponseType<MortalityReasonResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MortalityReasonResponse>> CreateMortalityReasonAsync(Guid farmId, CreateMortalityReasonRequest request, CancellationToken cancellationToken)
    {
        var result = await createMortalityReason.HandleAsync(new FarmHelm.Application.Crops.CreateMortalityReason.CreateMortalityReasonRequest(farmId, request.Name, request.DisplayOrder), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new MortalityReasonResponse(result.MortalityReasonId, result.FarmId, result.Name, result.DisplayOrder));
    }

    [HttpGet("{farmId:guid}/mortality-reasons")]
    [ProducesResponseType<IReadOnlyList<MortalityReasonReadResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<MortalityReasonReadResponse>>> ListMortalityReasonsAsync(Guid farmId, [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var reasons = await listMortalityReasons.HandleAsync(farmId, includeInactive, cancellationToken);
        return Ok(reasons.Select(reason => new MortalityReasonReadResponse(reason.Id, reason.FarmId, reason.Name, reason.DisplayOrder, reason.IsActive)).ToArray());
    }
}
