using NanosSharp.Scripting;

namespace NanosSharp.Packages;

internal sealed class PackageScope(string name, string directory)
{
    public string Name { get; } = name;

    public string Directory { get; } = directory;

    public nint Environment { get; set; }

    public bool IsActive { get; set; }

    public HashSet<long> Functions { get; } = [];

    public HashSet<ScriptSubscription> Subscriptions { get; } = [];
}
