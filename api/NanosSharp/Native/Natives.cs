using System.Reflection;
using System.Runtime.InteropServices;

namespace NanosSharp.Native;

/// <summary>
/// The class containing all natives for the runtime.
/// </summary>
// ReSharper disable InconsistentNaming
internal static unsafe class Natives
{
    
    internal static void Initialize()
    {
        const DllImportSearchPath dllImportSearchPath = DllImportSearchPath.LegacyBehavior | DllImportSearchPath.AssemblyDirectory | DllImportSearchPath.SafeDirectories | DllImportSearchPath.System32 | DllImportSearchPath.UserDirectories | DllImportSearchPath.ApplicationDirectory | DllImportSearchPath.UseDllDirectoryForDependencies;
        var handle = NativeLibrary.Load("nanossharp_runtime", Assembly.GetExecutingAssembly(), dllImportSearchPath);

        
    }
}