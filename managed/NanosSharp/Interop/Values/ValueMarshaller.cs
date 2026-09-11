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

    public static object? FromNative(nint value)
    {
        ref readonly var api = ref NativeRuntime.Api;
        switch ((ValueType)api.ValueGetType(value))
        {
            case ValueType.Bool:
                return api.ValueGetBool(value) != 0;
            case ValueType.Integer:
                return api.ValueGetInteger(value);
            case ValueType.Double:
                return api.ValueGetDouble(value);
            case ValueType.String:
                int length;
                var text = api.ValueGetString(value, &length);
                return text == null ? string.Empty : Encoding.UTF8.GetString(text, length);
            case ValueType.Entity:
                ulong classId;
                nint instance;
                api.ValueGetEntity(value, &classId, &instance);
                return EntityRegistry.Resolve(classId, instance);
            case ValueType.Array:
            case ValueType.Tuple:
                return ReadArray(value);
            case ValueType.Map:
                return ReadMap(value);
            case ValueType.Opaque:
            case ValueType.WeakTable:
                return new ScriptOpaque(api.ValueClone(value));
            case ValueType.Function:
                return new ScriptFunction(api.ValueClone(value));
            case ValueType.Vector:
                {
                    var buffer = stackalloc double[3];
                    api.ValueGetDoubles(value, buffer);
                    return new Vector(buffer[0], buffer[1], buffer[2]);
                }
            case ValueType.Rotator:
                {
                    var buffer = stackalloc double[3];
                    api.ValueGetDoubles(value, buffer);
                    return new Rotator(buffer[0], buffer[1], buffer[2]);
                }
            case ValueType.Color:
                {
                    var buffer = stackalloc float[4];
                    api.ValueGetColor(value, buffer);
                    return new Color(buffer[0], buffer[1], buffer[2], buffer[3]);
                }
            case ValueType.Vector2D:
                {
                    var buffer = stackalloc double[2];
                    api.ValueGetDoubles(value, buffer);
                    return new Vector2D(buffer[0], buffer[1]);
                }
            case ValueType.EntityDeferred:
                return EntityRegistry.ResolveDeferred(api.ValueGetNetworkId(value));
            default:
                return null;
        }
    }

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
