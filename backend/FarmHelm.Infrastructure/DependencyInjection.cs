using FarmHelm.Infrastructure.Persistence;
using FarmHelm.Infrastructure.Persistence.Codes;
using FarmHelm.Infrastructure.Persistence.Repositories;
using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Batches;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmHelm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("FarmHelmDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'FarmHelmDatabase' is required to configure persistence.");
        }

        services.AddDbContext<FarmHelmDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IFarmRepository, FarmRepository>();
        services.AddScoped<IFarmLocationRepository, FarmLocationRepository>();
        services.AddScoped<ICropRepository, CropRepository>();
        services.AddScoped<IVarietyRepository, VarietyRepository>();
        services.AddScoped<ICropStageRepository, CropStageRepository>();
        services.AddScoped<IMortalityReasonRepository, MortalityReasonRepository>();
        services.AddScoped<IBatchRepository, BatchRepository>();
        services.AddScoped<IBatchReadRepository, BatchReadRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBusinessCodeGenerator, PostgreSqlBusinessCodeGenerator>();

        return services;
    }
}
