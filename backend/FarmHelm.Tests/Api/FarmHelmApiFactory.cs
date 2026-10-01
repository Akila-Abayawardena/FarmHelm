using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Batches;
using FarmHelm.Domain.Crops;
using FarmHelm.Domain.Farms;
using FarmHelm.Tests.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FarmHelm.Tests.Api;

internal sealed class FarmHelmApiFactory : WebApplicationFactory<Program>
{
    internal FarmRepositoryFake Farms { get; } = new();
    internal FarmLocationRepositoryFake Locations { get; } = new();
    internal CropRepositoryFake Crops { get; } = new();
    internal VarietyRepositoryFake Varieties { get; } = new();
    internal CropStageRepositoryFake Stages { get; } = new();
    internal MortalityReasonRepositoryFake Reasons { get; } = new();
    internal BatchRepositoryFake Batches { get; } = new();
    internal BatchReadRepositoryFake BatchReads { get; } = new();
    internal UnitOfWorkFake UnitOfWork { get; } = new();
    internal BusinessCodeGeneratorFake Codes { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configuration =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:FarmHelmDatabase"] = "Host=example.invalid;Database=farmhelm_api_test"
            }));

        builder.ConfigureTestServices(services =>
        {
            Replace<IFarmRepository>(services, Farms);
            Replace<IFarmLocationRepository>(services, Locations);
            Replace<ICropRepository>(services, Crops);
            Replace<IVarietyRepository>(services, Varieties);
            Replace<ICropStageRepository>(services, Stages);
            Replace<IMortalityReasonRepository>(services, Reasons);
            Replace<IBatchRepository>(services, Batches);
            Replace<IBatchReadRepository>(services, BatchReads);
            Replace<IUnitOfWork>(services, UnitOfWork);
            Replace<IBusinessCodeGenerator>(services, Codes);
        });
    }

    internal AgriculturalGraph SeedGraph(bool individuallyTracked = false)
    {
        var farm = ApplicationFixture.Farm();
        var location = ApplicationFixture.Location(farm.Id);
        var crop = ApplicationFixture.Crop(farm.Id);
        var variety = ApplicationFixture.Variety(crop.Id);
        var stage = ApplicationFixture.Stage(crop.Id);
        var reason = ApplicationFixture.Reason(farm.Id);
        var batch = Batch.Create(Guid.NewGuid(), "BAT-0001", farm.Id, variety.Id, location.Id, new DateOnly(2026, 1, 1), 3, stage.Id, individuallyTracked);
        Farms.Items[farm.Id] = farm;
        Locations.Items[location.Id] = location;
        Crops.Items[crop.Id] = crop;
        Varieties.Items[variety.Id] = variety;
        Stages.Items[stage.Id] = stage;
        Reasons.Items[reason.Id] = reason;
        Batches.Items[batch.Id] = batch;
        return new AgriculturalGraph(farm, location, crop, variety, stage, reason, batch);
    }

    private static void Replace<TService>(IServiceCollection services, TService instance) where TService : class
    {
        services.RemoveAll<TService>();
        services.AddScoped<TService>(_ => instance);
    }
}

internal sealed record AgriculturalGraph(Farm Farm, FarmLocation Location, Crop Crop, Variety Variety, CropStage Stage, MortalityReason Reason, Batch Batch);
