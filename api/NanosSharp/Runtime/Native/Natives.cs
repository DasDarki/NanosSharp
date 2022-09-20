using System.Reflection;
using System.Runtime.InteropServices;

namespace NanosSharp.Runtime.Native;

/// <summary>
/// The class containing all natives for the runtime.
/// </summary>
// ReSharper disable InconsistentNaming
internal static unsafe class Natives
{
    internal static delegate* unmanaged[Cdecl]<IntPtr, void> ScriptLog;
    internal static delegate* unmanaged[Cdecl]<IntPtr, void> ErrorLog;
    internal static delegate* unmanaged[Cdecl]<IntPtr, void> FreeString;
    internal static delegate* unmanaged[Cdecl]<IntPtr, uint, void> FreeStringArray;
    internal static delegate* unmanaged[Cdecl]<IntPtr> ICValue_CreateNull;
    internal static delegate* unmanaged[Cdecl]<Bool, IntPtr> ICValue_CreateBool;
    internal static delegate* unmanaged[Cdecl]<long, IntPtr> ICValue_CreateInteger;
    internal static delegate* unmanaged[Cdecl]<double, IntPtr> ICValue_CreateDouble;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> ICValue_CreateString;
    internal static delegate* unmanaged[Cdecl]<IntPtr> ICValue_CreateArray;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> ICValue_CreatePointer;
    internal static delegate* unmanaged[Cdecl]<IntPtr, ICValueType> ICValue_GetType;
    internal static delegate* unmanaged[Cdecl]<IntPtr, void> ICValue_Destroy;
    internal static delegate* unmanaged[Cdecl]<IntPtr, Bool> ICValue_GetBoolean;
    internal static delegate* unmanaged[Cdecl]<IntPtr, long> ICValue_GetInteger;
    internal static delegate* unmanaged[Cdecl]<IntPtr, double> ICValue_GetDouble;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int*, IntPtr> ICValue_GetString;
    internal static delegate* unmanaged[Cdecl]<IntPtr, uint> ICValue_GetArraySize;
    internal static delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr> ICValue_GetArrayElement;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> ICValue_GetPointer;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> ICValue_AddArrayElement;

    // <% AUTOGENERATE(DELEGATE_DECLARE, 4)
    
    internal static void Initialize()
    {
        const DllImportSearchPath dllImportSearchPath = DllImportSearchPath.LegacyBehavior | DllImportSearchPath.AssemblyDirectory | DllImportSearchPath.SafeDirectories | DllImportSearchPath.System32 | DllImportSearchPath.UserDirectories | DllImportSearchPath.ApplicationDirectory | DllImportSearchPath.UseDllDirectoryForDependencies;
        var handle = NativeLibrary.Load("nanossharp_runtime", Assembly.GetExecutingAssembly(), dllImportSearchPath);

        ScriptLog = (delegate* unmanaged[Cdecl]<IntPtr, void>)NativeLibrary.GetExport(handle, nameof(ScriptLog));
        ErrorLog = (delegate* unmanaged[Cdecl]<IntPtr, void>)NativeLibrary.GetExport(handle, nameof(ErrorLog));
        FreeString = (delegate* unmanaged[Cdecl]<IntPtr, void>) NativeLibrary.GetExport(handle, nameof(FreeString));
        FreeStringArray = (delegate* unmanaged[Cdecl]<IntPtr, uint, void>) NativeLibrary.GetExport(handle, nameof(FreeStringArray));
        
        ICValue_CreateNull = (delegate* unmanaged[Cdecl]<IntPtr>) NativeLibrary.GetExport(handle, nameof(ICValue_CreateNull));
        ICValue_CreateBool = (delegate* unmanaged[Cdecl]<Bool, IntPtr>) NativeLibrary.GetExport(handle, nameof(ICValue_CreateBool));
        ICValue_CreateInteger = (delegate* unmanaged[Cdecl]<long, IntPtr>) NativeLibrary.GetExport(handle, nameof(ICValue_CreateInteger));
        ICValue_CreateDouble = (delegate* unmanaged[Cdecl]<double, IntPtr>) NativeLibrary.GetExport(handle, nameof(ICValue_CreateDouble));
        ICValue_CreateString = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr>) NativeLibrary.GetExport(handle, nameof(ICValue_CreateString));
        ICValue_CreateArray = (delegate* unmanaged[Cdecl]<IntPtr>) NativeLibrary.GetExport(handle, nameof(ICValue_CreateArray));
        ICValue_CreatePointer = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr>) NativeLibrary.GetExport(handle, nameof(ICValue_CreatePointer));
        ICValue_GetType = (delegate* unmanaged[Cdecl]<IntPtr, ICValueType>) NativeLibrary.GetExport(handle, nameof(ICValue_GetType));
        ICValue_Destroy = (delegate* unmanaged[Cdecl]<IntPtr, void>) NativeLibrary.GetExport(handle, nameof(ICValue_Destroy));
        ICValue_GetBoolean = (delegate* unmanaged[Cdecl]<IntPtr, Bool>) NativeLibrary.GetExport(handle, nameof(ICValue_GetBoolean));
        ICValue_GetInteger = (delegate* unmanaged[Cdecl]<IntPtr, long>) NativeLibrary.GetExport(handle, nameof(ICValue_GetInteger));
        ICValue_GetDouble = (delegate* unmanaged[Cdecl]<IntPtr, double>) NativeLibrary.GetExport(handle, nameof(ICValue_GetDouble));
        ICValue_GetString = (delegate* unmanaged[Cdecl]<IntPtr, int*, IntPtr>) NativeLibrary.GetExport(handle, nameof(ICValue_GetString));
        ICValue_GetArraySize = (delegate* unmanaged[Cdecl]<IntPtr, uint>) NativeLibrary.GetExport(handle, nameof(ICValue_GetArraySize));
        ICValue_GetArrayElement = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr>) NativeLibrary.GetExport(handle, nameof(ICValue_GetArrayElement));
        ICValue_GetPointer = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr>) NativeLibrary.GetExport(handle, nameof(ICValue_GetPointer));
        ICValue_AddArrayElement = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void>) NativeLibrary.GetExport(handle, nameof(ICValue_AddArrayElement));
        
        // <% AUTOGENERATE(DELEGATE_LOAD, 8)
    }
}