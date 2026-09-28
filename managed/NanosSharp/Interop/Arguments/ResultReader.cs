using System.Runtime.CompilerServices;
using System.Text;
using NanosSharp.Entities.Binding;
using NanosSharp.Interop.Values;
using NanosSharp.Mathematics;
using ValueType = NanosSharp.Interop.Values.ValueType;

namespace NanosSharp.Interop.Arguments;

internal static unsafe class ResultReader
{
    public static object? Read(ref NativeArgument result) => (ValueType)result.Type switch
    {
        _ when result.Type == NativeArgument.HandleType => ValueMarshaller.TakeFromNative(result.Handle),
        _ when result.Type == NativeArgument.BorrowedHandleType => ValueMarshaller.FromNative(result.Handle),
        ValueType.Bool => result.Boolean != 0,
        ValueType.Integer => result.Integer,
        ValueType.Double => result.Number,
        ValueType.String => ReadString(ref result),
        ValueType.Entity => EntityRegistry.Resolve(result.ClassId, result.Instance),
        ValueType.Vector => new Vector(result.X, result.Y, result.Z),
        ValueType.Rotator => new Rotator(result.X, result.Y, result.Z),
        ValueType.Color => new Color(result.R, result.G, result.B, result.A),
        ValueType.Vector2D => new Vector2D(result.X, result.Y),
        _ => null,
    };

    public static T? Read<T>(ref NativeArgument result)
    {
        var type = (ValueType)result.Type;
        if (typeof(T) == typeof(int) && type == ValueType.Integer)
        {
            var value = (int)result.Integer;
            return Unsafe.As<int, T>(ref value);
        }

        if (typeof(T) == typeof(uint) && type == ValueType.Integer)
        {
            var value = (uint)result.Integer;
            return Unsafe.As<uint, T>(ref value);
        }

        if (typeof(T) == typeof(float) && type == ValueType.Double)
        {
            var value = (float)result.Number;
            return Unsafe.As<float, T>(ref value);
        }

        if (typeof(T) == typeof(double) && type == ValueType.Double)
        {
            return Unsafe.As<double, T>(ref result.Number);
        }

        if (typeof(T) == typeof(bool) && type == ValueType.Bool)
        {
            var value = result.Boolean != 0;
            return Unsafe.As<bool, T>(ref value);
        }

        if (typeof(T) == typeof(Vector) && type == ValueType.Vector)
        {
            var value = new Vector(result.X, result.Y, result.Z);
            return Unsafe.As<Vector, T>(ref value);
        }

        if (typeof(T) == typeof(Rotator) && type == ValueType.Rotator)
        {
            var value = new Rotator(result.X, result.Y, result.Z);
            return Unsafe.As<Rotator, T>(ref value);
        }

        return ValueConverter.Convert<T>(Read(ref result));
    }

    public static object?[] ReadAll(NativeArgument* arguments, int count)
    {
        var values = new object?[count];
        for (var index = 0; index < count; index++)
        {
            values[index] = Read(ref arguments[index]);
        }

        return values;
    }

    private static string ReadString(ref NativeArgument result) =>
        result.StringData == 0 ? string.Empty : Encoding.UTF8.GetString((byte*)result.StringData, (int)result.StringLength);
}
