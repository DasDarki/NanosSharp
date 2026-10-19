using NanosSharp.Host.Compilation;
using NanosSharp.Host.Compilation.Sandbox;

namespace NanosSharp.Host.Tests;

public sealed class SandboxAudit : IDisposable
{
    private readonly string _directory = Directory.CreateTempSubdirectory("nanossharp-audit-").FullName;

    private static readonly (string Name, string Body)[] MustReject =
    [
        ("Reflection.GetMethod", "typeof(string).GetMethod(\"Trim\");"),
        ("Reflection.GetField", "typeof(string).GetField(\"x\");"),
        ("Reflection.GetProperty", "typeof(string).GetProperty(\"Length\");"),
        ("Reflection.Module", "_ = typeof(string).Module;"),
        ("Reflection.Assembly", "_ = typeof(string).Assembly;"),
        ("Reflection.GetMembers", "typeof(string).GetMembers();"),
        ("Reflection.InvokeMember", "typeof(string).InvokeMember(\"\", default, null, null, null);"),
        ("GetTypeThenMethods", "new object().GetType().GetMethods();"),
        ("AssemblyLoad", "System.Reflection.Assembly.Load(\"x\");"),
        ("AssemblyLoadFile", "System.Reflection.Assembly.LoadFile(\"x\");"),
        ("ReflectionEmit", "_ = System.Reflection.Emit.OpCodes.Nop;"),
        ("DynamicMethod", "_ = new System.Reflection.Emit.DynamicMethod(\"x\", null, null);"),
        ("Activator", "System.Activator.CreateInstance(typeof(object));"),
        ("AppDomain", "_ = System.AppDomain.CurrentDomain;"),
        ("AppContext", "_ = System.AppContext.BaseDirectory;"),
        ("EnvironmentExit", "System.Environment.Exit(0);"),
        ("EnvironmentFailFast", "System.Environment.FailFast(\"x\");"),
        ("EnvironmentGetVar", "_ = System.Environment.GetEnvironmentVariable(\"PATH\");"),
        ("EnvironmentCommandLine", "_ = System.Environment.CommandLine;"),
        ("ProcessStart", "System.Diagnostics.Process.Start(\"calc\");"),
        ("ProcessGetCurrent", "_ = System.Diagnostics.Process.GetCurrentProcess();"),
        ("FileRead", "System.IO.File.ReadAllText(\"x\");"),
        ("FileWrite", "System.IO.File.WriteAllText(\"x\", \"y\");"),
        ("Directory", "System.IO.Directory.GetFiles(\"x\");"),
        ("FileStream", "_ = new System.IO.FileStream(\"x\", System.IO.FileMode.Open);"),
        ("HttpClient", "_ = new System.Net.Http.HttpClient();"),
        ("Socket", "_ = new System.Net.Sockets.Socket(default, default, default);"),
        ("Dns", "System.Net.Dns.GetHostName();"),
        ("Thread", "new System.Threading.Thread(() => { }).Start();"),
        ("ThreadPool", "System.Threading.ThreadPool.QueueUserWorkItem(_ => { });"),
        ("Marshal", "System.Runtime.InteropServices.Marshal.AllocHGlobal(16);"),
        ("NativeMemory", "unsafe { System.Runtime.InteropServices.NativeMemory.Alloc(16); }"),
        ("GCHandle", "System.Runtime.InteropServices.GCHandle.Alloc(new object());"),
        ("GCCollect", "System.GC.Collect();"),
        ("GCAllocmem", "System.GC.AddMemoryPressure(1);"),
        ("Unsafe", "_ = System.Runtime.CompilerServices.Unsafe.SizeOf<int>();"),
        ("UnsafeAs", "object o = \"x\"; ref var r = ref System.Runtime.CompilerServices.Unsafe.As<object, int>(ref o);"),
        ("UnsafeAlias", "using U = System.Runtime.CompilerServices.Unsafe; _ = U.SizeOf<int>();"),
        ("RuntimeHelpersUninit", "System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(object));"),
        ("MemoryMarshalGetRef", "System.Span<int> s = stackalloc int[1]; ref var r = ref System.Runtime.InteropServices.MemoryMarshal.GetReference(s);"),
        ("MemoryMarshalCast", "System.Span<int> s = stackalloc int[1]; var b = System.Runtime.InteropServices.MemoryMarshal.Cast<int, byte>(s);"),
        ("MemoryMarshalAsBytes", "System.Span<int> s = stackalloc int[1]; var b = System.Runtime.InteropServices.MemoryMarshal.AsBytes(s);"),
        ("CollectionsMarshal", "var l = new List<int> { 1 }; var s = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(l);"),
        ("CollectionsMarshalAlias", "using CM = System.Runtime.InteropServices.CollectionsMarshal; var l = new List<int> { 1 }; var s = CM.AsSpan(l);"),
        ("ExpressionCompile", "System.Linq.Expressions.Expression<System.Func<int>> e = () => 1; e.Compile();"),
        ("DelegateMethod", "System.Action a = () => { }; _ = ((System.MulticastDelegate)a).Method;"),
        ("DelegateDynamicInvoke", "System.Action a = () => { }; ((System.MulticastDelegate)a).DynamicInvoke();"),
        ("DllImport", "[System.Runtime.InteropServices.DllImport(\"libc\")] static extern int system(string s);"),
        ("FunctionPointer", "unsafe { delegate*<void> p = null; p(); }"),
        ("UnsafeField", "unsafe { int x = 0; int* p = &x; *p = 1; }"),
        ("ModuleInitializer", "[System.Runtime.CompilerServices.ModuleInitializer] static void Init() { }"),
        ("TypeGetType", "_ = System.Type.GetType(\"System.IO.File\");"),
        ("NanosSharpInterop", "_ = NanosSharp.Interop.NativeRuntime.IsEngineAttached;"),
        ("StackTrace", "_ = new System.Diagnostics.StackTrace().ToString();"),
        ("TypeInitializer", "typeof(string).TypeInitializer?.Invoke(null, null);"),
        ("TypedReference", "int x = 0; System.TypedReference r = __makeref(x);"),
        ("ForbiddenAsGenericArg", "var l = new List<System.IO.FileStream>();"),
        ("TypeofForbidden", "var t = typeof(System.IO.File); Log.Info(t.Name);"),
        ("ForbiddenInCast", "object o = null; var f = (System.IO.FileStream)o;"),
        ("ForbiddenField", "System.IO.FileStream f = null; _ = f;"),
        ("NativeLibrary", "System.Runtime.InteropServices.NativeLibrary.Load(\"libc\");"),
        ("RuntimeInformation", "_ = System.Runtime.InteropServices.RuntimeInformation.OSDescription;"),
        ("ConsoleWrite", "System.Console.WriteLine(\"x\");"),
        ("ArgIterator", "var t = typeof(System.ArgIterator); Log.Info(t.Name);"),
        ("UsingStaticUnsafe", "using static System.Runtime.CompilerServices.Unsafe; _ = SizeOf<int>();"),
        ("UsingStaticCollectionsMarshal", "using static System.Runtime.InteropServices.CollectionsMarshal; var l = new List<int> { 1 }; var s = AsSpan(l);"),
        ("UsingStaticMemoryMarshal", "using static System.Runtime.InteropServices.MemoryMarshal; System.Span<int> sp = stackalloc int[1]; ref var r = ref GetReference(sp);"),
        ("UsingStaticFile", "using static System.IO.File; var txt = ReadAllText(\"x\");"),
        ("UsingStaticRuntimeHelpers", "using static System.Runtime.CompilerServices.RuntimeHelpers; var o = GetUninitializedObject(typeof(object));"),
        ("GlobalNamespaceFile", "global::System.IO.File.ReadAllText(\"x\");"),
    ];

    private static readonly (string Name, string Body)[] MustAllow =
    [
        ("Collections", "var d = new Dictionary<string, int> { [\"a\"] = 1 }; Log.Info(d.Count.ToString());"),
        ("Linq", "var n = new[] { 3, 1 }.OrderBy(x => x).First(); Log.Info(n.ToString());"),
        ("SpanLiteral", "ReadOnlySpan<int> s = [1, 2, 3]; Log.Info(s.Length.ToString());"),
        ("Stackalloc", "System.Span<int> s = stackalloc int[4]; s[0] = 1; Log.Info(s[0].ToString());"),
        ("Json", "var j = System.Text.Json.JsonSerializer.Serialize(new { a = 1 }); Log.Info(j);"),
        ("Regex", "var m = System.Text.RegularExpressions.Regex.IsMatch(\"a\", \"a\"); Log.Info(m.ToString());"),
        ("TypeofName", "Log.Info(typeof(string).Name);"),
        ("Async", "await Task.Delay(1); Log.Info(\"done\");"),
        ("TaskRun", "await Task.Run(() => 1); Log.Info(\"done\");"),
        ("GameApi", "var v = new Vector(1, 2, 3) + Vector.Zero; Log.Info(v.ToString());"),
        ("Iterator", "foreach (var n in Count()) { Log.Info(n.ToString()); } static IEnumerable<int> Count() { yield return 1; yield return 2; }"),
        ("ClosureCapture", "var list = new List<System.Func<int>>(); for (var i = 0; i < 3; i++) { var j = i; list.Add(() => j); } Log.Info(list[0]().ToString());"),
        ("Record", "var p = new Point(1, 2) with { X = 3 }; Log.Info(p.X.ToString());"),
        ("Tuple", "var (a, b) = (1, \"x\"); Log.Info($\"{a}{b}\");"),
        ("GenericMethod", "Log.Info(Identity(5).ToString()); static T Identity<T>(T value) => value;"),
    ];

    private static readonly string RecordType = "public readonly record struct Point(int X, int Y);";

    [Fact]
    public void AttacksAreRejected()
    {
        var slipped = MustReject.Where(attack => Compile(attack.Body).Succeeded).Select(attack => attack.Name).ToArray();
        Assert.True(slipped.Length == 0, "These attacks compiled: " + string.Join(", ", slipped));
    }

    [Fact]
    public void LegitimateCodeIsAllowed()
    {
        var blocked = MustAllow.Where(sample => !Compile(sample.Body).Succeeded)
            .Select(sample => $"{sample.Name}: {string.Join(" | ", Compile(sample.Body).Diagnostics)}").ToArray();
        Assert.True(blocked.Length == 0, "These legitimate samples were blocked: " + string.Join("\n", blocked));
    }

    private CompilationResult Compile(string body)
    {
        var source = $$"""
            public static class Attempt
            {
                public static async System.Threading.Tasks.Task Run()
                {
                    {{body}}
                    await System.Threading.Tasks.Task.CompletedTask;
                }
            }
            {{RecordType}}
            """;
        var path = Path.Combine(_directory, $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(path, source);
        return ScriptCompiler.Compile("Audit", [path], [], false, ApiAllowList.Client);
    }

    public void Dispose() => Directory.Delete(_directory, true);
}
