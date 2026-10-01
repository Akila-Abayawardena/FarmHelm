using FarmHelm.Infrastructure;
using FarmHelm.Infrastructure.Persistence;
using FarmHelm.Application.Abstractions.Codes;
using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Batches;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmHelm.Tests.Persistence;

public sealed class InfrastructureRegistrationTests
{
    [Fact]
    public void AddInfrastructureRegistersDbContextOptionsWithoutConnectingToDatabase()
    {
        var configuration = new ConfigurationManager
        {
            ["ConnectionStrings:FarmHelmDatabase"] = "Host=example.invalid;Database=farmhelm_test"
        };
        var services = new ServiceCollection();

        services.AddInfrastructure(configuration);

        Assert.Contains(
            services,
            registration => registration.ServiceType == typeof(DbContextOptions<FarmHelmDbContext>));
    }

    [Fact]
    public void AddInfrastructureRegistersAllAgriculturalCorePersistenceContracts()
    {
        var configuration = new ConfigurationManager
        {
            ["ConnectionStrings:FarmHelmDatabase"] = "Host=example.invalid;Database=farmhelm_test"
        };
        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        Assert.IsAssignableFrom<IFarmRepository>(serviceProvider.GetRequiredService<IFarmRepository>());
        Assert.IsAssignableFrom<IFarmLocationRepository>(serviceProvider.GetRequiredService<IFarmLocationRepository>());
        Assert.IsAssignableFrom<ICropRepository>(serviceProvider.GetRequiredService<ICropRepository>());
        Assert.IsAssignableFrom<IVarietyRepository>(serviceProvider.GetRequiredService<IVarietyRepository>());
        Assert.IsAssignableFrom<ICropStageRepository>(serviceProvider.GetRequiredService<ICropStageRepository>());
        Assert.IsAssignableFrom<IMortalityReasonRepository>(serviceProvider.GetRequiredService<IMortalityReasonRepository>());
        Assert.IsAssignableFrom<IBatchRepository>(serviceProvider.GetRequiredService<IBatchRepository>());
        Assert.IsAssignableFrom<IBatchReadRepository>(serviceProvider.GetRequiredService<IBatchReadRepository>());
        Assert.IsAssignableFrom<IUnitOfWork>(serviceProvider.GetRequiredService<IUnitOfWork>());
        Assert.IsAssignableFrom<IBusinessCodeGenerator>(serviceProvider.GetRequiredService<IBusinessCodeGenerator>());
    }

    [Fact]
    public void AddInfrastructureThrowsWhenFarmHelmDatabaseConnectionStringIsMissing()
    {
        var configuration = new ConfigurationManager();
        var services = new ServiceCollection();

        var exception = Assert.Throws<InvalidOperationException>(() => services.AddInfrastructure(configuration));

        Assert.Contains("FarmHelmDatabase", exception.Message, StringComparison.Ordinal);
    }
}
