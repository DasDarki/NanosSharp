namespace NanosSharp.Interop.Reflection;

internal sealed unsafe class NativeClass
{
    public string Name { get; }

    public ulong Id { get; }

    public NativeMethod? Constructor { get; }

    public IReadOnlyDictionary<string, NativeMethod> Methods { get; }

    public IReadOnlyDictionary<string, NativeMethod> StaticMethods { get; }

    public IReadOnlyDictionary<string, NativeProperty> Properties { get; }

    public NativeClass(nint handle)
    {
        ref readonly var api = ref NativeRuntime.Api;
        Name = NativeRuntime.ReadString(api.ClassGetName(handle));
        Id = api.ClassGetId(handle);

        var constructor = api.ClassGetConstructor(handle);
        Constructor = constructor == 0 ? null : new NativeMethod(constructor);
        Methods = ReadMethods(handle, false);
        StaticMethods = ReadMethods(handle, true);

        var properties = new Dictionary<string, NativeProperty>(StringComparer.Ordinal);
        var propertyCount = api.ClassGetPropertyCount(handle);
        for (var index = 0; index < propertyCount; index++)
        {
            var property = new NativeProperty(api.ClassGetProperty(handle, index));
            properties.TryAdd(property.Name, property);
        }

        Properties = properties;
    }

    public NativeMethod GetMethod(string name) =>
        Methods.TryGetValue(name, out var method)
            ? method
            : throw new MissingMethodException($"{Name} has no method named {name}.");

    public NativeMethod GetStaticMethod(string name) =>
        StaticMethods.TryGetValue(name, out var method)
            ? method
            : throw new MissingMethodException($"{Name} has no static function named {name}.");

    private static Dictionary<string, NativeMethod> ReadMethods(nint handle, bool isStatic)
    {
        ref readonly var api = ref NativeRuntime.Api;
        var flag = isStatic ? (byte)1 : (byte)0;
        var methods = new Dictionary<string, NativeMethod>(StringComparer.Ordinal);
        var count = api.ClassGetFunctionCount(handle, flag);
        for (var index = 0; index < count; index++)
        {
            var method = new NativeMethod(api.ClassGetFunction(handle, flag, index));
            methods.TryAdd(method.Name, method);
        }

        return methods;
    }
}
