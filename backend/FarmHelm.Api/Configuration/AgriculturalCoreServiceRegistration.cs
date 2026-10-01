using FarmHelm.Application.Batches.ChangeBatchStage;
using FarmHelm.Application.Batches.CreateBatch;
using FarmHelm.Application.Batches.GetBatchDetails;
using FarmHelm.Application.Batches.ListBatches;
using FarmHelm.Application.Batches.RecordBatchMortality;
using FarmHelm.Application.Batches.RecordPlantMortality;
using FarmHelm.Application.Batches.RemoveBatch;
using FarmHelm.Application.Crops.CreateCrop;
using FarmHelm.Application.Crops.CreateCropStage;
using FarmHelm.Application.Crops.CreateMortalityReason;
using FarmHelm.Application.Crops.CreateVariety;
using FarmHelm.Application.Farms.CreateFarm;
using FarmHelm.Application.Farms.CreateFarmLocation;

namespace FarmHelm.Api.Configuration;

public static class AgriculturalCoreServiceRegistration
{
    public static IServiceCollection AddAgriculturalCoreApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateFarmHandler>();
        services.AddScoped<CreateFarmLocationHandler>();
        services.AddScoped<CreateCropHandler>();
        services.AddScoped<CreateVarietyHandler>();
        services.AddScoped<CreateCropStageHandler>();
        services.AddScoped<CreateMortalityReasonHandler>();
        services.AddScoped<CreateBatchHandler>();
        services.AddScoped<RecordBatchMortalityHandler>();
        services.AddScoped<RecordPlantMortalityHandler>();
        services.AddScoped<ChangeBatchStageHandler>();
        services.AddScoped<RemoveBatchHandler>();
        services.AddScoped<GetBatchDetailsHandler>();
        services.AddScoped<ListBatchesHandler>();

        return services;
    }
}
