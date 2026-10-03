namespace NanosSharp.Interop.Arguments;

internal static unsafe class ArgumentAccess
{
    public static T? Get<T>(nint arguments, int count, int index) =>
        index < count ? ResultReader.Read<T>(ref ((NativeArgument*)arguments)[index]) : default;

    public static object?[] GetAll(nint arguments, int count) => ResultReader.ReadAll((NativeArgument*)arguments, count);
}
