using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NanosSharp.Interop;
using NanosSharp.Interop.Values;
using NanosSharp.Logging;
using NanosSharp.Packages;
using NanosSharp.Scripting;
using NanosSharp.Threading;

namespace NanosSharp.Core;

/// <summary>Global values shared with every package, the C# side of Package.Export in Lua.</summary>
public static unsafe class Exports
{
    private const char PathSeparator = '.';

    /// <summary>Makes a value available as a global in every package, delegates become callable functions.</summary>
    public static void Set(string name, object? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        MainThread.Ensure();
        SetGlobal(name, Wrap(value));
        if (PackageContext.Current is { } scope)
        {
            scope.Exports.Add(name);
        }
    }

    /// <summary>Reads a global value, a path like Library.Function reads a field of an exported table.</summary>
    public static object? Get(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        MainThread.Ensure();
        var segments = path.Split(PathSeparator);
        var value = GetGlobal(segments[0]);
        foreach (var key in segments.Skip(1))
        {
            value = value is IDictionary table ? table[key] : null;
        }

        return value;
    }

    /// <summary>Reads a global value and converts it.</summary>
    public static T? Get<T>(string path) => ValueConverter.Convert<T>(Get(path));

    /// <summary>Calls a global function, a path like Library.Function calls a function of an exported table.</summary>
    public static object? Call(string path, params ReadOnlySpan<object?> arguments) =>
        Get(path) is ScriptFunction function
            ? function.Invoke(arguments)
            : throw new ScriptException($"The global '{path}' is not a function.");

    /// <summary>Calls a global function and converts its first result.</summary>
    public static T? Call<T>(string path, params ReadOnlySpan<object?> arguments) =>
        ValueConverter.Convert<T>(Call(path, arguments));

    internal static void Register(Type type)
    {
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (method.GetCustomAttribute<ExportAttribute>() is { } methodExport)
            {
                Set(methodExport.Name ?? method.Name, CreateDelegate(method));
            }
        }

        if (type.GetCustomAttribute<ExportAttribute>() is not { } typeExport)
        {
            return;
        }

        var table = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .GroupBy(method => method.Name)
            .ToDictionary(group => (object)group.Key, group => (object?)CreateDelegate(SelectOverload(type, group)));
        Set(typeExport.Name ?? type.Name, table);
    }

    internal static void ReleaseScope(PackageScope scope)
    {
        if (NativeRuntime.Api.EngineIsAttached() == 0)
        {
            scope.Exports.Clear();
            return;
        }

        foreach (var name in scope.Exports)
        {
            try
            {
                SetGlobal(name, null);
            }
            catch (ScriptException exception)
            {
                Log.Exception(exception, $"Removing the export {name} failed");
            }
        }

        scope.Exports.Clear();
    }

    private static MethodInfo SelectOverload(Type type, IGrouping<string, MethodInfo> overloads)
    {
        var selected = overloads.MaxBy(method => method.GetParameters().Length)!;
        if (overloads.Count() > 1)
        {
            Log.Warning($"{type.Name}.{overloads.Key} has overloads, only the one with the most parameters is exported.");
        }

        return selected;
    }

    private static Delegate CreateDelegate(MethodInfo method)
    {
        var types = method.GetParameters().Select(parameter => parameter.ParameterType).Append(method.ReturnType);
        return method.CreateDelegate(Expression.GetDelegateType([.. types]));
    }

    private static object? Wrap(object? value) => value switch
    {
        Delegate handler => ScriptCallback.From(handler),
        IDictionary dictionary => WrapDictionary(dictionary),
        _ => value,
    };

    private static Dictionary<object, object?> WrapDictionary(IDictionary dictionary)
    {
        var wrapped = new Dictionary<object, object?>(dictionary.Count);
        var enumerator = dictionary.GetEnumerator();
        while (enumerator.MoveNext())
        {
            wrapped[enumerator.Key] = Wrap(enumerator.Value);
        }

        return wrapped;
    }

    private static object? GetGlobal(string name)
    {
        var bytes = Encoding.UTF8.GetBytes(name);
        byte* error = null;
        nint value;
        fixed (byte* pointer = bytes)
        {
            value = NativeRuntime.Api.LuaGetGlobal(PackageContext.Environment, pointer, bytes.Length, &error);
        }

        NativeRuntime.ThrowIfFailed(value != 0 ? (byte)1 : (byte)0, error);
        return ValueMarshaller.TakeFromNative(value);
    }

    private static void SetGlobal(string name, object? value)
    {
        var bytes = Encoding.UTF8.GetBytes(name);
        var handle = ValueMarshaller.ToNative(value);
        try
        {
            byte* error = null;
            byte succeeded;
            fixed (byte* pointer = bytes)
            {
                succeeded = NativeRuntime.Api.LuaSetGlobal(PackageContext.Environment, pointer, bytes.Length, handle, &error);
            }

            NativeRuntime.ThrowIfFailed(succeeded, error);
        }
        finally
        {
            NativeRuntime.Api.ValueFree(handle);
        }
    }
}
