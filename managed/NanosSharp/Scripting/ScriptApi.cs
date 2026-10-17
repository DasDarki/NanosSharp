using System.Text.Encodings.Web;
using System.Text.Json;
using NanosSharp.Entities;
using NanosSharp.Interop;
using NanosSharp.Interop.Reflection;
using NanosSharp.Interop.Values;
using NanosSharp.Packages;
using NanosSharp.Threading;

namespace NanosSharp.Scripting;

/// <summary>Dynamic access to every scripting class and function exposed by nanos world.</summary>
public static partial class ScriptApi
{
    /// <summary>Whether the scripting engine is currently running.</summary>
    public static bool IsAvailable => NativeRuntime.IsEngineAttached;

    /// <summary>Names of all scripting classes.</summary>
    public static IEnumerable<string> ClassNames => NativeClassRegistry.Classes.Select(nativeClass => nativeClass.Name);

    /// <summary>Calls a static function of a scripting class, for example Server.GetMap.</summary>
    public static object? Invoke(string className, string function, params ReadOnlySpan<object?> arguments) =>
        NativeClassRegistry.Get(className).GetStaticMethod(function).Invoke(PackageContext.Environment, null, arguments);

    /// <summary>Calls a static function of a scripting class and converts its result.</summary>
    public static T? Invoke<T>(string className, string function, params ReadOnlySpan<object?> arguments) =>
        NativeClassRegistry.Get(className).GetStaticMethod(function).Invoke<T>(PackageContext.Environment, null, arguments);

    /// <summary>Spawns an entity of a scripting class, like calling the class in Lua.</summary>
    public static Entity Spawn(string className, params ReadOnlySpan<object?> arguments)
    {
        var constructor = NativeClassRegistry.Get(className).Constructor ??
                          throw new ScriptException($"{className} cannot be spawned.");
        return constructor.Invoke(PackageContext.Environment, null, arguments) as Entity ??
               throw new ScriptException($"{className} did not spawn an entity.");
    }

    internal static void Invoke(StaticMethodSlot function, params ReadOnlySpan<object?> arguments) =>
        function.Resolve().Invoke(PackageContext.Environment, null, arguments);

    internal static T? Invoke<T>(StaticMethodSlot function, params ReadOnlySpan<object?> arguments) =>
        function.Resolve().Invoke<T>(PackageContext.Environment, null, arguments);

    internal static void InvokeVariadic(StaticMethodSlot function, ReadOnlySpan<object?> arguments, ReadOnlySpan<object?> variadic) =>
        function.Resolve().Invoke(PackageContext.Environment, null, arguments, variadic);

    internal static T? InvokeVariadic<T>(StaticMethodSlot function, ReadOnlySpan<object?> arguments, ReadOnlySpan<object?> variadic) =>
        function.Resolve().Invoke<T>(PackageContext.Environment, null, arguments, variadic);

    /// <summary>Calls a global scripting function.</summary>
    public static object? InvokeGlobal(string function, params ReadOnlySpan<object?> arguments) =>
        NativeClassRegistry.GetFunction(function).Invoke(PackageContext.Environment, null, arguments);

    /// <summary>Subscribes to a static event of a scripting class, the handler parameters are converted automatically.</summary>
    public static ScriptSubscription Subscribe(string className, string eventName, Delegate handler) =>
        ScriptSubscription.Create(handler,
            function => Invoke(className, "Subscribe", eventName, function),
            function => Invoke(className, "Unsubscribe", eventName, function));

    /// <summary>Writes every scripting class with its functions and native signatures as deterministic JSON.</summary>
    public static void DumpApi(string path)
    {
        MainThread.Ensure();
        var classes = NativeClassRegistry.Classes.OrderBy(nativeClass => nativeClass.Name, StringComparer.Ordinal).Select(nativeClass => new
        {
            nativeClass.Name,
            Id = nativeClass.Id.ToString("x16"),
            Constructor = nativeClass.Constructor?.Signature,
            Methods = DescribeMethods(nativeClass.Methods.Values),
            StaticMethods = DescribeMethods(nativeClass.StaticMethods.Values),
            Properties = nativeClass.Properties.Values
                .OrderBy(property => property.Name, StringComparer.Ordinal)
                .Select(property => new { property.Name, property.Signature }),
        });
        var document = new
        {
            ServerVersion = ReadVersion(),
            Classes = classes,
            Functions = DescribeMethods(NativeClassRegistry.Functions),
        };
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            NewLine = "\n",
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        System.IO.File.WriteAllText(path, JsonSerializer.Serialize(document, options) + "\n");
    }

    private static string ReadVersion()
    {
        var source = NativeClassRegistry.Classes
            .Where(nativeClass => nativeClass.Name is "Server" or "Client")
            .FirstOrDefault(nativeClass => nativeClass.StaticMethods.ContainsKey("GetVersion"));
        return source is null ? "unknown" : Invoke<string>(source.Name, "GetVersion") ?? "unknown";
    }

    private static IEnumerable<object> DescribeMethods(IEnumerable<NativeMethod> methods) =>
        methods.OrderBy(method => method.Name, StringComparer.Ordinal).Select(method => new { method.Name, method.Signature });
}
