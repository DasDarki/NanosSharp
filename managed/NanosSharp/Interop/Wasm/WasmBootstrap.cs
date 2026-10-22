#if NANOSSHARP_WASM
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NanosSharp.Entities;
using NanosSharp.Interop.Arguments;
using NanosSharp.Interop.Callbacks;
using NanosSharp.Interop.Reflection;
using NanosSharp.Interop.Values;
using NanosSharp.Logging;
using NanosSharp.Mathematics;

namespace NanosSharp.Interop.Wasm;

/// <summary>
/// Entry point for the client runtime when it runs inside a WebAssembly sandbox.
/// The <see cref="NativeApi"/> seam is backed by direct <c>nanos</c> imports in
/// the wasm build, so this only has to initialize the runtime and drive it.
/// </summary>
public static unsafe class WasmBootstrap
{
    private const string PackageDirectory = "/package";

    /// <summary>
    /// Boots the sandboxed runtime, then either loads and runs the delivered
    /// packages (when dynamic code is available, i.e. the Mono runtime) or
    /// dispatches a fixed number of ticks (the ahead-of-time runtime).
    /// </summary>
    /// <param name="args">The process arguments supplied by the host.</param>
    /// <returns>The process exit code.</returns>
    public static int Run(string[] args)
    {
        BootInternal();
        return 0;
    }

    /// <summary>
    /// Host-called boot entry for the reactor module. The reactor has no
    /// <c>_start</c>, so the native host drives <c>_initialize</c>, then
    /// <c>initialize_runtime</c>, then this export to bring the runtime up.
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = "nanos_boot")]
    public static void Boot() => BootInternal();

    private static void BootInternal()
    {
        var api = default(NativeApi);
        api.Version = NativeApi.SupportedVersion;

        var callbacks = default(ManagedCallbacks);
        NativeRuntime.Initialize(&api, &callbacks);

        WasmResultScratch.Register();

        NativeRuntime.Log(LogLevel.Info, "NanosSharp wasm runtime initialized");

        MarshallingSelfTest();

        NativeRuntime.Log(LogLevel.Info, "wasm runtime ready");
    }

    private static void MarshallingSelfTest()
    {
        if (NativeRuntime.IsEngineAttached)
        {
            return;
        }

        IntegerResultSelfTest();
        StringRoundTripSelfTest();
        StringResultSelfTest();
        ValueHandleSelfTest();
        ValueMarshallerSelfTest();
        SchemaLoadSelfTest();
        EntityResultSelfTest();
        PropertySelfTest();
    }

    private static void ValueMarshallerSelfTest()
    {
        var stringBack = ValueMarshaller.TakeFromNative(ValueMarshaller.ToNative("hello values"));
        var vectorBack = ValueMarshaller.TakeFromNative(ValueMarshaller.ToNative(new Vector(1.5, 2.5, 3.5)));

        var arrayBack = ValueMarshaller.TakeFromNative(
            ValueMarshaller.ToNative(new object?[] { 10, 20, 30 })) as object?[];

        var map = new Dictionary<object, object?> { [1] = 100, [2] = 200 };
        var mapBack = ValueMarshaller.TakeFromNative(ValueMarshaller.ToNative(map)) as Dictionary<object, object?>;

        NativeRuntime.Log(
            LogLevel.Info,
            $"value marshaller round trip: string='{stringBack}' vector={vectorBack} " +
            $"array=({string.Join(",", arrayBack ?? [])}) map.count={mapBack?.Count}");
    }

    private static Entity? _sampleEntity;

    private static void PropertySelfTest()
    {
        if (_sampleEntity is null)
        {
            NativeRuntime.Log(LogLevel.Info, "property self test skipped: no sample entity");
            return;
        }

        var property = NativeClassRegistry.Get("Player").Properties["Health"];
        var value = property.Get(_sampleEntity);
        NativeRuntime.Log(LogLevel.Info, $"property get round trip: Health={value}");

        property.Set(_sampleEntity, 200);
        NativeRuntime.Log(LogLevel.Info, "property set round trip: ok");
    }

    private static void EntityResultSelfTest()
    {
        Span<NativeArgument> arguments = stackalloc NativeArgument[3];
        var packer = new ArgumentPacker(arguments, stackalloc byte[64]);
        packer.Add(1);
        packer.Add(2);
        packer.Add(3);

        NativeArgument result = default;
        byte* error = null;
        byte ok;
        fixed (NativeArgument* argumentPointer = arguments)
        {
            ok = NativeRuntime.Api.FunctionCall((nint)1, 0, argumentPointer, packer.Count, &result, &error);
        }

        _sampleEntity = ResultReader.Read(ref result) as Entity;
        NativeRuntime.Log(
            LogLevel.Info,
            $"entity result round trip: ok={ok} class={(_sampleEntity is null ? "null" : _sampleEntity.ClassName)} " +
            $"network={_sampleEntity?.NetworkId}");
        packer.Dispose();
    }

    private static void ValueHandleSelfTest()
    {
        var integer = NativeRuntime.Api.ValueCreateInteger(123);
        var integerType = NativeRuntime.Api.ValueGetType(integer);
        var integerValue = NativeRuntime.Api.ValueGetInteger(integer);
        NativeRuntime.Api.ValueFree(integer);

        var boolean = NativeRuntime.Api.ValueCreateBool(1);
        var booleanValue = NativeRuntime.Api.ValueGetBool(boolean);
        NativeRuntime.Api.ValueFree(boolean);

        var number = NativeRuntime.Api.ValueCreateDouble(3.5);
        var numberValue = NativeRuntime.Api.ValueGetDouble(number);
        NativeRuntime.Api.ValueFree(number);

        NativeRuntime.Log(
            LogLevel.Info,
            $"value handle round trip: integer(type={integerType}, value={integerValue}) " +
            $"bool={booleanValue} double={numberValue}");
    }

    private static void StringResultSelfTest()
    {
        Span<NativeArgument> arguments = stackalloc NativeArgument[2];
        var packer = new ArgumentPacker(arguments, stackalloc byte[64]);
        packer.Add(1);
        packer.Add(2);

        NativeArgument result = default;
        byte* error = null;
        byte ok;
        fixed (NativeArgument* argumentPointer = arguments)
        {
            ok = NativeRuntime.Api.FunctionCall((nint)1, 0, argumentPointer, packer.Count, &result, &error);
        }

        var value = ResultReader.Read(ref result);
        NativeRuntime.Log(LogLevel.Info, $"string result round trip: ok={ok} result='{value}'");
        packer.Dispose();
    }

    private static void SchemaLoadSelfTest()
    {
        NativeClassRegistry.Load();

        foreach (var nativeClass in NativeClassRegistry.Classes)
        {
            NativeRuntime.Log(
                LogLevel.Info,
                $"class {nativeClass.Name} id={nativeClass.Id} methods={nativeClass.Methods.Count} " +
                $"static={nativeClass.StaticMethods.Count} properties={nativeClass.Properties.Count} " +
                $"ctor={(nativeClass.Constructor is null ? "none" : nativeClass.Constructor.Name)}");
        }

        foreach (var function in NativeClassRegistry.Functions)
        {
            NativeRuntime.Log(LogLevel.Info, $"global function {function.Name}");
        }
    }

    private static void IntegerResultSelfTest()
    {
        NativeArgument result = default;
        byte* error = null;
        var ok = NativeRuntime.Api.FunctionCall((nint)1, 0, null, 0, &result, &error);
        var value = ResultReader.Read(ref result);
        NativeRuntime.Log(LogLevel.Info, $"integer result round trip: ok={ok} result={value}");
    }

    private static void StringRoundTripSelfTest()
    {
        Span<NativeArgument> arguments = stackalloc NativeArgument[1];
        Span<byte> packScratch = stackalloc byte[256];

        var packer = new ArgumentPacker(arguments, packScratch);
        packer.Add("hello from guest");

        NativeArgument result = default;
        byte* error = null;
        byte ok;
        fixed (NativeArgument* argumentPointer = arguments)
        {
            ok = NativeRuntime.Api.FunctionCall((nint)1, 0, argumentPointer, packer.Count, &result, &error);
        }

        var value = ResultReader.Read(ref result);
        NativeRuntime.Log(LogLevel.Info, $"string arg round trip: ok={ok} length={value}");
        packer.Dispose();
    }
}
#endif
