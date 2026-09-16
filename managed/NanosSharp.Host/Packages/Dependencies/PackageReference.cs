using NuGet.Versioning;

namespace NanosSharp.Host.Packages.Dependencies;

internal sealed record PackageReference(string Id, VersionRange Version);
