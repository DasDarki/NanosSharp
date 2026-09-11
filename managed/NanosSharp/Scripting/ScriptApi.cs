using System.Text.Json;
using NanosSharp.Interop;
using NanosSharp.Interop.Reflection;
using NanosSharp.Interop.Values;
using NanosSharp.Packages;
using NanosSharp.Threading;

namespace NanosSharp.Scripting;

/// <summary>Dynamic access to every scripting class and function exposed by nanos world.</summary>
public static class ScriptApi
{
    /// <summary>Whether the scripting engine is currently running.</summary>
    public static bool IsAvailable => NativeRuntime.IsEngineAttached;

    /// <summary>Names of all scripting classes.</summary>
    public static IEnumerable<string> ClassNames => NativeClassRegistry.Classes.Select(nativeClass => nativeClass.Name);

    /// <summary>Calls a static function of a scripting class, for example Server.GetMap.</summary>
    public static object? Invoke(string className, string function, params ReadOnlySpan<object?> arguments) =>
        NativeClassRegistry.Get(className).GetStaticMethod(function).Invoke(PackageContext.Environment, arguments);

    /// <summary>Calls a static function of a scripting class and converts its result.</summary>
    public static T? Invoke<T>(string className, string function, params ReadOnlySpan<object?> arguments) =>
        ValueConverter.Convert<T>(Invoke(className, function, arguments));

    /// <summary>Calls a global scripting function.</summary>
    public static object? InvokeGlobal(string function, params ReadOnlySpan<object?> arguments) =>
        NativeClassRegistry.GetFunction(function).Invoke(PackageContext.Environment, arguments);

    /// <summary>Subscribes to a static event of a scripting class, the handler parameters are converted automatically.</summary>
    public static ScriptSubscription Subscribe(string className, string eventName, Delegate handler) =>
        ScriptSubscription.Create(handler,
            function => Invoke(className, "Subscribe", eventName, function),
            function => Invoke(className, "Unsubscribe", eventName, function));

    /// <summary>Writes every scripting class with its functions and native signatures as JSON.</summary>
    public static void DumpApi(string path)
    {
        MainThread.Ensure();
        var classes = NativeClassRegistry.Classes.OrderBy(nativeClass => nativeClass.Name).Select(nativeClass => new
        {
            nativeClass.Name,
            Id = nativeClass.Id.ToString("x16"),
            Constructor = nativeClass.Constructor?.Signature,
            Methods = DescribeMethods(nativeClass.Methods.Values),
            StaticMethods = DescribeMethods(nativeClass.StaticMethods.Values),
            Properties = nativeClass.Properties.Values.OrderBy(property => property.Name)
                .Select(property => new { property.Name, property.Signature }),
        });
        var document = new { Classes = classes, Functions = DescribeMethods(NativeClassRegistry.Functions) };
        File.WriteAllText(path, JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static IEnumerable<object> DescribeMethods(IEnumerable<NativeMethod> methods) =>
        methods.OrderBy(method => method.Name).Select(method => new { method.Name, method.Signature });
}
