using NanosSharp.Host.Packages.Dependencies;

namespace NanosSharp.Host.Packages.Building;

internal sealed record PackageBuildOptions(NuGetRestorer Restorer, bool GenerateProjectFiles);
