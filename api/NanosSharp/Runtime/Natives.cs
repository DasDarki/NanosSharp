using System.Reflection;
using System.Runtime.InteropServices;

namespace NanosSharp.Runtime;

/// <summary>
/// The class containing all natives for the runtime.
/// </summary>
internal static unsafe class Natives
{
    internal static delegate* unmanaged[Cdecl]<IntPtr, void> FreeString;
    internal static delegate* unmanaged[Cdecl]<IntPtr, uint, void> FreeStringArray;

    // <% AUTOGENERATE(DELEGATE_DECLARE, 4)
    
    internal static void Initialize()
    {
        const DllImportSearchPath dllImportSearchPath = DllImportSearchPath.LegacyBehavior | DllImportSearchPath.AssemblyDirectory | DllImportSearchPath.SafeDirectories | DllImportSearchPath.System32 | DllImportSearchPath.UserDirectories | DllImportSearchPath.ApplicationDirectory | DllImportSearchPath.UseDllDirectoryForDependencies;
        var handle = NativeLibrary.Load("nanossharp_runtime", Assembly.GetExecutingAssembly(), dllImportSearchPath);

        FreeString = (delegate* unmanaged[Cdecl]<IntPtr, void>) NativeLibrary.GetExport(handle, nameof(FreeString));
        FreeStringArray = (delegate* unmanaged[Cdecl]<IntPtr, uint, void>) NativeLibrary.GetExport(handle, nameof(FreeStringArray));
        
        // <% AUTOGENERATE(DELEGATE_LOAD, 8)
    }
}