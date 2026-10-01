using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FarmHelm.Api.Contracts.Batches;
using FarmHelm.Api.Contracts.Farms;
using FarmHelm.Application.Batches;
using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Crops;

namespace FarmHelm.Tests.Api;

public sealed class AgriculturalCoreApiTests
{
    [Fact]
    public async Task HealthEndpointReturnsOkWithoutPostgreSql()
    {
        using var factory = new FarmHelmApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateFarmReturnsApiContractAndApplicationDefaults()
    {
        using var factory = new FarmHelmApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/farms", new { name = "API Farm" });
        var body = await response.Content.ReadFromJsonAsync<FarmResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("FARM-0001", body.FarmCode);
        var farm = Assert.Single(factory.Farms.Added);
        Assert.Equal("LKR", farm.DefaultCurrency);
        Assert.Equal("Asia/Colombo", farm.TimeZone);
    }

    [Fact]
    public async Task InvalidFarmNameReturnsValidationProblemDetails()
    {
        using var factory = new FarmHelmApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/farms", new { name = "" });

        await AssertProblemAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MissingFarmForLocationAndCropReturnsNotFoundProblemDetails()
    {
        using var factory = new FarmHelmApiFactory();
        using var client = factory.CreateClient();
        var farmId = Guid.NewGuid();

        var location = await client.PostAsJsonAsync($"/api/farms/{farmId}/locations", new { name = "Field" });
        var crop = await client.PostAsJsonAsync($"/api/farms/{farmId}/crops", new { name = "Pepper" });

        await AssertProblemAsync(location, HttpStatusCode.NotFound);
        await AssertProblemAsync(crop, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task VarietyAndStageMapMissingAndInactiveCropFailures()
    {
        using var factory = new FarmHelmApiFactory();
        using var client = factory.CreateClient();
        var missing = await client.PostAsJsonAsync($"/api/crops/{Guid.NewGuid()}/varieties", new { name = "Variety" });
        await AssertProblemAsync(missing, HttpStatusCode.NotFound);

        var crop = FarmHelm.Tests.Application.ApplicationFixture.Crop(Guid.NewGuid());
        crop.Deactivate();
        factory.Crops.Items[crop.Id] = crop;
        var inactiveVariety = await client.PostAsJsonAsync($"/api/crops/{crop.Id}/varieties", new { name = "Variety" });
        var inactiveStage = await client.PostAsJsonAsync($"/api/crops/{crop.Id}/stages", new { name = "Stage", displayOrder = 1 });

        await AssertProblemAsync(inactiveVariety, HttpStatusCode.BadRequest);
        await AssertProblemAsync(inactiveStage, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBatchReturnsCreatedAndTransportValidationRejectsInvalidPlantCount()
    {
        using var factory = new FarmHelmApiFactory();
        using var client = factory.CreateClient();
        var graph = factory.SeedGraph();
        var valid = new { farmId = graph.Farm.Id, varietyId = graph.Variety.Id, locationId = graph.Location.Id, plantingDate = new DateOnly(2026, 1, 1), initialPlantCount = 3, initialStageId = graph.Stage.Id, individualTrackingEnabled = true };

        var created = await client.PostAsJsonAsync("/api/batches", valid);
        var invalid = await client.PostAsJsonAsync("/api/batches", new { valid.farmId, valid.varietyId, valid.locationId, valid.plantingDate, initialPlantCount = 0, valid.initialStageId, valid.individualTrackingEnabled });

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.NotNull(await created.Content.ReadFromJsonAsync<BatchCreatedResponse>());
        await AssertProblemAsync(invalid, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BatchDetailsAndListFiltersUseReadRepositoryContracts()
    {
        using var factory = new FarmHelmApiFactory();
        using var client = factory.CreateClient();
        var graph = factory.SeedGraph();
        var details = CreateDetails(graph);
        factory.BatchReads.DetailsById[graph.Batch.Id] = details;
        factory.BatchReads.Summaries = [new BatchSummary(graph.Batch.Id, graph.Batch.BatchCode, graph.Farm.Id, graph.Crop.Id, graph.Variety.Id, graph.Variety.Name, graph.Batch.PlantingDate, BatchStatus.Active)];

        var found = await client.GetAsync($"/api/batches/{graph.Batch.Id}");
        var unknown = await client.GetAsync($"/api/batches/{Guid.NewGuid()}");
        var listed = await client.GetAsync($"/api/batches?farmId={graph.Farm.Id}&status=active");
        var invalidStatus = await client.GetAsync("/api/batches?status=unknown");

        Assert.Equal(HttpStatusCode.OK, found.StatusCode);
        await AssertProblemAsync(unknown, HttpStatusCode.NotFound);
        Assert.Equal(HttpStatusCode.OK, listed.StatusCode);
        Assert.Equal(graph.Farm.Id, factory.BatchReads.LastFilter!.FarmId);
        Assert.Equal(BatchStatus.Active, factory.BatchReads.LastFilter.Status);
        await AssertProblemAsync(invalidStatus, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MortalityEndpointsMapApplicationAndDomainBehavior()
    {
        using var factory = new FarmHelmApiFactory();
        using var client = factory.CreateClient();
        var quantityGraph = factory.SeedGraph();
        var batchMortality = await client.PostAsJsonAsync($"/api/batches/{quantityGraph.Batch.Id}/mortality", new { mortalityDate = new DateOnly(2026, 1, 2), quantity = 1, mortalityReasonId = quantityGraph.Reason.Id });
        Assert.Equal(HttpStatusCode.OK, batchMortality.StatusCode);

        using var trackedFactory = new FarmHelmApiFactory();
        using var trackedClient = trackedFactory.CreateClient();
        var trackedGraph = trackedFactory.SeedGraph(individuallyTracked: true);
        var plantId = trackedGraph.Batch.Plants[0].Id;
        var first = await trackedClient.PostAsJsonAsync($"/api/batches/{trackedGraph.Batch.Id}/plants/{plantId}/mortality", new { mortalityDate = new DateOnly(2026, 1, 2), mortalityReasonId = trackedGraph.Reason.Id });
        var duplicate = await trackedClient.PostAsJsonAsync($"/api/batches/{trackedGraph.Batch.Id}/plants/{plantId}/mortality", new { mortalityDate = new DateOnly(2026, 1, 3), mortalityReasonId = trackedGraph.Reason.Id });

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        await AssertProblemAsync(duplicate, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task StageChangeAndHistoricalRemovalMapDomainFailuresToBadRequest()
    {
        using var factory = new FarmHelmApiFactory();
        using var client = factory.CreateClient();
        var graph = factory.SeedGraph();
        var duplicateStage = await client.PostAsJsonAsync($"/api/batches/{graph.Batch.Id}/stage-changes", new { stageId = graph.Stage.Id, effectiveDate = new DateOnly(2026, 1, 2) });
        await AssertProblemAsync(duplicateStage, HttpStatusCode.BadRequest);

        var removal = await client.PostAsJsonAsync($"/api/batches/{graph.Batch.Id}/removal", new { removedDate = new DateOnly(2026, 1, 3) });
        var repeated = await client.PostAsJsonAsync($"/api/batches/{graph.Batch.Id}/removal", new { removedDate = new DateOnly(2026, 1, 4) });

        Assert.Equal(HttpStatusCode.NoContent, removal.StatusCode);
        await AssertProblemAsync(repeated, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UnexpectedExceptionReturnsSanitizedProblemDetails()
    {
        using var factory = new FarmHelmApiFactory();
        factory.Codes.ExceptionToThrow = new InvalidOperationException("internal diagnostic text must not leak");
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/farms", new { name = "Failure Farm" });
        var content = await response.Content.ReadAsStringAsync();

        await AssertProblemAsync(response, HttpStatusCode.InternalServerError);
        Assert.DoesNotContain("internal diagnostic text", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ConflictExceptionReturnsConflictProblemDetails()
    {
        using var factory = new FarmHelmApiFactory();
        factory.Codes.ExceptionToThrow = new ConflictException("Business code already exists.");
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/farms", new { name = "Conflict Farm" });

        await AssertProblemAsync(response, HttpStatusCode.Conflict);
    }

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal((int)expectedStatus, document.RootElement.GetProperty("status").GetInt32());
        Assert.False(document.RootElement.TryGetProperty("stackTrace", out _));
    }

    private static BatchDetails CreateDetails(AgriculturalGraph graph) => new(
        graph.Batch.Id, graph.Batch.BatchCode, graph.Crop.Id, graph.Crop.Name, graph.Variety.Id, graph.Variety.Name,
        graph.Farm.Id, graph.Farm.Name, graph.Location.Id, graph.Location.Name, graph.Batch.PlantingDate, 3, 3, 0, 100m,
        false, graph.Stage.Id, graph.Stage.Name, BatchStatus.Active, null, null, [], [], []);
}
