using System.Reflection;

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
}
