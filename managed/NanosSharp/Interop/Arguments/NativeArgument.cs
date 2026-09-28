using System.Runtime.InteropServices;

namespace NanosSharp.Interop.Arguments;

[StructLayout(LayoutKind.Explicit, Size = 32)]
internal struct NativeArgument
{
    public const byte HandleType = 0xff;
    public const byte BorrowedHandleType = 0xfe;

    [FieldOffset(0)] public byte Type;
    [FieldOffset(8)] public byte Boolean;
    [FieldOffset(8)] public long Integer;
    [FieldOffset(8)] public double Number;
    [FieldOffset(8)] public nint StringData;
    [FieldOffset(16)] public long StringLength;
    [FieldOffset(8)] public double X;
    [FieldOffset(16)] public double Y;
    [FieldOffset(24)] public double Z;
    [FieldOffset(8)] public float R;
    [FieldOffset(12)] public float G;
    [FieldOffset(16)] public float B;
    [FieldOffset(20)] public float A;
    [FieldOffset(8)] public ulong ClassId;
    [FieldOffset(16)] public nint Instance;
    [FieldOffset(8)] public nint Handle;
}
