namespace NanosSharp.Runtime.Native;

internal enum ICValueType : byte
{
    Null,
    Boolean,
    Integer,
    Double,
    String,
    Pointer,
    Array, // The array can contain any IC value type
}