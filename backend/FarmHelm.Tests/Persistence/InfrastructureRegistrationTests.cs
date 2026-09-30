using FarmHelm.Infrastructure;
using FarmHelm.Infrastructure.Persistence;
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
    public void AddInfrastructureThrowsWhenFarmHelmDatabaseConnectionStringIsMissing()
    {
        var configuration = new ConfigurationManager();
        var services = new ServiceCollection();

        var exception = Assert.Throws<InvalidOperationException>(() => services.AddInfrastructure(configuration));

        Assert.Contains("FarmHelmDatabase", exception.Message, StringComparison.Ordinal);
    }
}
