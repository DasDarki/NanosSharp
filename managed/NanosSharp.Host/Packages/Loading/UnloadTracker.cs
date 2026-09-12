using System.Runtime.Loader;
using NanosSharp.Logging;

namespace NanosSharp.Host.Packages.Loading;

internal sealed class UnloadTracker
{
    private const int TicksBetweenChecks = 30;
    private const int MaximumChecks = 10;

    private readonly List<TrackedContext> _contexts = [];
    private int _ticks;

    public void Track(string packageName, AssemblyLoadContext context) =>
        _contexts.Add(new TrackedContext(packageName, new WeakReference(context)));

    public void Tick()
    {
        if (_contexts.Count == 0 || ++_ticks % TicksBetweenChecks != 0)
        {
            return;
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        for (var index = _contexts.Count - 1; index >= 0; index--)
        {
            var tracked = _contexts[index];
            if (!tracked.Context.IsAlive)
            {
                Log.Debug($"Previous assemblies of package {tracked.PackageName} were unloaded.");
                _contexts.RemoveAt(index);
            }
            else if (++tracked.Checks >= MaximumChecks)
            {
                Log.Warning($"Previous assemblies of package {tracked.PackageName} are still referenced and cannot be " +
                            "unloaded, check for running tasks, timers, threads or handlers registered outside of nanos world.");
                _contexts.RemoveAt(index);
            }
        }
    }

    private sealed class TrackedContext(string packageName, WeakReference context)
    {
        public string PackageName { get; } = packageName;

        public WeakReference Context { get; } = context;

        public int Checks { get; set; }
    }
}
