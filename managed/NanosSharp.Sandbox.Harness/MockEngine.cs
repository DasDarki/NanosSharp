using System.Runtime.InteropServices;
using System.Text;
using NanosSharp.Interop;
using NanosSharp.Interop.Arguments;

namespace NanosSharp.Sandbox.Harness;

internal sealed unsafe class MockValue
{
    public int Type;
    public long Integer;
    public double Number;
    public nint TextPointer;
    public int TextLength;
    public readonly double[] Doubles = new double[3];
    public readonly float[] Colors = new float[4];
    public List<nint>? Items;
    public List<nint>? Keys;
}

internal static unsafe class MockEngine
{
    private const long PlayerToken = 100;
    private const long VehicleToken = 101;
    private const long ClientToken = 102;

    internal static readonly List<long> CreatedFunctions = new();

    private static readonly Dictionary<nint, MockValue> Values = new();
    private static readonly Dictionary<string, nint> Names = new();
    private static nint _nextValue = 1;
    private static nint _reply;

    public static NativeApi Build()
    {
        NativeApi api = default;
        api.Version = NativeApi.SupportedVersion;
        api.Log = &Log;
        api.FreeString = &FreeString;
        api.EngineIsAttached = &EngineIsAttached;
        api.ClassCount = &ClassCount;
        api.ClassGet = &ClassGet;
        api.ClassGetName = &ClassGetName;
        api.ClassGetId = &ClassGetId;
        api.ClassGetConstructor = &ClassGetConstructor;
        api.ClassGetFunctionCount = &ClassGetFunctionCount;
        api.ClassGetFunction = &ClassGetFunction;
        api.ClassGetPropertyCount = &ClassGetPropertyCount;
        api.ClassGetProperty = &ClassGetProperty;
        api.GlobalFunctionCount = &GlobalFunctionCount;
        api.GlobalFunctionGet = &GlobalFunctionGet;
        api.FunctionGetName = &FunctionGetName;
        api.PropertyGetName = &PropertyGetName;
        api.FunctionCall = &FunctionCall;
        api.ValueCreateNil = &ValueCreateNil;
        api.ValueCreateBool = &ValueCreateBool;
        api.ValueCreateInteger = &ValueCreateInteger;
        api.ValueCreateDouble = &ValueCreateDouble;
        api.ValueCreateString = &ValueCreateString;
        api.ValueCreateVector = &ValueCreateVector;
        api.ValueCreateRotator = &ValueCreateRotator;
        api.ValueCreateColor = &ValueCreateColor;
        api.ValueCreateVector2D = &ValueCreateVector2D;
        api.ValueCreateEntity = &ValueCreateEntity;
        api.ValueCreateArray = &ValueCreateArray;
        api.ValueCreateMap = &ValueCreateMap;
        api.ValueClone = &ValueClone;
        api.ValueFree = &ValueFree;
        api.ValueGetType = &ValueGetType;
        api.ValueGetBool = &ValueGetBool;
        api.ValueGetInteger = &ValueGetInteger;
        api.ValueGetDouble = &ValueGetDouble;
        api.ValueGetString = &ValueGetString;
        api.ValueGetDoubles = &ValueGetDoubles;
        api.ValueGetColor = &ValueGetColor;
        api.ValueGetLength = &ValueGetLength;
        api.ValueGetItem = &ValueGetItem;
        api.ValueGetEntries = &ValueGetEntries;
        api.PropertyGet = &PropertyGet;
        api.PropertySet = &PropertySet;
        api.ValueCreateFunction = &ValueCreateFunction;
        api.EnvironmentCreate = &EnvironmentCreate;
        api.EnvironmentDestroy = &EnvironmentDestroy;
        return api;
    }

    private static byte* Name(string value)
    {
        if (!Names.TryGetValue(value, out var pointer))
        {
            pointer = Marshal.StringToCoTaskMemUTF8(value);
            Names[value] = pointer;
        }

        return (byte*)pointer;
    }

    private static nint Register(MockValue value)
    {
        var handle = _nextValue++;
        Values[handle] = value;
        return handle;
    }

    private static MockValue Resolve(nint handle) => Values[handle];

    private static nint CloneHandle(nint handle)
    {
        var source = Resolve(handle);
        var clone = new MockValue
        {
            Type = source.Type,
            Integer = source.Integer,
            Number = source.Number,
        };
        Array.Copy(source.Doubles, clone.Doubles, 3);
        Array.Copy(source.Colors, clone.Colors, 4);

        if (source.TextPointer != 0)
        {
            clone.TextPointer = (nint)NativeMemory.Alloc((nuint)source.TextLength);
            clone.TextLength = source.TextLength;
            Buffer.MemoryCopy((void*)source.TextPointer, (void*)clone.TextPointer, source.TextLength, source.TextLength);
        }

        if (source.Items != null)
        {
            clone.Items = new List<nint>(source.Items);
        }

        if (source.Keys != null)
        {
            clone.Keys = new List<nint>(source.Keys);
        }

        return Register(clone);
    }

    [UnmanagedCallersOnly]
    private static void Log(int type, byte* message, int length)
    {
        var text = length <= 0 ? string.Empty : Encoding.UTF8.GetString(message, length);
        Console.WriteLine($"[guest/{type}] {text}");
    }

    [UnmanagedCallersOnly]
    private static void FreeString(byte* value)
    {
    }

    [UnmanagedCallersOnly]
    private static int EngineIsAttached() => 0;

    [UnmanagedCallersOnly]
    private static int ClassCount() => 3;

    [UnmanagedCallersOnly]
    private static nint ClassGet(int index) => (nint)(index switch
    {
        0 => PlayerToken,
        1 => VehicleToken,
        _ => ClientToken,
    });

    [UnmanagedCallersOnly]
    private static byte* ClassGetName(nint handle) => Name(handle == PlayerToken ? "Player" : handle == VehicleToken ? "Vehicle" : "Client");

    [UnmanagedCallersOnly]
    private static ulong ClassGetId(nint handle) => handle == PlayerToken ? 1001UL : handle == VehicleToken ? 1002UL : 1003UL;

    [UnmanagedCallersOnly]
    private static nint ClassGetConstructor(nint handle) => handle == PlayerToken ? 10 : 0;

    [UnmanagedCallersOnly]
    private static int ClassGetFunctionCount(nint handle, byte isStatic)
    {
        if (handle == PlayerToken)
        {
            return isStatic != 0 ? 1 : 2;
        }

        if (handle == ClientToken)
        {
            return isStatic != 0 ? 2 : 0;
        }

        return isStatic != 0 ? 0 : 1;
    }

    [UnmanagedCallersOnly]
    private static nint ClassGetFunction(nint handle, byte isStatic, int index)
    {
        if (handle == PlayerToken)
        {
            if (isStatic != 0)
            {
                return 13;
            }

            return index == 0 ? 11 : 12;
        }

        if (handle == ClientToken)
        {
            return index == 0 ? 40 : 41;
        }

        return 21;
    }

    [UnmanagedCallersOnly]
    private static int ClassGetPropertyCount(nint handle) => handle == PlayerToken ? 1 : 0;

    [UnmanagedCallersOnly]
    private static nint ClassGetProperty(nint handle, int index) => 14;

    [UnmanagedCallersOnly]
    private static int GlobalFunctionCount() => 1;

    [UnmanagedCallersOnly]
    private static nint GlobalFunctionGet(int index) => 31;

    [UnmanagedCallersOnly]
    private static byte* FunctionGetName(nint handle) => Name(handle switch
    {
        10 => "Constructor",
        11 => "GetName",
        12 => "SetHealth",
        13 => "GetAll",
        21 => "Honk",
        31 => "GetWorldTime",
        40 => "Subscribe",
        41 => "Unsubscribe",
        _ => "Unknown",
    });

    [UnmanagedCallersOnly]
    private static byte* PropertyGetName(nint handle) => Name("Health");

    [UnmanagedCallersOnly]
    private static byte FunctionCall(nint function, nint environment, NativeArgument* args, int count, NativeArgument* result, byte** error)
    {
        *result = default;
        result->Type = 1;

        if (count == 0)
        {
            result->Integer = 42;
            return 1;
        }

        if (count == 2)
        {
            if (_reply == 0)
            {
                _reply = (nint)Name("pong from host");
            }

            result->Type = 3;
            result->StringData = _reply;
            result->StringLength = 14;
            return 1;
        }

        if (count == 3)
        {
            result->Type = 4;
            result->ClassId = 1001;
            result->Instance = 0xE0001;
            result->NetworkId = 7;
            return 1;
        }

        var first = args[0];
        if (first.Type == 3)
        {
            var text = Encoding.UTF8.GetString((byte*)first.StringData, (int)first.StringLength);
            Console.WriteLine($"[host] string arg length={first.StringLength} '{text}'");
            result->Integer = first.StringLength;
        }
        else
        {
            result->Integer = first.Type;
        }

        return 1;
    }

    [UnmanagedCallersOnly]
    private static nint ValueCreateNil() => Register(new MockValue { Type = 7 });

    [UnmanagedCallersOnly]
    private static nint ValueCreateBool(byte boolean) => Register(new MockValue { Type = 0, Integer = boolean != 0 ? 1 : 0 });

    [UnmanagedCallersOnly]
    private static nint ValueCreateInteger(long integer) => Register(new MockValue { Type = 1, Integer = integer });

    [UnmanagedCallersOnly]
    private static nint ValueCreateDouble(double number) => Register(new MockValue { Type = 2, Number = number });

    [UnmanagedCallersOnly]
    private static nint ValueCreateString(byte* text, int length)
    {
        var value = new MockValue
        {
            Type = 3,
            TextLength = length < 0 ? 0 : length,
        };
        if (value.TextLength > 0)
        {
            value.TextPointer = (nint)NativeMemory.Alloc((nuint)value.TextLength);
            Buffer.MemoryCopy(text, (void*)value.TextPointer, value.TextLength, value.TextLength);
        }

        return Register(value);
    }

    [UnmanagedCallersOnly]
    private static nint ValueCreateVector(double x, double y, double z)
    {
        var value = new MockValue { Type = 10 };
        value.Doubles[0] = x;
        value.Doubles[1] = y;
        value.Doubles[2] = z;
        return Register(value);
    }

    [UnmanagedCallersOnly]
    private static nint ValueCreateRotator(double pitch, double yaw, double roll)
    {
        var value = new MockValue { Type = 11 };
        value.Doubles[0] = pitch;
        value.Doubles[1] = yaw;
        value.Doubles[2] = roll;
        return Register(value);
    }

    [UnmanagedCallersOnly]
    private static nint ValueCreateColor(float r, float g, float b, float a)
    {
        var value = new MockValue { Type = 12 };
        value.Colors[0] = r;
        value.Colors[1] = g;
        value.Colors[2] = b;
        value.Colors[3] = a;
        return Register(value);
    }

    [UnmanagedCallersOnly]
    private static nint ValueCreateVector2D(double x, double y)
    {
        var value = new MockValue { Type = 13 };
        value.Doubles[0] = x;
        value.Doubles[1] = y;
        return Register(value);
    }

    [UnmanagedCallersOnly]
    private static nint ValueCreateEntity(ulong classId, nint instance, uint networkId) => Register(new MockValue { Type = 4 });

    [UnmanagedCallersOnly]
    private static nint ValueCreateFunction(long id, nint environment)
    {
        CreatedFunctions.Add(id);
        return Register(new MockValue { Type = 9, Integer = id });
    }

    [UnmanagedCallersOnly]
    private static nint EnvironmentCreate(byte* name, int length) => 0xE117;

    [UnmanagedCallersOnly]
    private static void EnvironmentDestroy(byte* name, int length)
    {
    }

    [UnmanagedCallersOnly]
    private static nint ValueCreateArray(nint* items, int count)
    {
        var value = new MockValue { Type = 5, Items = new List<nint>() };
        for (var i = 0; i < count; i++)
        {
            value.Items.Add(CloneHandle(items[i]));
        }

        return Register(value);
    }

    [UnmanagedCallersOnly]
    private static nint ValueCreateMap(nint* keys, nint* values, int count)
    {
        var value = new MockValue { Type = 6, Keys = new List<nint>(), Items = new List<nint>() };
        for (var i = 0; i < count; i++)
        {
            value.Keys.Add(CloneHandle(keys[i]));
            value.Items.Add(CloneHandle(values[i]));
        }

        return Register(value);
    }

    [UnmanagedCallersOnly]
    private static nint ValueClone(nint handle) => CloneHandle(handle);

    [UnmanagedCallersOnly]
    private static void ValueFree(nint handle)
    {
        if (Values.Remove(handle, out var value) && value.TextPointer != 0)
        {
            NativeMemory.Free((void*)value.TextPointer);
        }
    }

    [UnmanagedCallersOnly]
    private static int ValueGetType(nint handle) => Resolve(handle).Type;

    [UnmanagedCallersOnly]
    private static byte ValueGetBool(nint handle) => Resolve(handle).Integer != 0 ? (byte)1 : (byte)0;

    [UnmanagedCallersOnly]
    private static long ValueGetInteger(nint handle) => Resolve(handle).Integer;

    [UnmanagedCallersOnly]
    private static double ValueGetDouble(nint handle) => Resolve(handle).Number;

    [UnmanagedCallersOnly]
    private static byte* ValueGetString(nint handle, int* length)
    {
        var value = Resolve(handle);
        *length = value.TextLength;
        return (byte*)value.TextPointer;
    }

    [UnmanagedCallersOnly]
    private static void ValueGetDoubles(nint handle, double* output)
    {
        var value = Resolve(handle);
        output[0] = value.Doubles[0];
        output[1] = value.Doubles[1];
        output[2] = value.Doubles[2];
    }

    [UnmanagedCallersOnly]
    private static void ValueGetColor(nint handle, float* output)
    {
        var value = Resolve(handle);
        output[0] = value.Colors[0];
        output[1] = value.Colors[1];
        output[2] = value.Colors[2];
        output[3] = value.Colors[3];
    }

    [UnmanagedCallersOnly]
    private static int ValueGetLength(nint handle) => Resolve(handle).Items?.Count ?? 0;

    [UnmanagedCallersOnly]
    private static nint ValueGetItem(nint handle, int index) => Resolve(handle).Items![index];

    [UnmanagedCallersOnly]
    private static void ValueGetEntries(nint handle, nint* keys, nint* values)
    {
        var value = Resolve(handle);
        if (value.Keys == null || value.Items == null)
        {
            return;
        }

        for (var i = 0; i < value.Keys.Count; i++)
        {
            keys[i] = value.Keys[i];
            values[i] = value.Items[i];
        }
    }

    [UnmanagedCallersOnly]
    private static byte PropertyGet(nint property, nint self, nint* result, byte** error)
    {
        *result = Register(new MockValue { Type = 1, Integer = 150 });
        return 1;
    }

    [UnmanagedCallersOnly]
    private static byte PropertySet(nint property, nint self, nint value, byte** error)
    {
        var mock = Resolve(value);
        Console.WriteLine($"[host] property set: type={mock.Type} integer={mock.Integer}");
        return 1;
    }
}
