using System.Reflection;
using System.Xml.Linq;
using FarmHelm.Application.Farms.CreateFarm;
using FarmHelm.Domain.Farms;

namespace FarmHelm.Tests.Architecture;

public sealed class DependencyBoundaryTests
{
    // ProjectReference declarations in the .csproj files are the primary source of truth.
    // This reflection check validates compiled dependencies; an unused reference can be absent
    // from assembly metadata, so it cannot by itself prove that a project file is compliant.
    [Theory]
    [InlineData("FarmHelm.Domain", "FarmHelm.Application")]
    [InlineData("FarmHelm.Domain", "FarmHelm.Infrastructure")]
    [InlineData("FarmHelm.Domain", "FarmHelm.Api")]
    [InlineData("FarmHelm.Application", "FarmHelm.Infrastructure")]
    [InlineData("FarmHelm.Application", "FarmHelm.Api")]
    public void CompiledAssemblyDoesNotReferenceForbiddenFarmHelmAssembly(
        string assemblyName,
        string forbiddenAssemblyName)
    {
        var assembly = Assembly.Load(assemblyName);

        Assert.DoesNotContain(
            assembly.GetReferencedAssemblies(),
            reference => string.Equals(reference.Name, forbiddenAssemblyName, StringComparison.Ordinal));
    }

    [Fact]
    public void InfrastructureProjectReferencesApplicationAndDomain()
    {
        var projectFile = Path.Combine(
            FindRepositoryRoot(),
            "backend",
            "FarmHelm.Infrastructure",
            "FarmHelm.Infrastructure.csproj");
        var projectReferences = XDocument.Load(projectFile)
            .Descendants("ProjectReference")
            .Select(reference => Path.GetFileNameWithoutExtension(reference.Attribute("Include")?.Value ?? string.Empty) ?? string.Empty)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(["FarmHelm.Application", "FarmHelm.Domain"], projectReferences);
    }

    [Fact]
    public void ApplicationProjectReferencesDomainOnlyAndHasNoPersistenceDependencies()
    {
        var projectFile = Path.Combine(
            FindRepositoryRoot(),
            "backend",
            "FarmHelm.Application",
            "FarmHelm.Application.csproj");
        var projectReferences = XDocument.Load(projectFile)
            .Descendants("ProjectReference")
            .Select(reference => Path.GetFileNameWithoutExtension(reference.Attribute("Include")?.Value ?? string.Empty) ?? string.Empty)
            .ToArray();
        var references = typeof(CreateFarmHandler).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .ToArray();

        Assert.Equal(["FarmHelm.Domain"], projectReferences);
        Assert.DoesNotContain(references, name => name!.StartsWith("FarmHelm.Infrastructure", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name!.StartsWith("FarmHelm.Api", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name!.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name!.StartsWith("Npgsql", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name!.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name!.StartsWith("Microsoft.Extensions.Configuration", StringComparison.Ordinal));
    }

    [Fact]
    public void DomainAssemblyDoesNotReferenceFrameworkOrInfrastructureAssemblies()
    {
        var references = typeof(Farm).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .ToArray();

        Assert.DoesNotContain(references, name => name!.StartsWith("FarmHelm.", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name!.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name!.StartsWith("Npgsql", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name!.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name!.StartsWith("Microsoft.Extensions.Configuration", StringComparison.Ordinal));
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FarmHelm.sln")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the FarmHelm repository root.");
    }
}
