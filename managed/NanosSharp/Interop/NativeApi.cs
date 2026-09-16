using System.Runtime.InteropServices;

namespace NanosSharp.Interop;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeApi
{
    public const int SupportedVersion = 1;

    public int Version;

    public delegate* unmanaged<int, byte*, int, void> Log;
    public delegate* unmanaged<byte*, void> FreeString;

    public delegate* unmanaged<nint> ValueCreateNil;
    public delegate* unmanaged<byte, nint> ValueCreateBool;
    public delegate* unmanaged<long, nint> ValueCreateInteger;
    public delegate* unmanaged<double, nint> ValueCreateDouble;
    public delegate* unmanaged<byte*, int, nint> ValueCreateString;
    public delegate* unmanaged<double, double, double, nint> ValueCreateVector;
    public delegate* unmanaged<double, double, double, nint> ValueCreateRotator;
    public delegate* unmanaged<float, float, float, float, nint> ValueCreateColor;
    public delegate* unmanaged<double, double, nint> ValueCreateVector2D;
    public delegate* unmanaged<ulong, nint, nint> ValueCreateEntity;
    public delegate* unmanaged<nint*, int, nint> ValueCreateArray;
    public delegate* unmanaged<nint*, nint*, int, nint> ValueCreateMap;
    public delegate* unmanaged<long, nint, nint> ValueCreateFunction;
    public delegate* unmanaged<nint, nint> ValueClone;
    public delegate* unmanaged<nint, void> ValueFree;

    public delegate* unmanaged<nint, int> ValueGetType;
    public delegate* unmanaged<nint, byte> ValueGetBool;
    public delegate* unmanaged<nint, long> ValueGetInteger;
    public delegate* unmanaged<nint, double> ValueGetDouble;
    public delegate* unmanaged<nint, int*, byte*> ValueGetString;
    public delegate* unmanaged<nint, double*, void> ValueGetDoubles;
    public delegate* unmanaged<nint, float*, void> ValueGetColor;
    public delegate* unmanaged<nint, ulong*, nint*, void> ValueGetEntity;
    public delegate* unmanaged<nint, uint> ValueGetNetworkId;
    public delegate* unmanaged<nint, nint> ValueResolveDeferred;
    public delegate* unmanaged<nint, int> ValueGetLength;
    public delegate* unmanaged<nint, int, nint> ValueGetItem;
    public delegate* unmanaged<nint, nint*, nint*, void> ValueGetEntries;
    public delegate* unmanaged<nint, nint*, int, nint*, byte**, byte> ValueInvoke;

    public delegate* unmanaged<int> EngineIsAttached;
    public delegate* unmanaged<int> ClassCount;
    public delegate* unmanaged<int, nint> ClassGet;
    public delegate* unmanaged<nint, byte*> ClassGetName;
    public delegate* unmanaged<nint, ulong> ClassGetId;
    public delegate* unmanaged<nint, nint> ClassGetConstructor;
    public delegate* unmanaged<nint, byte, int> ClassGetFunctionCount;
    public delegate* unmanaged<nint, byte, int, nint> ClassGetFunction;
    public delegate* unmanaged<nint, int> ClassGetPropertyCount;
    public delegate* unmanaged<nint, int, nint> ClassGetProperty;
    public delegate* unmanaged<int> GlobalFunctionCount;
    public delegate* unmanaged<int, nint> GlobalFunctionGet;
    public delegate* unmanaged<nint, byte*> FunctionGetName;
    public delegate* unmanaged<nint, byte*> FunctionGetSignature;
    public delegate* unmanaged<nint, byte*> PropertyGetName;
    public delegate* unmanaged<nint, byte*> PropertyGetSignature;

    public delegate* unmanaged<nint, nint, nint*, int, nint*, byte**, byte> FunctionInvoke;
    public delegate* unmanaged<nint, nint, nint*, byte**, byte> PropertyGet;
    public delegate* unmanaged<nint, nint, nint, byte**, byte> PropertySet;

    public delegate* unmanaged<byte*, int, nint> EnvironmentCreate;
    public delegate* unmanaged<byte*, int, void> EnvironmentDestroy;
    public delegate* unmanaged<nint, byte**, byte> PackageReload;
    public delegate* unmanaged<byte*, byte**, nint> LibraryLoad;
}
