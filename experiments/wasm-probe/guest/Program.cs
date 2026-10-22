using System.Reflection;
using System.Runtime.InteropServices;

namespace NanosSharp.Wasm.Probe.Guest;

/// <summary>
/// Stand-in for the future client runtime.
///
/// It now exercises both directions of the boundary over host-supplied imports,
/// mirroring the shape of <c>NativeApi</c> (NanosSharp/Interop/NativeApi.cs):
/// <c>nanos.log</c>, <c>nanos.value_create_integer</c> and
/// <c>nanos.value_get_integer</c>. Reflection, attribute scanning and
/// constructor-injection DI run inside the sandbox, as question 1 established.
///
/// The imports force a native relink (wasi-sdk), so building this needs
/// <c>WASI_SDK_PATH</c>. The host that supplies the imports is the component
/// host under <c>host-rs/</c>.
/// </summary>
internal static unsafe partial class Program
{
    [LibraryImport("nanos", EntryPoint = "log")]
    [WasmImportLinkage]
    private static partial void HostLog(byte* message, int length);

    [LibraryImport("nanos", EntryPoint = "value-create-integer")]
    [WasmImportLinkage]
    private static partial int HostValueCreateInteger(long value);

    [LibraryImport("nanos", EntryPoint = "value-get-integer")]
    [WasmImportLinkage]
    private static partial long HostValueGetInteger(int handle);

    private static int Main(string[] args)
    {
        if (args.Length >= 2 && args[0] == "chatty")
        {
            var calls = long.Parse(args[1]);
            var (result, elapsed) = Boundary.ChattyBench(calls);
            Log($"result={result} elapsed_ms={elapsed:F1}");
            return 0;
        }

        if (args.Length >= 3 && args[0] == "batch")
        {
            var perBatch = int.Parse(args[1]);
            var batches = long.Parse(args[2]);
            var (crossings, elapsed) = Boundary.BatchBench(perBatch, batches);
            Log($"crossings={crossings} values={crossings * perBatch} elapsed_ms={elapsed:F1}");
            return 0;
        }

        Log("guest runtime entered");

        var handle = HostValueCreateInteger(1337);
        Log($"host value round trip: handle {handle} -> {HostValueGetInteger(handle)}");

        Log($"function-call sum(40, 2) = {Boundary.Call(1, 40, 2)}");
        Log($"function-call string bytes(\"hello \", \"wasm\") = {Boundary.Call(2, "hello ", "wasm")}");

        var services = new MiniContainer();
        services.Register<IGreeter, Greeter>();
        services.Register<EventSink, EventSink>();

        var discovered = ScanForHooks(typeof(Program).Assembly);
        Log($"reflection discovered {discovered.Count} hook method(s)");

        var greeter = services.Resolve<IGreeter>();
        Log(greeter.Greet("nanos"));

        foreach (var hook in discovered)
        {
            var target = hook.IsStatic ? null : services.Resolve(hook.DeclaringType!);
            hook.Invoke(target, null);
        }

        var handled = Boundary.PumpEvents(id => Log($"handled event {id}"));
        Log($"pumped {handled} host event(s)");

        Log("guest runtime finished");
        return 0;
    }

    private static List<MethodInfo> ScanForHooks(Assembly assembly) =>
        assembly.GetTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                BindingFlags.Static | BindingFlags.Instance))
            .Where(method => method.GetCustomAttribute<EventHookAttribute>() is not null)
            .ToList();

    private static void Log(string message)
    {
        var length = System.Text.Encoding.UTF8.GetByteCount(message);
        Span<byte> buffer = length <= 512 ? stackalloc byte[length] : new byte[length];
        System.Text.Encoding.UTF8.GetBytes(message, buffer);
        fixed (byte* pointer = buffer)
        {
            HostLog(pointer, length);
        }
    }

    [EventHook]
    private static void OnStaticTick() => Log("static hook fired");
}

[AttributeUsage(AttributeTargets.Method)]
internal sealed class EventHookAttribute : Attribute;

internal interface IGreeter
{
    string Greet(string name);
}

internal sealed class Greeter : IGreeter
{
    public string Greet(string name) => $"hello {name} from inside wasm";
}

internal sealed class EventSink
{
    [EventHook]
    public void OnInstanceTick() { }
}

/// <summary>
/// A deliberately tiny reflection-driven container. It exists only to prove that
/// activation by <see cref="Activator"/> and constructor injection still work
/// inside the wasm sandbox, which is what the real DI runtime relies on.
/// </summary>
internal sealed class MiniContainer
{
    private readonly Dictionary<Type, Type> _registrations = new();
    private readonly Dictionary<Type, object> _instances = new();

    public void Register<TService, TImplementation>() where TImplementation : TService =>
        _registrations[typeof(TService)] = typeof(TImplementation);

    public TService Resolve<TService>() => (TService)Resolve(typeof(TService));

    public object Resolve(Type service)
    {
        if (_instances.TryGetValue(service, out var existing))
        {
            return existing;
        }

        var implementation = _registrations.GetValueOrDefault(service, service);
        var constructor = implementation.GetConstructors().First();
        var arguments = constructor.GetParameters().Select(parameter => Resolve(parameter.ParameterType)).ToArray();
        var instance = Activator.CreateInstance(implementation, arguments)!;
        _instances[service] = instance;
        return instance;
    }
}
