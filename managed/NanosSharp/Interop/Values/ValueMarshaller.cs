using System.Collections;
using System.Text;
using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Interop.Values;

internal static unsafe class ValueMarshaller
{
    public const int NativeValueSize = 32;

    public static nint ToNative(object? value)
    {
        ref readonly var api = ref NativeRuntime.Api;
        return value switch
        {
            null => api.ValueCreateNil(),
            bool boolean => api.ValueCreateBool(boolean ? (byte)1 : (byte)0),
            string text => CreateString(text),
            char character => CreateString(character.ToString()),
            Enum enumeration => api.ValueCreateInteger(Convert.ToInt64(enumeration)),
            sbyte or byte or short or ushort or int or uint or long => api.ValueCreateInteger(Convert.ToInt64(value)),
            ulong unsigned => api.ValueCreateInteger(unchecked((long)unsigned)),
            float or double or decimal => api.ValueCreateDouble(Convert.ToDouble(value)),
            Vector vector => api.ValueCreateVector(vector.X, vector.Y, vector.Z),
            Rotator rotator => api.ValueCreateRotator(rotator.Pitch, rotator.Yaw, rotator.Roll),
            Color color => api.ValueCreateColor(color.R, color.G, color.B, color.A),
            Vector2D vector => api.ValueCreateVector2D(vector.X, vector.Y),
            Entity entity => api.ValueCreateEntity(entity.ClassId, entity.EnsureValid()),
            ScriptFunction function => api.ValueClone(function.Handle),
            ScriptOpaque opaque => api.ValueClone(opaque.Handle),
            Delegate => throw new ArgumentException(
                "Delegates cannot be passed as plain values because Lua packages cannot call them, use a Subscribe method instead."),
            IDictionary dictionary => CreateMap(dictionary),
            IEnumerable sequence => CreateArray(sequence),
            _ => throw new ArgumentException($"Values of type {value.GetType()} cannot be passed to nanos world."),
        };
    }

    public static object? FromNative(nint value) => (ValueType)NativeRuntime.Api.ValueGetType(value) switch
    {
        ValueType.Bool => NativeRuntime.Api.ValueGetBool(value) != 0,
        ValueType.Integer => NativeRuntime.Api.ValueGetInteger(value),
        ValueType.Double => NativeRuntime.Api.ValueGetDouble(value),
        ValueType.String => ReadString(value),
        ValueType.Entity => ReadEntity(value),
        ValueType.Array or ValueType.Tuple => ReadArray(value),
        ValueType.Map => ReadMap(value),
        ValueType.Opaque or ValueType.WeakTable => new ScriptOpaque(NativeRuntime.Api.ValueClone(value)),
        ValueType.Function => new ScriptFunction(NativeRuntime.Api.ValueClone(value)),
        ValueType.Vector => ReadVector(value),
        ValueType.Rotator => ReadRotator(value),
        ValueType.Color => ReadColor(value),
        ValueType.Vector2D => ReadVector2D(value),
        ValueType.EntityDeferred => EntityRegistry.ResolveDeferred(NativeRuntime.Api.ValueGetNetworkId(value)),
        _ => null,
    };

    public static object? TakeFromNative(nint value)
    {
        try
        {
            return FromNative(value);
        }
        finally
        {
            NativeRuntime.Api.ValueFree(value);
        }
    }

    public static object?[] ReadArguments(nint arguments, int count)
    {
        var values = new object?[count];
        for (var index = 0; index < count; index++)
        {
            values[index] = FromNative(arguments + index * NativeValueSize);
        }

        return values;
    }

    private static nint CreateString(string text)
    {
        var length = Encoding.UTF8.GetByteCount(text);
        Span<byte> buffer = length <= 1024 ? stackalloc byte[length] : new byte[length];
        Encoding.UTF8.GetBytes(text, buffer);
        fixed (byte* pointer = buffer)
        {
            return NativeRuntime.Api.ValueCreateString(pointer, length);
        }
    }

    private static nint CreateArray(IEnumerable sequence)
    {
        using var items = new NativeValueList(sequence.Cast<object?>());
        return NativeRuntime.Api.ValueCreateArray(items.Pointer, items.Count);
    }

    private static nint CreateMap(IDictionary dictionary)
    {
        var entries = dictionary.Cast<DictionaryEntry>().ToArray();
        using var keys = new NativeValueList(entries.Select(entry => entry.Key));
        using var values = new NativeValueList(entries.Select(entry => entry.Value));
        return NativeRuntime.Api.ValueCreateMap(keys.Pointer, values.Pointer, keys.Count);
    }

    private static string ReadString(nint value)
    {
        int length;
        var text = NativeRuntime.Api.ValueGetString(value, &length);
        return text == null ? string.Empty : Encoding.UTF8.GetString(text, length);
    }

    private static Entity? ReadEntity(nint value)
    {
        ulong classId;
        nint instance;
        NativeRuntime.Api.ValueGetEntity(value, &classId, &instance);
        return EntityRegistry.Resolve(classId, instance);
    }

    private static Vector ReadVector(nint value)
    {
        var buffer = stackalloc double[3];
        NativeRuntime.Api.ValueGetDoubles(value, buffer);
        return new Vector(buffer[0], buffer[1], buffer[2]);
    }

    private static Rotator ReadRotator(nint value)
    {
        var buffer = stackalloc double[3];
        NativeRuntime.Api.ValueGetDoubles(value, buffer);
        return new Rotator(buffer[0], buffer[1], buffer[2]);
    }

    private static Color ReadColor(nint value)
    {
        var buffer = stackalloc float[4];
        NativeRuntime.Api.ValueGetColor(value, buffer);
        return new Color(buffer[0], buffer[1], buffer[2], buffer[3]);
    }

    private static Vector2D ReadVector2D(nint value)
    {
        var buffer = stackalloc double[2];
        NativeRuntime.Api.ValueGetDoubles(value, buffer);
        return new Vector2D(buffer[0], buffer[1]);
    }

    private static object?[] ReadArray(nint value)
    {
        ref readonly var api = ref NativeRuntime.Api;
        var items = new object?[api.ValueGetLength(value)];
        for (var index = 0; index < items.Length; index++)
        {
            items[index] = FromNative(api.ValueGetItem(value, index));
        }

        return items;
    }

    private static Dictionary<object, object?> ReadMap(nint value)
    {
        ref readonly var api = ref NativeRuntime.Api;
        var count = api.ValueGetLength(value);
        var keys = new nint[count];
        var values = new nint[count];
        fixed (nint* keyPointer = keys, valuePointer = values)
        {
            api.ValueGetEntries(value, keyPointer, valuePointer);
        }

        var map = new Dictionary<object, object?>(count);
        for (var index = 0; index < count; index++)
        {
            if (FromNative(keys[index]) is { } key)
            {
                map[key] = FromNative(values[index]);
            }
        }

        return map;
    }
}
