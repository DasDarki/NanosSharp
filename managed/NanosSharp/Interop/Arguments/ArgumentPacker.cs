using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NanosSharp.Entities;
using NanosSharp.Interop.Values;
using NanosSharp.Mathematics;
using ValueType = NanosSharp.Interop.Values.ValueType;

namespace NanosSharp.Interop.Arguments;

internal unsafe ref struct ArgumentPacker
{
    public readonly int Count => _count;

    private readonly Span<NativeArgument> _arguments;
    private readonly byte* _scratch;
    private readonly int _scratchLength;
    private int _scratchOffset;
    private List<GCHandle>? _pinned;
    private List<nint>? _handles;
    private int _count;

    public ArgumentPacker(Span<NativeArgument> arguments, Span<byte> scratch)
    {
        _arguments = arguments;
        _scratch = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(scratch));
        _scratchLength = scratch.Length;
    }

    public void AddSelf(Entity entity) => AddEntity(entity, entity.NetworkId);

    public void Add(object? value)
    {
        switch (value)
        {
            case null:
                AddNil();
                break;
            case bool boolean:
                AddBoolean(boolean);
                break;
            case int or long or uint or short or ushort or byte or sbyte or Enum:
                AddInteger(Convert.ToInt64(value));
                break;
            case float or double:
                AddNumber(Convert.ToDouble(value));
                break;
            case string text:
                AddString(text);
                break;
            case Entity entity:
                AddEntity(entity, entity.ResolveNetworkId());
                break;
            case Vector vector:
                AddVector(vector);
                break;
            case Rotator rotator:
                AddRotator(rotator);
                break;
            case Color color:
                AddColor(color);
                break;
            case Vector2D vector:
                AddVector2D(vector);
                break;
            default:
                AddHandle(value);
                break;
        }
    }

    public void Add<T>(T value)
    {
        if (typeof(T) == typeof(int))
        {
            AddInteger(Unsafe.As<T, int>(ref value));
        }
        else if (typeof(T) == typeof(uint))
        {
            AddInteger(Unsafe.As<T, uint>(ref value));
        }
        else if (typeof(T) == typeof(long))
        {
            AddInteger(Unsafe.As<T, long>(ref value));
        }
        else if (typeof(T) == typeof(ulong))
        {
            AddInteger(unchecked((long)Unsafe.As<T, ulong>(ref value)));
        }
        else if (typeof(T) == typeof(float))
        {
            AddNumber(Unsafe.As<T, float>(ref value));
        }
        else if (typeof(T) == typeof(double))
        {
            AddNumber(Unsafe.As<T, double>(ref value));
        }
        else if (typeof(T) == typeof(bool))
        {
            AddBoolean(Unsafe.As<T, bool>(ref value));
        }
        else if (typeof(T) == typeof(Vector))
        {
            AddVector(Unsafe.As<T, Vector>(ref value));
        }
        else if (typeof(T) == typeof(Rotator))
        {
            AddRotator(Unsafe.As<T, Rotator>(ref value));
        }
        else if (typeof(T) == typeof(Color))
        {
            AddColor(Unsafe.As<T, Color>(ref value));
        }
        else if (typeof(T) == typeof(Vector2D))
        {
            AddVector2D(Unsafe.As<T, Vector2D>(ref value));
        }
        else if (typeof(T).IsValueType)
        {
            AddNullable(value);
        }
        else
        {
            Add((object?)value);
        }
    }

    public void TrimTrailingNil(int minimumCount)
    {
        while (_count > minimumCount && _arguments[_count - 1].Type == (byte)ValueType.Nil)
        {
            _count--;
        }
    }

    public void Dispose()
    {
        FreePinned(_pinned);
        FreeHandles(_handles);
        _pinned = null;
        _handles = null;
    }

    private void AddNullable<T>(T value)
    {
        if (typeof(T) == typeof(float?))
        {
            AddOptionalNumber(Unsafe.As<T, float?>(ref value));
        }
        else if (typeof(T) == typeof(int?))
        {
            AddOptionalInteger(Unsafe.As<T, int?>(ref value));
        }
        else if (typeof(T) == typeof(uint?))
        {
            AddOptionalInteger(Unsafe.As<T, uint?>(ref value));
        }
        else if (typeof(T) == typeof(bool?))
        {
            AddOptionalBoolean(Unsafe.As<T, bool?>(ref value));
        }
        else if (typeof(T) == typeof(double?))
        {
            AddOptionalNumber(Unsafe.As<T, double?>(ref value));
        }
        else
        {
            Add((object?)value);
        }
    }

    private void AddOptionalNumber(double? value)
    {
        if (value is { } number)
        {
            AddNumber(number);
            return;
        }

        AddNil();
    }

    private void AddOptionalInteger(long? value)
    {
        if (value is { } integer)
        {
            AddInteger(integer);
            return;
        }

        AddNil();
    }

    private void AddOptionalBoolean(bool? value)
    {
        if (value is { } boolean)
        {
            AddBoolean(boolean);
            return;
        }

        AddNil();
    }

    private ref NativeArgument Next(ValueType type)
    {
        ref var argument = ref _arguments[_count++];
        argument.Type = (byte)type;
        return ref argument;
    }

    private void AddNil() => Next(ValueType.Nil);

    private void AddBoolean(bool value) => Next(ValueType.Bool).Boolean = value ? (byte)1 : (byte)0;

    private void AddInteger(long value) => Next(ValueType.Integer).Integer = value;

    private void AddNumber(double value) => Next(ValueType.Double).Number = value;

    private void AddVector(Vector vector)
    {
        ref var argument = ref Next(ValueType.Vector);
        (argument.X, argument.Y, argument.Z) = (vector.X, vector.Y, vector.Z);
    }

    private void AddRotator(Rotator rotator)
    {
        ref var argument = ref Next(ValueType.Rotator);
        (argument.X, argument.Y, argument.Z) = (rotator.Pitch, rotator.Yaw, rotator.Roll);
    }

    private void AddColor(Color color)
    {
        ref var argument = ref Next(ValueType.Color);
        (argument.R, argument.G, argument.B, argument.A) = (color.R, color.G, color.B, color.A);
    }

    private void AddVector2D(Vector2D vector)
    {
        ref var argument = ref Next(ValueType.Vector2D);
        (argument.X, argument.Y) = (vector.X, vector.Y);
    }

    private void AddEntity(Entity entity, uint networkId)
    {
        ref var argument = ref Next(ValueType.Entity);
        argument.Instance = entity.EnsureValid();
        argument.ClassId = entity.ClassId;
        argument.NetworkId = networkId;
    }

    private void AddString(string text)
    {
        var maximumLength = (text.Length + 1) * 3;
        var fitsScratch = _scratchLength - _scratchOffset >= maximumLength;
        var target = fitsScratch ? _scratch + _scratchOffset : PinBuffer(maximumLength);
        var length = WriteUtf8(text, new Span<byte>(target, maximumLength));
        if (fitsScratch)
        {
            _scratchOffset += length;
        }

        ref var argument = ref Next(ValueType.String);
        argument.StringData = (nint)target;
        argument.StringLength = length;
    }

    private static int WriteUtf8(string text, Span<byte> destination) =>
        Ascii.FromUtf16(text, destination, out var written) == OperationStatus.Done
            ? written
            : Encoding.UTF8.GetBytes(text, destination);

    private byte* PinBuffer(int length)
    {
        var handle = GCHandle.Alloc(new byte[length], GCHandleType.Pinned);
        (_pinned ??= []).Add(handle);
        return (byte*)handle.AddrOfPinnedObject();
    }

    private void AddHandle(object value)
    {
        var handle = ValueMarshaller.ToNative(value);
        (_handles ??= []).Add(handle);
        ref var argument = ref Next((ValueType)NativeArgument.HandleType);
        argument.Handle = handle;
    }

    private static void FreePinned(List<GCHandle>? pinned)
    {
        if (pinned is null)
        {
            return;
        }

        foreach (var handle in pinned)
        {
            handle.Free();
        }
    }

    private static void FreeHandles(List<nint>? handles)
    {
        if (handles is null)
        {
            return;
        }

        foreach (var handle in handles)
        {
            NativeRuntime.Api.ValueFree(handle);
        }
    }
}
