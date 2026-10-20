using System.Runtime.InteropServices;
using NanosSharp.Host.Packages;
using NanosSharp.Interop;
using NanosSharp.Logging;

namespace NanosSharp.Host;

internal static unsafe class Entry
{
    [UnmanagedCallersOnly]
    public static int Initialize(NativeApi* api, ManagedCallbacks* callbacks, byte* runtimeDirectory)
    {
        try
        {
            NativeRuntime.Initialize(api, callbacks);
            var directory = NativeRuntime.ReadString(runtimeDirectory);
            PackageManager.Initialize(directory);
#if CLIENT
            Packages.ClientUpdater.Start(directory);
#endif
            Log.Info($".NET {Environment.Version} runtime started.");
            return 0;
        }
        catch (Exception exception)
        {
            NativeRuntime.Log(LogLevel.Error, $"Failed to initialize NanosSharp: {exception}");
            return 1;
        }
    }
}
