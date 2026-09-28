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
    private readonly byte* _strings;
    private readonly int _stringCapacity;
    private byte[]? _rentedStrings;
    private GCHandle _rentedHandle;
    private int _stringOffset;
    private List<nint>? _handles;
    private int _count;

    public ArgumentPacker(Span<NativeArgument> arguments, ReadOnlySpan<object?> values, Span<byte> stackStrings)
    {
        _arguments = arguments;
        var stringBytes = 0;
        foreach (var value in values)
        {
            stringBytes += value is string text ? Encoding.UTF8.GetMaxByteCount(text.Length) : 0;
        }

        if (stringBytes <= stackStrings.Length)
        {
            _strings = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(stackStrings));
            _stringCapacity = stackStrings.Length;
            return;
        }

        _rentedStrings = ArrayPool<byte>.Shared.Rent(stringBytes);
        _rentedHandle = GCHandle.Alloc(_rentedStrings, GCHandleType.Pinned);
        _strings = (byte*)_rentedHandle.AddrOfPinnedObject();
        _stringCapacity = _rentedStrings.Length;
    }

    public void Add(object? value)
    {
        ref var argument = ref _arguments[_count++];
        argument = default;
        switch (value)
        {
            case null:
                argument.Type = (byte)ValueType.Nil;
                break;
            case bool boolean:
                argument.Type = (byte)ValueType.Bool;
                argument.Boolean = boolean ? (byte)1 : (byte)0;
                break;
            case int or long or uint or short or ushort or byte or sbyte or Enum:
                argument.Type = (byte)ValueType.Integer;
                argument.Integer = Convert.ToInt64(value);
                break;
            case float or double:
                argument.Type = (byte)ValueType.Double;
                argument.Number = Convert.ToDouble(value);
                break;
            case string text:
                AddString(ref argument, text);
                break;
            case Entity entity:
                argument.Type = (byte)ValueType.Entity;
                argument.Instance = entity.EnsureValid();
                argument.ClassId = entity.ClassId;
                break;
            case Vector vector:
                argument.Type = (byte)ValueType.Vector;
                (argument.X, argument.Y, argument.Z) = (vector.X, vector.Y, vector.Z);
                break;
            case Rotator rotator:
                argument.Type = (byte)ValueType.Rotator;
                (argument.X, argument.Y, argument.Z) = (rotator.Pitch, rotator.Yaw, rotator.Roll);
                break;
            case Color color:
                argument.Type = (byte)ValueType.Color;
                (argument.R, argument.G, argument.B, argument.A) = (color.R, color.G, color.B, color.A);
                break;
            case Vector2D vector:
                argument.Type = (byte)ValueType.Vector2D;
                (argument.X, argument.Y) = (vector.X, vector.Y);
                break;
            default:
                AddHandle(ref argument, value);
                break;
        }
    }

    public void Dispose()
    {
        if (_rentedStrings is not null)
        {
            _rentedHandle.Free();
            ArrayPool<byte>.Shared.Return(_rentedStrings);
            _rentedStrings = null;
        }

        foreach (var handle in _handles ?? [])
        {
            NativeRuntime.Api.ValueFree(handle);
        }

        _handles = null;
    }

    private void AddString(ref NativeArgument argument, string text)
    {
        var length = Encoding.UTF8.GetBytes(text, new Span<byte>(_strings + _stringOffset, _stringCapacity - _stringOffset));
        argument.Type = (byte)ValueType.String;
        argument.StringData = (nint)(_strings + _stringOffset);
        argument.StringLength = length;
        _stringOffset += length;
    }

    private void AddHandle(ref NativeArgument argument, object value)
    {
        var handle = ValueMarshaller.ToNative(value);
        (_handles ??= []).Add(handle);
        argument.Type = NativeArgument.HandleType;
        argument.Handle = handle;
    }
}
