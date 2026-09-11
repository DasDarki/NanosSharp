namespace NanosSharp.Interop.Callbacks;

internal readonly record struct NativePackage(nint Handle, string Name, string Directory, nint Environment);
