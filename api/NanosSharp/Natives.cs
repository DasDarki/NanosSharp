using System.Reflection;
using System.Runtime.InteropServices;
using NanosSharp.API;

namespace NanosSharp;

/// <summary>
/// The class containing all natives for the runtime.
/// </summary>
// ReSharper disable InconsistentNaming
internal static unsafe class Natives
{
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_AbsIndex;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int> Lua_GetTop;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_SetTop;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_PushValue;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int, void> Lua_Rotate;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int, void> Lua_Copy;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_CheckStack;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int, void> Lua_XMove;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsNumber;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsString;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsCFunction;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsInteger;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsUserData;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, LuaType> Lua_Type;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, IntPtr> Lua_TypeName;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int*, double> Lua_ToNumberX;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int*, long> Lua_ToIntegerX;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_ToBoolean;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, IntPtr> Lua_ToLString;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, ulong> Lua_RawLen;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr> Lua_ToCFunction;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr> Lua_ToUserData;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr> Lua_ToPointer;
    internal static delegate* unmanaged[Cdecl]<IntPtr, LuaOperator, void> Lua_Arith;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int, int> Lua_RawEqual;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int, LuaComparator, int> Lua_Compare;
    internal static delegate* unmanaged[Cdecl]<IntPtr, void> Lua_PushNil;
    internal static delegate* unmanaged[Cdecl]<IntPtr, double, void> Lua_PushNumber;
    internal static delegate* unmanaged[Cdecl]<IntPtr, long, void> Lua_PushInteger;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, uint, void> Lua_PushLString;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> Lua_PushString;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int, void> Lua_PushCClosure;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_PushBoolean;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> Lua_PushLightUserData;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> Lua_GetGlobal;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_GetTable;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, int> Lua_GetField;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, long, int> Lua_GetI;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_RawGet;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, long, int> Lua_RawGetI;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, long, void> Lua_RawSetI;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, int> Lua_RawGetP;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int, void> Lua_CreateTable;
    internal static delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr> Lua_NewUserData;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_GetMetaTable;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> Lua_SetGlobal;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_SetTable;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, void> Lua_SetField;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_RawSet;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_SetMetaTable;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int, void> Lua_Call;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int, int, int> Lua_PCall;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_Yield;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, double> Lua_ToNumber;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, long> Lua_ToInteger;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_Pop;
    internal static delegate* unmanaged[Cdecl]<IntPtr, void> Lua_NewTable;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> Lua_PushCFunction;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsFunction;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsTable;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsLightUserData;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsNil;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsBoolean;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_IsNone;
    internal static delegate* unmanaged[Cdecl]<IntPtr, void> Lua_PushGlobalTable;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_GetUserValue;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_SetUserValue;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_Insert;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_Remove;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_Replace;
    internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> Lua_NewMetaTable;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> Lua_Next;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_Concat;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, void> Lua_Len;
    
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int> LuaL_Ref;
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, int, void> LuaL_Unref;
    
    internal static IntPtr ManagedFunctionWrapper;

    internal static void Initialize()
    {
        const DllImportSearchPath dllImportSearchPath = DllImportSearchPath.LegacyBehavior | DllImportSearchPath.AssemblyDirectory | DllImportSearchPath.SafeDirectories | DllImportSearchPath.System32 | DllImportSearchPath.UserDirectories | DllImportSearchPath.ApplicationDirectory | DllImportSearchPath.UseDllDirectoryForDependencies;
        var handle = NativeLibrary.Load("nanossharp_runtime", Assembly.GetExecutingAssembly(), dllImportSearchPath);

        Lua_AbsIndex = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_AbsIndex));
        Lua_GetTop = (delegate* unmanaged[Cdecl]<IntPtr, int>) NativeLibrary.GetExport(handle, nameof(Lua_GetTop));
        Lua_SetTop = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_SetTop));
        Lua_PushValue = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushValue));
        Lua_Rotate = (delegate* unmanaged[Cdecl]<IntPtr, int, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_Rotate));
        Lua_Copy = (delegate* unmanaged[Cdecl]<IntPtr, int, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_Copy));
        Lua_CheckStack = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_CheckStack));
        Lua_XMove = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_XMove));
        Lua_IsNumber = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsNumber));
        Lua_IsString = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsString));
        Lua_IsCFunction = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsCFunction));
        Lua_IsInteger = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsInteger));
        Lua_IsUserData = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsUserData));
        Lua_Type = (delegate* unmanaged[Cdecl]<IntPtr, int, LuaType>) NativeLibrary.GetExport(handle, nameof(Lua_Type));
        Lua_TypeName = (delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, IntPtr>) NativeLibrary.GetExport(handle, nameof(Lua_TypeName));
        Lua_ToNumberX = (delegate* unmanaged[Cdecl]<IntPtr, int, int*, double>) NativeLibrary.GetExport(handle, nameof(Lua_ToNumberX));
        Lua_ToIntegerX = (delegate* unmanaged[Cdecl]<IntPtr, int, int*, long>) NativeLibrary.GetExport(handle, nameof(Lua_ToIntegerX));
        Lua_ToBoolean = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_ToBoolean));
        Lua_ToLString = (delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, IntPtr>) NativeLibrary.GetExport(handle, nameof(Lua_ToLString));
        Lua_RawLen = (delegate* unmanaged[Cdecl]<IntPtr, int, ulong>) NativeLibrary.GetExport(handle, nameof(Lua_RawLen));
        Lua_ToCFunction = (delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr>) NativeLibrary.GetExport(handle, nameof(Lua_ToCFunction));
        Lua_ToUserData = (delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr>) NativeLibrary.GetExport(handle, nameof(Lua_ToUserData));
        Lua_ToPointer = (delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr>) NativeLibrary.GetExport(handle, nameof(Lua_ToPointer));
        Lua_Arith = (delegate* unmanaged[Cdecl]<IntPtr, LuaOperator, void>) NativeLibrary.GetExport(handle, nameof(Lua_Arith));
        Lua_RawEqual = (delegate* unmanaged[Cdecl]<IntPtr, int, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_RawEqual));
        Lua_Compare = (delegate* unmanaged[Cdecl]<IntPtr, int, int, LuaComparator, int>) NativeLibrary.GetExport(handle, nameof(Lua_Compare));
        Lua_PushNil = (delegate* unmanaged[Cdecl]<IntPtr, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushNil));
        Lua_PushNumber = (delegate* unmanaged[Cdecl]<IntPtr, double, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushNumber));
        Lua_PushInteger = (delegate* unmanaged[Cdecl]<IntPtr, long, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushInteger));
        Lua_PushLString = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, uint, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushLString));
        Lua_PushString = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushString));
        Lua_PushCClosure = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushCClosure));
        Lua_PushBoolean = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushBoolean));
        Lua_PushLightUserData = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushLightUserData));
        Lua_GetGlobal = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>) NativeLibrary.GetExport(handle, nameof(Lua_GetGlobal));
        Lua_GetTable = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_GetTable));
        Lua_GetField = (delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, int>) NativeLibrary.GetExport(handle, nameof(Lua_GetField));
        Lua_GetI = (delegate* unmanaged[Cdecl]<IntPtr, int, long, int>) NativeLibrary.GetExport(handle, nameof(Lua_GetI));
        Lua_RawGet = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_RawGet));
        Lua_RawGetI = (delegate* unmanaged[Cdecl]<IntPtr, int, long, int>) NativeLibrary.GetExport(handle, nameof(Lua_RawGetI));
        Lua_RawSetI = (delegate* unmanaged[Cdecl]<IntPtr, int, long, void>) NativeLibrary.GetExport(handle, nameof(Lua_RawSetI));
        Lua_RawGetP = (delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, int>) NativeLibrary.GetExport(handle, nameof(Lua_RawGetP));
        Lua_CreateTable = (delegate* unmanaged[Cdecl]<IntPtr, int, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_CreateTable));
        Lua_NewUserData = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr>) NativeLibrary.GetExport(handle, nameof(Lua_NewUserData));
        Lua_GetMetaTable = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_GetMetaTable));
        Lua_SetGlobal = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void>) NativeLibrary.GetExport(handle, nameof(Lua_SetGlobal));
        Lua_SetTable = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_SetTable));
        Lua_SetField = (delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, void>) NativeLibrary.GetExport(handle, nameof(Lua_SetField));
        Lua_RawSet = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_RawSet));
        Lua_SetMetaTable = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_SetMetaTable));
        Lua_Call = (delegate* unmanaged[Cdecl]<IntPtr, int, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_Call));
        Lua_PCall = (delegate* unmanaged[Cdecl]<IntPtr, int, int, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_PCall));
        Lua_Yield = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_Yield));
        Lua_ToNumber = (delegate* unmanaged[Cdecl]<IntPtr, int, double>) NativeLibrary.GetExport(handle, nameof(Lua_ToNumber));
        Lua_ToInteger = (delegate* unmanaged[Cdecl]<IntPtr, int, long>) NativeLibrary.GetExport(handle, nameof(Lua_ToInteger));
        Lua_Pop = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_Pop));
        Lua_NewTable = (delegate* unmanaged[Cdecl]<IntPtr, void>) NativeLibrary.GetExport(handle, nameof(Lua_NewTable));
        Lua_PushCFunction = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushCFunction));
        Lua_IsFunction = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsFunction));
        Lua_IsTable = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsTable));
        Lua_IsLightUserData = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsLightUserData));
        Lua_IsNil = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsNil));
        Lua_IsBoolean = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsBoolean));
        Lua_IsNone = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_IsNone));
        Lua_PushGlobalTable = (delegate* unmanaged[Cdecl]<IntPtr, void>) NativeLibrary.GetExport(handle, nameof(Lua_PushGlobalTable));
        Lua_GetUserValue = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_GetUserValue));
        Lua_SetUserValue = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_SetUserValue));
        Lua_Insert = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_Insert));
        Lua_Remove = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_Remove));
        Lua_Replace = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_Replace));
        Lua_NewMetaTable = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>) NativeLibrary.GetExport(handle, nameof(Lua_NewMetaTable));
        Lua_Next = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(Lua_Next));
        Lua_Concat = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_Concat));
        Lua_Len = (delegate* unmanaged[Cdecl]<IntPtr, int, void>) NativeLibrary.GetExport(handle, nameof(Lua_Len));
        
        LuaL_Ref = (delegate* unmanaged[Cdecl]<IntPtr, int, int>) NativeLibrary.GetExport(handle, nameof(LuaL_Ref));
        LuaL_Unref = (delegate* unmanaged[Cdecl]<IntPtr, int, int, void>) NativeLibrary.GetExport(handle, nameof(LuaL_Unref));
        
        ManagedFunctionWrapper = NativeLibrary.GetExport(handle, nameof(ManagedFunctionWrapper));
    }
}