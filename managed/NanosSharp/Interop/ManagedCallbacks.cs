using System.Runtime.InteropServices;

namespace NanosSharp.Interop;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ManagedCallbacks
{
    public delegate* unmanaged<void> EngineAttached;
    public delegate* unmanaged<void> EngineDetaching;
    public delegate* unmanaged<float, void> Tick;
    public delegate* unmanaged<ulong, nint, void> EntityCreated;
    public delegate* unmanaged<ulong, nint, void> EntityDestroyed;
    public delegate* unmanaged<long, nint, int, nint> InvokeFunction;
    public delegate* unmanaged<long, void> ReleaseFunction;
    public delegate* unmanaged<nint, byte*, byte*, nint, void> PackageLoaded;
    public delegate* unmanaged<nint, void> PackageUnloading;
}
