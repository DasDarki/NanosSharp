using System.Reflection;
using System.Runtime.Loader;
using NanosSharp.Eventing;
using NanosSharp.Injection;
using NanosSharp.Scripting;

namespace NanosSharp.Packages;

internal sealed class PackageScope(string name, string directory)
{
    public string Name { get; } = name;

    public string Directory { get; } = directory;

    public nint Environment { get; set; }

    public nint Handle { get; set; }

    public bool IsActive { get; set; }

    public AssemblyLoadContext? LoadContext { get; set; }

    public HashSet<long> Functions { get; } = [];

    public HashSet<ScriptSubscription> Subscriptions { get; } = [];

    public HashSet<string> Exports { get; } = [];

    public EventDispatcher Events { get; } = new();

    public ServiceContainer? Container { get; set; }

    public bool Owns(Delegate handler) =>
        LoadContext is not null && handler.GetInvocationList().Any(entry => IsOwnedMethod(entry.Method));

    private bool IsOwnedMethod(MethodInfo method) =>
        method.DeclaringType is { } type && AssemblyLoadContext.GetLoadContext(type.Assembly) == LoadContext;
}
