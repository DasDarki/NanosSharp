using System.Reflection;
using NanosSharp.Interop.Values;

namespace NanosSharp.Interop.Arguments;

internal static unsafe class ArgumentAccess
{
    public static T? Get<T>(nint arguments, int count, int index) =>
        index < count ? ResultReader.Read<T>(ref ((NativeArgument*)arguments)[index]) : default;

    public static object?[] GetAll(nint arguments, int count) => ResultReader.ReadAll((NativeArgument*)arguments, count);

    public static object? GetValue(ParameterInfo parameter, nint arguments, int count, int index)
    {
        if (index < count)
        {
            return ValueConverter.Convert(ResultReader.Read(ref ((NativeArgument*)arguments)[index]), parameter.ParameterType);
        }

        if (parameter.HasDefaultValue)
        {
            return parameter.DefaultValue;
        }

        var type = parameter.ParameterType;
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
