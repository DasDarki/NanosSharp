using System.Text;
using NanosSharp.Interop;
using NanosSharp.Interop.Arguments;
using Wasmtime;
using ValueType = NanosSharp.Interop.Values.ValueType;

namespace NanosSharp.Sandbox;

/// <summary>
/// Runs the sandboxed client runtime inside wasmtime from managed code, via the
/// Wasmtime .NET package. This is the managed counterpart of the native C++
/// WasmHost: the trusted host (CoreCLR) embeds wasmtime, owns the i32 to native
/// handle tables, marshals across guest linear memory, and routes the guest's
/// <c>nanos</c> imports to the real engine seam (the <see cref="NativeApi"/>
/// pointer the native layer already hands the trusted host).
/// </summary>
internal sealed unsafe class WasmHost : IDisposable
{
    private const int ResultScratchRequest = 64 * 1024;

    private readonly NativeApi* _api;
    private readonly bool _ownsEngine;
    private readonly Engine _engine;
    private readonly Module _module;
    private Store _store = null!;
    private Linker _linker = null!;
    private Instance _instance = null!;
    private Memory _memory = null!;

    private readonly List<nint> _functions = [];
    private readonly Dictionary<nint, int> _functionLookup = [];
    private readonly List<nint> _properties = [];
    private readonly Dictionary<nint, int> _propertyLookup = [];
    private readonly List<nint> _values = [];
    private readonly Stack<int> _valueFreeList = new();
    private readonly List<nint> _instances = [];
    private readonly Dictionary<nint, int> _instanceLookup = [];
    private readonly List<nint> _environments = [];
    private readonly Dictionary<nint, int> _environmentLookup = [];

    private int _resultScratch;
    private int _resultScratchCapacity;

    public WasmHost(NativeApi* api, string reactorModule, string packageDirectory)
    {
        _api = api;
        _ownsEngine = true;
        _engine = new Engine();
        _module = Module.FromFile(_engine, reactorModule);
        Initialize(packageDirectory);
    }

    public WasmHost(NativeApi* api, ReactorModule reactor, string packageDirectory)
    {
        _api = api;
        _ownsEngine = false;
        _engine = reactor.Engine;
        _module = reactor.Module;
        Initialize(packageDirectory);
    }

    public static ReactorModule Compile(string reactorModule)
    {
        var engine = new Engine();
        return new ReactorModule(engine, Module.FromFile(engine, reactorModule));
    }

    private void Initialize(string packageDirectory)
    {
        _linker = new Linker(_engine);
        _store = new Store(_engine);

        _store.SetWasiConfiguration(new WasiConfiguration()
            .WithInheritedStandardOutput()
            .WithInheritedStandardError()
            .WithArgs("nanossharp")
            .WithPreopenedDirectory(packageDirectory, "/package"));
        _linker.DefineWasi();

        DefineImports();
    }

    public void Boot()
    {
        _instance = _linker.Instantiate(_store, _module);
        _memory = _instance.GetMemory("memory")
            ?? throw new InvalidOperationException("core module exported no memory");

        _instance.GetFunction("_initialize")?.Invoke();
        _instance.GetFunction("initialize_runtime")?.Invoke();
        _instance.GetFunction("nanos_warmup")?.Invoke();
        (_instance.GetFunction("nanos_boot")
            ?? throw new InvalidOperationException("reactor does not export nanos_boot")).Invoke();
    }

    public void EngineAttached() => _instance.GetFunction("nanos_engine_attached")?.Invoke();

    public void EngineDetaching() => _instance.GetFunction("nanos_engine_detaching")?.Invoke();

    public void Tick(float deltaTime) => _instance.GetFunction("nanos_tick")?.Invoke(deltaTime);

    public void SyncPackages() => _instance.GetFunction("nanos_sync_packages")?.Invoke();

    public nint InvokeFunction(long id, NativeArgument* arguments, int count)
    {
        var invoke = _instance.GetFunction("nanos_invoke_function");
        if (invoke is null)
        {
            return 0;
        }

        var records = count < 0 ? 0 : count;
        var size = records * 32;
        for (var i = 0; i < records; i++)
        {
            if ((ValueType)arguments[i].Type == ValueType.String)
            {
                size += (int)arguments[i].StringLength;
            }
        }

        var block = Allocate(size == 0 ? 1 : size);
        if (block == 0)
        {
            return 0;
        }

        var baseptr = MemoryBase();
        var stringCursor = block + records * 32;
        for (var i = 0; i < records; i++)
        {
            WriteGuestArgument(baseptr + block + i * 32, ref arguments[i], ref stringCursor);
        }

        var result = invoke.Invoke(id, block, records);
        Free(block);
        return TakeValue(result is null ? 0 : Convert.ToInt32(result));
    }

    public void ReleaseFunction(long id) => _instance.GetFunction("nanos_release_function")?.Invoke(id);

    public void EntityCreated(ulong classId, nint instance, uint networkId) =>
        _instance.GetFunction("nanos_entity_created")?.Invoke((long)classId, InternInstance(instance), (int)networkId);

    public void EntityDestroyed(ulong classId, nint instance)
    {
        if (_instanceLookup.TryGetValue(instance, out var index))
        {
            _instance.GetFunction("nanos_entity_destroyed")?.Invoke((long)classId, index);
        }
    }

    public Function? GetExport(string name) => _instance.GetFunction(name);

    private void DefineImports()
    {
        foreach (var import in _module.Imports)
        {
            if (import is not FunctionImport function)
            {
                continue;
            }

            if (function.ModuleName == "nanos")
            {
                var name = function.Name;
                _linker.DefineFunction("nanos", name,
                    (Caller caller, ReadOnlySpan<ValueBox> arguments, Span<ValueBox> results) =>
                        Dispatch(name, arguments, results),
                    function.Parameters, function.Results);
            }
            else if (function.ModuleName == "wasi_snapshot_preview1" && function.Name != "adapter_close_badfd")
            {
                // provided by DefineWasi (preview1)
            }
            else
            {
                var resultKinds = function.Results;
                _linker.DefineFunction(function.ModuleName, function.Name,
                    (Caller caller, ReadOnlySpan<ValueBox> arguments, Span<ValueBox> results) =>
                    {
                        for (var i = 0; i < results.Length; i++)
                        {
                            results[i] = Zero(resultKinds[i]);
                        }
                    },
                    function.Parameters, function.Results);
            }
        }
    }

    private static ValueBox Zero(ValueKind kind) => kind switch
    {
        ValueKind.Int64 => 0L,
        ValueKind.Float32 => 0f,
        ValueKind.Float64 => 0.0,
        _ => 0,
    };

    private int Intern(nint handle, List<nint> table, Dictionary<nint, int> lookup)
    {
        if (handle == 0)
        {
            return 0;
        }

        if (lookup.TryGetValue(handle, out var existing))
        {
            return existing;
        }

        table.Add(handle);
        var index = table.Count;
        lookup[handle] = index;
        return index;
    }

    private int InternFunction(nint handle) => Intern(handle, _functions, _functionLookup);

    private int InternProperty(nint handle) => Intern(handle, _properties, _propertyLookup);

    private int InternInstance(nint handle) => Intern(handle, _instances, _instanceLookup);

    private int InternEnvironment(nint handle) => Intern(handle, _environments, _environmentLookup);

    private int InternValue(nint handle)
    {
        if (handle == 0)
        {
            return 0;
        }

        if (_valueFreeList.Count > 0)
        {
            var index = _valueFreeList.Pop();
            _values[index - 1] = handle;
            return index;
        }

        _values.Add(handle);
        return _values.Count;
    }

    private nint ResolveFunction(int index) =>
        index > 0 && index <= _functions.Count ? _functions[index - 1] : 0;

    private nint ResolveProperty(int index) =>
        index > 0 && index <= _properties.Count ? _properties[index - 1] : 0;

    private nint ResolveInstance(int index) =>
        index > 0 && index <= _instances.Count ? _instances[index - 1] : 0;

    private nint ResolveEnvironment(int index) =>
        index > 0 && index <= _environments.Count ? _environments[index - 1] : 0;

    private nint ResolveValue(int index) =>
        index > 0 && index <= _values.Count ? _values[index - 1] : 0;

    private byte* MemoryBase() => (byte*)_memory.GetPointer();

    private string ReadGuestString(int pointer, long length)
    {
        if (pointer < 0 || length < 0)
        {
            return string.Empty;
        }

        return Encoding.UTF8.GetString(MemoryBase() + pointer, (int)length);
    }

    private void Dispatch(string name, ReadOnlySpan<ValueBox> a, Span<ValueBox> r)
    {
        switch (name)
        {
            case "log":
                _api->Log(a[0].AsInt32(), MemoryBase() + a[1].AsInt32(), a[2].AsInt32());
                break;
            case "free-string":
                break;
            case "engine-is-attached":
                r[0] = _api->EngineIsAttached();
                break;
            case "set-result-scratch":
                _resultScratch = a[0].AsInt32();
                _resultScratchCapacity = a[1].AsInt32();
                break;
            case "schema-dump":
                r[0] = SchemaDump(a[0].AsInt32(), a[1].AsInt32());
                break;
            case "function-call":
                r[0] = FunctionCall(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32(),
                    a[4].AsInt32(), a[5].AsInt32());
                break;
            case "value-create-nil":
                r[0] = InternValue(_api->ValueCreateNil());
                break;
            case "value-create-bool":
                r[0] = InternValue(_api->ValueCreateBool(a[0].AsInt32() != 0 ? (byte)1 : (byte)0));
                break;
            case "value-create-integer":
                r[0] = InternValue(_api->ValueCreateInteger(a[0].AsInt64()));
                break;
            case "value-create-double":
                r[0] = InternValue(_api->ValueCreateDouble(a[0].AsDouble()));
                break;
            case "value-create-string":
                r[0] = InternValue(_api->ValueCreateString(MemoryBase() + a[0].AsInt32(), a[1].AsInt32()));
                break;
            case "value-create-vector":
                r[0] = InternValue(_api->ValueCreateVector(a[0].AsDouble(), a[1].AsDouble(), a[2].AsDouble()));
                break;
            case "value-create-rotator":
                r[0] = InternValue(_api->ValueCreateRotator(a[0].AsDouble(), a[1].AsDouble(), a[2].AsDouble()));
                break;
            case "value-create-color":
                r[0] = InternValue(_api->ValueCreateColor(a[0].AsSingle(), a[1].AsSingle(), a[2].AsSingle(), a[3].AsSingle()));
                break;
            case "value-create-vector2-d":
                r[0] = InternValue(_api->ValueCreateVector2D(a[0].AsDouble(), a[1].AsDouble()));
                break;
            case "value-create-entity":
                r[0] = InternValue(_api->ValueCreateEntity((ulong)a[0].AsInt64(), ResolveInstance(a[1].AsInt32()), (uint)a[2].AsInt32()));
                break;
            case "value-create-array":
                r[0] = ValueCreateArray(a[0].AsInt32(), a[1].AsInt32());
                break;
            case "value-create-map":
                r[0] = ValueCreateMap(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32());
                break;
            case "value-create-function":
                r[0] = InternValue(_api->ValueCreateFunction(a[0].AsInt64(), ResolveEnvironment(a[1].AsInt32())));
                break;
            case "value-clone":
                r[0] = InternValue(_api->ValueClone(ResolveValue(a[0].AsInt32())));
                break;
            case "value-free":
                ValueFree(a[0].AsInt32());
                break;
            case "value-get-type":
                r[0] = _api->ValueGetType(ResolveValue(a[0].AsInt32()));
                break;
            case "value-get-bool":
                r[0] = (int)_api->ValueGetBool(ResolveValue(a[0].AsInt32()));
                break;
            case "value-get-integer":
                r[0] = _api->ValueGetInteger(ResolveValue(a[0].AsInt32()));
                break;
            case "value-get-double":
                r[0] = _api->ValueGetDouble(ResolveValue(a[0].AsInt32()));
                break;
            case "value-get-string":
                r[0] = ValueGetString(a[0].AsInt32(), a[1].AsInt32());
                break;
            case "value-get-doubles":
                ValueGetDoubles(a[0].AsInt32(), a[1].AsInt32());
                break;
            case "value-get-color":
                ValueGetColor(a[0].AsInt32(), a[1].AsInt32());
                break;
            case "value-get-entity":
                ValueGetEntity(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32());
                break;
            case "value-get-network-id":
                r[0] = (int)_api->ValueGetNetworkId(ResolveValue(a[0].AsInt32()));
                break;
            case "value-resolve-deferred":
                r[0] = InternValue(_api->ValueResolveDeferred(ResolveValue(a[0].AsInt32())));
                break;
            case "value-get-length":
                r[0] = _api->ValueGetLength(ResolveValue(a[0].AsInt32()));
                break;
            case "value-get-item":
                r[0] = InternValue(_api->ValueGetItem(ResolveValue(a[0].AsInt32()), a[1].AsInt32()));
                break;
            case "value-get-entries":
                ValueGetEntries(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32());
                break;
            case "value-invoke":
                r[0] = ValueInvoke(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32(), a[4].AsInt32());
                break;
            case "function-invoke":
                r[0] = FunctionInvoke(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32(), a[4].AsInt32(), a[5].AsInt32());
                break;
            case "property-get":
                r[0] = PropertyGet(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32());
                break;
            case "property-set":
                r[0] = PropertySet(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32());
                break;
            case "environment-create":
                r[0] = InternEnvironment(_api->EnvironmentCreate(MemoryBase() + a[0].AsInt32(), a[1].AsInt32()));
                break;
            case "environment-destroy":
                _api->EnvironmentDestroy(MemoryBase() + a[0].AsInt32(), a[1].AsInt32());
                break;
            case "lua-get-global":
                r[0] = LuaGetGlobal(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32());
                break;
            case "lua-set-global":
                r[0] = LuaSetGlobal(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32(), a[4].AsInt32());
                break;
            case "library-load":
                r[0] = LibraryLoad(a[0].AsInt32());
                break;
            case "package-reload":
                r[0] = PackageReload(a[0].AsInt32());
                break;
            case "package-get-entity":
                r[0] = InternInstance(_api->PackageGetEntity(ResolveInstance(a[0].AsInt32())));
                break;
            case "function-get-signature":
                r[0] = WriteScratchString(_api->FunctionGetSignature(ResolveFunction(a[0].AsInt32())));
                break;
            case "property-get-signature":
                r[0] = WriteScratchString(_api->PropertyGetSignature(ResolveProperty(a[0].AsInt32())));
                break;
            default:
                for (var i = 0; i < r.Length; i++)
                {
                    r[i] = 0;
                }
                break;
        }
    }

    private int SchemaDump(int buffer, int capacity)
    {
        var blob = BuildSchemaBlob();
        if (blob.Length > capacity)
        {
            return blob.Length;
        }

        blob.CopyTo(_memory.GetSpan(buffer, blob.Length));
        return blob.Length;
    }

    private byte[] BuildSchemaBlob()
    {
        var blob = new List<byte>();

        void PutU16(int value) { blob.Add((byte)value); blob.Add((byte)(value >> 8)); }
        void PutU32(uint value) { for (var i = 0; i < 4; i++) blob.Add((byte)(value >> (i * 8))); }
        void PutU64(ulong value) { for (var i = 0; i < 8; i++) blob.Add((byte)(value >> (i * 8))); }
        void PutI32(int value) { PutU32((uint)value); }
        void PutString(byte* text)
        {
            var s = NativeRuntime.ReadString(text);
            var bytes = Encoding.UTF8.GetBytes(s);
            PutU16(bytes.Length);
            blob.AddRange(bytes);
        }

        var classCount = _api->ClassCount();
        PutU32((uint)classCount);
        for (var index = 0; index < classCount; index++)
        {
            var handle = _api->ClassGet(index);
            PutU64(_api->ClassGetId(handle));
            PutString(_api->ClassGetName(handle));

            var constructor = _api->ClassGetConstructor(handle);
            PutI32(InternFunction(constructor));
            if (constructor != 0)
            {
                PutString(_api->FunctionGetName(constructor));
            }

            for (byte isStatic = 0; isStatic <= 1; isStatic++)
            {
                var count = _api->ClassGetFunctionCount(handle, isStatic);
                PutU16(count);
                for (var i = 0; i < count; i++)
                {
                    var fn = _api->ClassGetFunction(handle, isStatic, i);
                    PutI32(InternFunction(fn));
                    PutString(_api->FunctionGetName(fn));
                }
            }

            var propertyCount = _api->ClassGetPropertyCount(handle);
            PutU16(propertyCount);
            for (var i = 0; i < propertyCount; i++)
            {
                var property = _api->ClassGetProperty(handle, i);
                PutI32(InternProperty(property));
                PutString(_api->PropertyGetName(property));
            }
        }

        var functionCount = _api->GlobalFunctionCount();
        PutU32((uint)functionCount);
        for (var index = 0; index < functionCount; index++)
        {
            var fn = _api->GlobalFunctionGet(index);
            PutI32(InternFunction(fn));
            PutString(_api->FunctionGetName(fn));
        }

        return [.. blob];
    }

    private int FunctionCall(int method, int environment, int args, int count, int result, int error)
    {
        var baseptr = MemoryBase();
        Span<NativeArgument> native = count <= 16 ? stackalloc NativeArgument[count] : new NativeArgument[count];
        for (var i = 0; i < count; i++)
        {
            ReadGuestArgument(baseptr + args + i * 32, ref native[i]);
        }

        NativeArgument resultArgument = default;
        byte* message = null;
        byte ok;
        fixed (NativeArgument* pointer = native)
        {
            ok = _api->FunctionCall((nint)ResolveFunction(method), environment, pointer, count, &resultArgument, &message);
        }

        if (ok == 0)
        {
            if (message != null)
            {
                _api->FreeString(message);
            }

            return 0;
        }

        WriteGuestResult(baseptr + result, ref resultArgument);
        return 1;
    }

    private void ReadGuestArgument(byte* source, ref NativeArgument target)
    {
        target = default;
        target.Type = source[0];
        switch ((ValueType)target.Type)
        {
            case ValueType.Bool:
                target.Boolean = source[8];
                break;
            case ValueType.Integer:
                target.Integer = *(long*)(source + 8);
                break;
            case ValueType.Double:
                target.Number = *(double*)(source + 8);
                break;
            case ValueType.String:
                target.StringData = (nint)(MemoryBase() + *(int*)(source + 8));
                target.StringLength = *(long*)(source + 16);
                break;
            case ValueType.Entity:
                target.ClassId = *(ulong*)(source + 8);
                target.Instance = ResolveInstance(*(int*)(source + 16));
                target.NetworkId = *(uint*)(source + 24);
                break;
            default:
                Buffer.MemoryCopy(source + 8, (byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref target) + 8, 24, 24);
                break;
        }
    }

    private void WriteGuestResult(byte* target, ref NativeArgument source)
    {
        for (var i = 0; i < 32; i++)
        {
            target[i] = 0;
        }

        target[0] = source.Type;
        switch ((ValueType)source.Type)
        {
            case ValueType.Bool:
                target[8] = source.Boolean;
                break;
            case ValueType.Integer:
                *(long*)(target + 8) = source.Integer;
                break;
            case ValueType.Double:
                *(double*)(target + 8) = source.Number;
                break;
            case ValueType.String:
                if (_resultScratch != 0 && source.StringLength >= 0 && source.StringLength <= _resultScratchCapacity)
                {
                    Buffer.MemoryCopy((void*)source.StringData, MemoryBase() + _resultScratch, source.StringLength, source.StringLength);
                    *(int*)(target + 8) = _resultScratch;
                    *(long*)(target + 16) = source.StringLength;
                }
                break;
            case ValueType.Entity:
                *(ulong*)(target + 8) = source.ClassId;
                *(int*)(target + 16) = InternInstance(source.Instance);
                *(uint*)(target + 24) = source.NetworkId;
                break;
            default:
                Buffer.MemoryCopy((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref source) + 8, target + 8, 24, 24);
                break;
        }
    }

    private int ValueCreateArray(int items, int count)
    {
        var baseptr = MemoryBase();
        var native = new nint[count];
        for (var i = 0; i < count; i++)
        {
            native[i] = ResolveValue(*(int*)(baseptr + items + i * 4));
        }

        fixed (nint* pointer = native)
        {
            return InternValue(_api->ValueCreateArray(pointer, count));
        }
    }

    private int ValueCreateMap(int keys, int values, int count)
    {
        var baseptr = MemoryBase();
        var nativeKeys = new nint[count];
        var nativeValues = new nint[count];
        for (var i = 0; i < count; i++)
        {
            nativeKeys[i] = ResolveValue(*(int*)(baseptr + keys + i * 4));
            nativeValues[i] = ResolveValue(*(int*)(baseptr + values + i * 4));
        }

        fixed (nint* keyPointer = nativeKeys, valuePointer = nativeValues)
        {
            return InternValue(_api->ValueCreateMap(keyPointer, valuePointer, count));
        }
    }

    private void ValueFree(int value)
    {
        if (value <= 0 || value > _values.Count)
        {
            return;
        }

        _api->ValueFree(_values[value - 1]);
        _values[value - 1] = 0;
        _valueFreeList.Push(value);
    }

    private int ValueGetString(int value, int lengthPointer)
    {
        int textLength;
        var text = _api->ValueGetString(ResolveValue(value), &textLength);
        var baseptr = MemoryBase();
        if (_resultScratch == 0 || text == null || textLength < 0 || textLength > _resultScratchCapacity)
        {
            *(int*)(baseptr + lengthPointer) = 0;
            return 0;
        }

        Buffer.MemoryCopy(text, baseptr + _resultScratch, textLength, textLength);
        *(int*)(baseptr + lengthPointer) = textLength;
        return _resultScratch;
    }

    private void ValueGetDoubles(int value, int output)
    {
        var buffer = stackalloc double[3];
        _api->ValueGetDoubles(ResolveValue(value), buffer);
        Buffer.MemoryCopy(buffer, MemoryBase() + output, 24, 24);
    }

    private void ValueGetColor(int value, int output)
    {
        var buffer = stackalloc float[4];
        _api->ValueGetColor(ResolveValue(value), buffer);
        Buffer.MemoryCopy(buffer, MemoryBase() + output, 16, 16);
    }

    private void ValueGetEntity(int value, int classId, int instance, int networkId)
    {
        ulong nativeClassId;
        nint nativeInstance;
        uint nativeNetworkId;
        _api->ValueGetEntity(ResolveValue(value), &nativeClassId, &nativeInstance, &nativeNetworkId);

        var baseptr = MemoryBase();
        *(ulong*)(baseptr + classId) = nativeClassId;
        *(int*)(baseptr + instance) = InternInstance(nativeInstance);
        *(uint*)(baseptr + networkId) = nativeNetworkId;
    }

    private void ValueGetEntries(int value, int keys, int values)
    {
        var length = _api->ValueGetLength(ResolveValue(value));
        if (length <= 0)
        {
            return;
        }

        var nativeKeys = new nint[length];
        var nativeValues = new nint[length];
        fixed (nint* keyPointer = nativeKeys, valuePointer = nativeValues)
        {
            _api->ValueGetEntries(ResolveValue(value), keyPointer, valuePointer);
        }

        var baseptr = MemoryBase();
        for (var i = 0; i < length; i++)
        {
            *(int*)(baseptr + keys + i * 4) = InternValue(nativeKeys[i]);
            *(int*)(baseptr + values + i * 4) = InternValue(nativeValues[i]);
        }
    }

    private int ValueInvoke(int function, int args, int count, int result, int error)
    {
        var native = ReadValueHandles(args, count);
        nint value;
        byte* message = null;
        byte ok;
        fixed (nint* pointer = native)
        {
            value = 0;
            ok = _api->ValueInvoke(ResolveValue(function), pointer, count, &value, &message);
        }

        if (ok == 0)
        {
            if (message != null)
            {
                _api->FreeString(message);
            }

            return 0;
        }

        *(int*)(MemoryBase() + result) = InternValue(value);
        return 1;
    }

    private int FunctionInvoke(int handle, int environment, int args, int count, int result, int error)
    {
        var native = ReadValueHandles(args, count);
        nint value;
        byte* message = null;
        byte ok;
        fixed (nint* pointer = native)
        {
            value = 0;
            ok = _api->FunctionInvoke(ResolveFunction(handle), ResolveEnvironment(environment), pointer, count, &value, &message);
        }

        if (ok == 0)
        {
            if (message != null)
            {
                _api->FreeString(message);
            }

            return 0;
        }

        *(int*)(MemoryBase() + result) = InternValue(value);
        return 1;
    }

    private nint[] ReadValueHandles(int pointer, int count)
    {
        var baseptr = MemoryBase();
        var native = new nint[count < 0 ? 0 : count];
        for (var i = 0; i < native.Length; i++)
        {
            native[i] = ResolveValue(*(int*)(baseptr + pointer + i * 4));
        }

        return native;
    }

    private int PropertyGet(int handle, int self, int result, int error)
    {
        nint value;
        byte* message = null;
        var ok = _api->PropertyGet(ResolveProperty(handle), ResolveValue(self), &value, &message);
        if (ok == 0)
        {
            if (message != null)
            {
                _api->FreeString(message);
            }

            return 0;
        }

        *(int*)(MemoryBase() + result) = InternValue(value);
        return 1;
    }

    private int PropertySet(int handle, int self, int value, int error)
    {
        byte* message = null;
        var ok = _api->PropertySet(ResolveProperty(handle), ResolveValue(self), ResolveValue(value), &message);
        if (ok == 0 && message != null)
        {
            _api->FreeString(message);
        }

        return ok;
    }

    private int LuaGetGlobal(int environment, int name, int length, int error)
    {
        byte* message = null;
        var value = _api->LuaGetGlobal(ResolveEnvironment(environment), MemoryBase() + name, length, &message);
        if (message != null)
        {
            _api->FreeString(message);
        }

        return InternValue(value);
    }

    private int LuaSetGlobal(int environment, int name, int length, int value, int error)
    {
        byte* message = null;
        var ok = _api->LuaSetGlobal(ResolveEnvironment(environment), MemoryBase() + name, length, ResolveValue(value), &message);
        if (ok == 0 && message != null)
        {
            _api->FreeString(message);
        }

        return ok;
    }

    private int LibraryLoad(int path)
    {
        byte* message = null;
        var handle = _api->LibraryLoad(MemoryBase() + path, &message);
        if (message != null)
        {
            _api->FreeString(message);
        }

        return InternInstance(handle);
    }

    private int PackageReload(int package)
    {
        byte* message = null;
        var ok = _api->PackageReload(ResolveInstance(package), &message);
        if (ok == 0 && message != null)
        {
            _api->FreeString(message);
        }

        return ok;
    }

    private int WriteScratchString(byte* text)
    {
        if (_resultScratch == 0 || text == null)
        {
            return 0;
        }

        var s = NativeRuntime.ReadString(text);
        var bytes = Encoding.UTF8.GetBytes(s);
        if (bytes.Length + 1 > _resultScratchCapacity)
        {
            _api->FreeString(text);
            return 0;
        }

        var baseptr = MemoryBase();
        for (var i = 0; i < bytes.Length; i++)
        {
            baseptr[_resultScratch + i] = bytes[i];
        }

        baseptr[_resultScratch + bytes.Length] = 0;
        _api->FreeString(text);
        return _resultScratch;
    }

    private int Allocate(int size)
    {
        var result = _instance.GetFunction("malloc")?.Invoke(size);
        return result is null ? 0 : Convert.ToInt32(result);
    }

    private void Free(int offset)
    {
        if (offset != 0)
        {
            _instance.GetFunction("free")?.Invoke(offset);
        }
    }

    private nint TakeValue(int index)
    {
        if (index <= 0 || index > _values.Count)
        {
            return 0;
        }

        var handle = _values[index - 1];
        _values[index - 1] = 0;
        _valueFreeList.Push(index);
        return handle;
    }

    private void WriteGuestArgument(byte* target, ref NativeArgument source, ref int stringCursor)
    {
        for (var i = 0; i < 32; i++)
        {
            target[i] = 0;
        }

        target[0] = source.Type;
        switch ((ValueType)source.Type)
        {
            case ValueType.Bool:
                target[8] = source.Boolean;
                break;
            case ValueType.Integer:
                *(long*)(target + 8) = source.Integer;
                break;
            case ValueType.Double:
                *(double*)(target + 8) = source.Number;
                break;
            case ValueType.String:
                var length = (int)source.StringLength;
                if (length > 0 && source.StringData != 0)
                {
                    Buffer.MemoryCopy((void*)source.StringData, MemoryBase() + stringCursor, length, length);
                }

                *(int*)(target + 8) = stringCursor;
                *(long*)(target + 16) = source.StringLength;
                stringCursor += length;
                break;
            case ValueType.Entity:
                *(ulong*)(target + 8) = source.ClassId;
                *(int*)(target + 16) = InternInstance(source.Instance);
                *(uint*)(target + 24) = source.NetworkId;
                break;
            default:
                if (source.Type == NativeArgument.HandleType || source.Type == NativeArgument.BorrowedHandleType)
                {
                    *(int*)(target + 8) = InternValue(source.Handle);
                }
                else
                {
                    Buffer.MemoryCopy((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref source) + 8, target + 8, 24, 24);
                }

                break;
        }
    }

    public void Dispose()
    {
        _store.Dispose();
        _linker.Dispose();
        if (_ownsEngine)
        {
            _module.Dispose();
            _engine.Dispose();
        }
    }
}

/// <summary>
/// A reactor module compiled once and reused across <see cref="WasmHost"/>
/// instances. Compiling the reactor is the expensive part of a reboot, so the
/// host caches this and rebuilds only the lightweight store and instance.
/// </summary>
internal sealed class ReactorModule : IDisposable
{
    internal ReactorModule(Engine engine, Module module)
    {
        Engine = engine;
        Module = module;
    }

    internal Engine Engine { get; }

    internal Module Module { get; }

    public void Dispose()
    {
        Module.Dispose();
        Engine.Dispose();
    }
}
