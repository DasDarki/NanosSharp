#if NANOSSHARP_WASM
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NanosSharp.Interop.Arguments;

namespace NanosSharp.Interop;

internal unsafe partial struct NativeApi
{
    public void Log(int a0, byte* a1, int a2) => HostLog(a0, a1, a2);
    public void FreeString(byte* a0) => HostFreeString(a0);
    public nint ValueCreateNil() => HostValueCreateNil();
    public nint ValueCreateBool(byte a0) => HostValueCreateBool(a0);
    public nint ValueCreateInteger(long a0) => HostValueCreateInteger(a0);
    public nint ValueCreateDouble(double a0) => HostValueCreateDouble(a0);
    public nint ValueCreateString(byte* a0, int a1) => HostValueCreateString(a0, a1);
    public nint ValueCreateVector(double a0, double a1, double a2) => HostValueCreateVector(a0, a1, a2);
    public nint ValueCreateRotator(double a0, double a1, double a2) => HostValueCreateRotator(a0, a1, a2);
    public nint ValueCreateColor(float a0, float a1, float a2, float a3) => HostValueCreateColor(a0, a1, a2, a3);
    public nint ValueCreateVector2D(double a0, double a1) => HostValueCreateVector2D(a0, a1);
    public nint ValueCreateEntity(ulong a0, nint a1, uint a2) => HostValueCreateEntity(a0, a1, a2);
    public nint ValueCreateArray(nint* a0, int a1) => HostValueCreateArray(a0, a1);
    public nint ValueCreateMap(nint* a0, nint* a1, int a2) => HostValueCreateMap(a0, a1, a2);
    public nint ValueCreateFunction(long a0, nint a1) => HostValueCreateFunction(a0, a1);
    public nint ValueClone(nint a0) => HostValueClone(a0);
    public void ValueFree(nint a0) => HostValueFree(a0);
    public int ValueGetType(nint a0) => HostValueGetType(a0);
    public byte ValueGetBool(nint a0) => HostValueGetBool(a0);
    public long ValueGetInteger(nint a0) => HostValueGetInteger(a0);
    public double ValueGetDouble(nint a0) => HostValueGetDouble(a0);
    public byte* ValueGetString(nint a0, int* a1) => HostValueGetString(a0, a1);
    public void ValueGetDoubles(nint a0, double* a1) => HostValueGetDoubles(a0, a1);
    public void ValueGetColor(nint a0, float* a1) => HostValueGetColor(a0, a1);
    public void ValueGetEntity(nint a0, ulong* a1, nint* a2, uint* a3) => HostValueGetEntity(a0, a1, a2, a3);
    public uint ValueGetNetworkId(nint a0) => HostValueGetNetworkId(a0);
    public nint ValueResolveDeferred(nint a0) => HostValueResolveDeferred(a0);
    public int ValueGetLength(nint a0) => HostValueGetLength(a0);
    public nint ValueGetItem(nint a0, int a1) => HostValueGetItem(a0, a1);
    public void ValueGetEntries(nint a0, nint* a1, nint* a2) => HostValueGetEntries(a0, a1, a2);
    public byte ValueInvoke(nint a0, nint* a1, int a2, nint* a3, byte** a4) => HostValueInvoke(a0, a1, a2, a3, a4);
    public int EngineIsAttached() => HostEngineIsAttached();
    public int ClassCount() => HostClassCount();
    public nint ClassGet(int a0) => HostClassGet(a0);
    public byte* ClassGetName(nint a0) => HostClassGetName(a0);
    public ulong ClassGetId(nint a0) => HostClassGetId(a0);
    public nint ClassGetConstructor(nint a0) => HostClassGetConstructor(a0);
    public int ClassGetFunctionCount(nint a0, byte a1) => HostClassGetFunctionCount(a0, a1);
    public nint ClassGetFunction(nint a0, byte a1, int a2) => HostClassGetFunction(a0, a1, a2);
    public int ClassGetPropertyCount(nint a0) => HostClassGetPropertyCount(a0);
    public nint ClassGetProperty(nint a0, int a1) => HostClassGetProperty(a0, a1);
    public int GlobalFunctionCount() => HostGlobalFunctionCount();
    public nint GlobalFunctionGet(int a0) => HostGlobalFunctionGet(a0);
    public byte* FunctionGetName(nint a0) => HostFunctionGetName(a0);
    public byte* FunctionGetSignature(nint a0) => HostFunctionGetSignature(a0);
    public byte* PropertyGetName(nint a0) => HostPropertyGetName(a0);
    public byte* PropertyGetSignature(nint a0) => HostPropertyGetSignature(a0);
    public byte FunctionInvoke(nint a0, nint a1, nint* a2, int a3, nint* a4, byte** a5) => HostFunctionInvoke(a0, a1, a2, a3, a4, a5);
    public byte PropertyGet(nint a0, nint a1, nint* a2, byte** a3) => HostPropertyGet(a0, a1, a2, a3);
    public byte PropertySet(nint a0, nint a1, nint a2, byte** a3) => HostPropertySet(a0, a1, a2, a3);
    public nint EnvironmentCreate(byte* a0, int a1) => HostEnvironmentCreate(a0, a1);
    public void EnvironmentDestroy(byte* a0, int a1) => HostEnvironmentDestroy(a0, a1);
    public byte PackageReload(nint a0, byte** a1) => HostPackageReload(a0, a1);
    public nint LibraryLoad(byte* a0, byte** a1) => HostLibraryLoad(a0, a1);
    public nint PackageGetEntity(nint a0) => HostPackageGetEntity(a0);
    public byte FunctionCall(nint a0, nint a1, NativeArgument* a2, int a3, NativeArgument* a4, byte** a5) => HostFunctionCall(a0, a1, a2, a3, a4, a5);
    public nint LuaGetGlobal(nint a0, byte* a1, int a2, byte** a3) => HostLuaGetGlobal(a0, a1, a2, a3);
    public byte LuaSetGlobal(nint a0, byte* a1, int a2, nint a3, byte** a4) => HostLuaSetGlobal(a0, a1, a2, a3, a4);

    [LibraryImport("nanos", EntryPoint = "log")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial void HostLog(int a0, byte* a1, int a2);
    [LibraryImport("nanos", EntryPoint = "free-string")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial void HostFreeString(byte* a0);
    [LibraryImport("nanos", EntryPoint = "value-create-nil")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateNil();
    [LibraryImport("nanos", EntryPoint = "value-create-bool")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateBool(byte a0);
    [LibraryImport("nanos", EntryPoint = "value-create-integer")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateInteger(long a0);
    [LibraryImport("nanos", EntryPoint = "value-create-double")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateDouble(double a0);
    [LibraryImport("nanos", EntryPoint = "value-create-string")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateString(byte* a0, int a1);
    [LibraryImport("nanos", EntryPoint = "value-create-vector")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateVector(double a0, double a1, double a2);
    [LibraryImport("nanos", EntryPoint = "value-create-rotator")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateRotator(double a0, double a1, double a2);
    [LibraryImport("nanos", EntryPoint = "value-create-color")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateColor(float a0, float a1, float a2, float a3);
    [LibraryImport("nanos", EntryPoint = "value-create-vector2-d")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateVector2D(double a0, double a1);
    [LibraryImport("nanos", EntryPoint = "value-create-entity")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateEntity(ulong a0, nint a1, uint a2);
    [LibraryImport("nanos", EntryPoint = "value-create-array")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateArray(nint* a0, int a1);
    [LibraryImport("nanos", EntryPoint = "value-create-map")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateMap(nint* a0, nint* a1, int a2);
    [LibraryImport("nanos", EntryPoint = "value-create-function")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueCreateFunction(long a0, nint a1);
    [LibraryImport("nanos", EntryPoint = "value-clone")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueClone(nint a0);
    [LibraryImport("nanos", EntryPoint = "value-free")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial void HostValueFree(nint a0);
    [LibraryImport("nanos", EntryPoint = "value-get-type")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial int HostValueGetType(nint a0);
    [LibraryImport("nanos", EntryPoint = "value-get-bool")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte HostValueGetBool(nint a0);
    [LibraryImport("nanos", EntryPoint = "value-get-integer")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial long HostValueGetInteger(nint a0);
    [LibraryImport("nanos", EntryPoint = "value-get-double")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial double HostValueGetDouble(nint a0);
    [LibraryImport("nanos", EntryPoint = "value-get-string")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte* HostValueGetString(nint a0, int* a1);
    [LibraryImport("nanos", EntryPoint = "value-get-doubles")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial void HostValueGetDoubles(nint a0, double* a1);
    [LibraryImport("nanos", EntryPoint = "value-get-color")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial void HostValueGetColor(nint a0, float* a1);
    [LibraryImport("nanos", EntryPoint = "value-get-entity")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial void HostValueGetEntity(nint a0, ulong* a1, nint* a2, uint* a3);
    [LibraryImport("nanos", EntryPoint = "value-get-network-id")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial uint HostValueGetNetworkId(nint a0);
    [LibraryImport("nanos", EntryPoint = "value-resolve-deferred")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueResolveDeferred(nint a0);
    [LibraryImport("nanos", EntryPoint = "value-get-length")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial int HostValueGetLength(nint a0);
    [LibraryImport("nanos", EntryPoint = "value-get-item")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostValueGetItem(nint a0, int a1);
    [LibraryImport("nanos", EntryPoint = "value-get-entries")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial void HostValueGetEntries(nint a0, nint* a1, nint* a2);
    [LibraryImport("nanos", EntryPoint = "value-invoke")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte HostValueInvoke(nint a0, nint* a1, int a2, nint* a3, byte** a4);
    [LibraryImport("nanos", EntryPoint = "engine-is-attached")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial int HostEngineIsAttached();
    [LibraryImport("nanos", EntryPoint = "class-count")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial int HostClassCount();
    [LibraryImport("nanos", EntryPoint = "class-get")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostClassGet(int a0);
    [LibraryImport("nanos", EntryPoint = "class-get-name")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte* HostClassGetName(nint a0);
    [LibraryImport("nanos", EntryPoint = "class-get-id")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial ulong HostClassGetId(nint a0);
    [LibraryImport("nanos", EntryPoint = "class-get-constructor")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostClassGetConstructor(nint a0);
    [LibraryImport("nanos", EntryPoint = "class-get-function-count")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial int HostClassGetFunctionCount(nint a0, byte a1);
    [LibraryImport("nanos", EntryPoint = "class-get-function")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostClassGetFunction(nint a0, byte a1, int a2);
    [LibraryImport("nanos", EntryPoint = "class-get-property-count")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial int HostClassGetPropertyCount(nint a0);
    [LibraryImport("nanos", EntryPoint = "class-get-property")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostClassGetProperty(nint a0, int a1);
    [LibraryImport("nanos", EntryPoint = "global-function-count")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial int HostGlobalFunctionCount();
    [LibraryImport("nanos", EntryPoint = "global-function-get")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostGlobalFunctionGet(int a0);
    [LibraryImport("nanos", EntryPoint = "function-get-name")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte* HostFunctionGetName(nint a0);
    [LibraryImport("nanos", EntryPoint = "function-get-signature")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte* HostFunctionGetSignature(nint a0);
    [LibraryImport("nanos", EntryPoint = "property-get-name")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte* HostPropertyGetName(nint a0);
    [LibraryImport("nanos", EntryPoint = "property-get-signature")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte* HostPropertyGetSignature(nint a0);
    [LibraryImport("nanos", EntryPoint = "function-invoke")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte HostFunctionInvoke(nint a0, nint a1, nint* a2, int a3, nint* a4, byte** a5);
    [LibraryImport("nanos", EntryPoint = "property-get")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte HostPropertyGet(nint a0, nint a1, nint* a2, byte** a3);
    [LibraryImport("nanos", EntryPoint = "property-set")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte HostPropertySet(nint a0, nint a1, nint a2, byte** a3);
    [LibraryImport("nanos", EntryPoint = "environment-create")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostEnvironmentCreate(byte* a0, int a1);
    [LibraryImport("nanos", EntryPoint = "environment-destroy")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial void HostEnvironmentDestroy(byte* a0, int a1);
    [LibraryImport("nanos", EntryPoint = "package-reload")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte HostPackageReload(nint a0, byte** a1);
    [LibraryImport("nanos", EntryPoint = "library-load")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostLibraryLoad(byte* a0, byte** a1);
    [LibraryImport("nanos", EntryPoint = "package-get-entity")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostPackageGetEntity(nint a0);
    [LibraryImport("nanos", EntryPoint = "function-call")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte HostFunctionCall(nint a0, nint a1, NativeArgument* a2, int a3, NativeArgument* a4, byte** a5);
    [LibraryImport("nanos", EntryPoint = "lua-get-global")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial nint HostLuaGetGlobal(nint a0, byte* a1, int a2, byte** a3);
    [LibraryImport("nanos", EntryPoint = "lua-set-global")]
    [WasmImportLinkage]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static partial byte HostLuaSetGlobal(nint a0, byte* a1, int a2, nint a3, byte** a4);
}
#endif
