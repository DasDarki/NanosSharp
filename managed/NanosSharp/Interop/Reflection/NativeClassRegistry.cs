using NanosSharp.Scripting;

namespace NanosSharp.Interop.Reflection;

internal static unsafe class NativeClassRegistry
{
    private static readonly Dictionary<string, NativeClass> ClassesByName = new(StringComparer.Ordinal);
    private static readonly Dictionary<ulong, NativeClass> ClassesById = [];
    private static readonly Dictionary<string, NativeMethod> GlobalFunctions = new(StringComparer.Ordinal);

    public static int Generation { get; private set; }

    public static IEnumerable<NativeClass> Classes => ClassesByName.Values;

    public static IEnumerable<NativeMethod> Functions => GlobalFunctions.Values;

    public static void Load()
    {
        Clear();
        ref readonly var api = ref NativeRuntime.Api;
        var classCount = api.ClassCount();
        for (var index = 0; index < classCount; index++)
        {
            var nativeClass = new NativeClass(api.ClassGet(index));
            ClassesByName.TryAdd(nativeClass.Name, nativeClass);
            ClassesById.TryAdd(nativeClass.Id, nativeClass);
        }

        var functionCount = api.GlobalFunctionCount();
        for (var index = 0; index < functionCount; index++)
        {
            var function = new NativeMethod(api.GlobalFunctionGet(index));
            GlobalFunctions.TryAdd(function.Name, function);
        }
    }

    public static void Clear()
    {
        Generation++;
        ClassesByName.Clear();
        ClassesById.Clear();
        GlobalFunctions.Clear();
    }

    public static NativeClass Get(string name) =>
        ClassesByName.TryGetValue(name, out var nativeClass)
            ? nativeClass
            : throw new ScriptException($"The scripting class {name} does not exist or the engine is not attached.");

    public static NativeClass? Find(ulong id) => ClassesById.GetValueOrDefault(id);

    public static NativeMethod GetFunction(string name) =>
        GlobalFunctions.TryGetValue(name, out var function)
            ? function
            : throw new ScriptException($"The global function {name} does not exist.");
}
