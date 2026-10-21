using NanosSharp.Host.Compilation;
using NanosSharp.Host.Compilation.Sandbox;

namespace NanosSharp.Host.Tests;

public sealed class SandboxTests : IDisposable
{
    private readonly string _directory = Directory.CreateTempSubdirectory("nanossharp-sandbox-").FullName;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OrdinaryClientCodeIsAllowed(bool optimize)
    {
        var result = Compile(optimize, """
            public sealed record Score(string Name, int Points);

            public static class Scoreboard
            {
                private static readonly List<Score> Scores = [new("first", 3), new("second", 7)];

                public static async Task<string> DescribeAsync(CancellationToken token)
                {
                    await Task.Delay(1, token);
                    var builder = new System.Text.StringBuilder();
                    foreach (var score in Scores.Where(score => score.Points > 1).OrderByDescending(score => score.Points))
                    {
                        builder.Append($"{score.Name}={Math.Max(score.Points, 0)};");
                    }

                    var lookup = new Dictionary<string, int> { ["a"] = 1 };
                    ReadOnlySpan<int> values = [1, 2, 3];
                    var location = new Vector(1, 2, 3) + new Vector(1, 1, 1);
                    Log.Info($"{builder} {lookup.Count} {values.Length} {location} {typeof(Score).Name}");
                    return builder.ToString();
                }
            }
            """);

        Assert.True(result.Succeeded, string.Join(Environment.NewLine, result.Diagnostics));
    }

    [Fact]
    public void DependencyInjectionIsAllowed()
    {
        var result = Compile("""
            public sealed class Config
            {
                public int MaxPlayers { get; set; } = 32;
            }

            public sealed class Stats : Service, IAutoRegister
            {
                [Inject] private Config _config = null!;

                public int Online { get; private set; }

                [OnInit]
                private void Initialize() => Log.Info($"max {_config.MaxPlayers}");

                public override Task StartAsync(CancellationToken token) => Task.CompletedTask;

                [ServerEvent]
                private void OnTick(ServerTickEvent tickEvent) => Online++;
            }

            public sealed class Heartbeat : ScheduledService
            {
                [Inject] private Stats _stats = null!;

                public Heartbeat() : base(TimeSpan.FromSeconds(1))
                {
                }

                protected override void OnTick(float deltaTime) => Log.Info($"online {_stats.Online}");
            }

            public sealed class Game : Package
            {
                protected override void OnStart()
                {
                    Services.Add(new Config());
                    Log.Info($"online {Services.GetRequired<Stats>().Online}");
                }
            }
            """);

        Assert.True(result.Succeeded, string.Join(Environment.NewLine, result.Diagnostics));
    }

    [Theory]
    [InlineData("System.IO.File.ReadAllText(\"secret.txt\");", "System.IO.File")]
    [InlineData("System.Diagnostics.Process.Start(\"calc\");", "System.Diagnostics.Process")]
    [InlineData("typeof(string).GetMethods();", "System.Type::GetMethods")]
    [InlineData("_ = new object().GetType().Assembly;", "System.Type::get_Assembly")]
    [InlineData("System.Environment.Exit(0);", "System.Environment::Exit")]
    [InlineData("new System.Threading.Thread(() => { }).Start();", "System.Threading.Thread")]
    [InlineData("System.Activator.CreateInstance(typeof(object));", "System.Activator")]
    [InlineData("System.Runtime.InteropServices.Marshal.AllocHGlobal(16);", "System.Runtime.InteropServices.Marshal")]
    [InlineData("System.Linq.Expressions.Expression<Func<int>> expression = () => 1;", "System.Linq.Expressions")]
    [InlineData("_ = new System.Net.Http.HttpClient();", "System.Net.Http.HttpClient")]
    [InlineData("_ = System.Runtime.CompilerServices.Unsafe.SizeOf<int>();", "System.Runtime.CompilerServices.Unsafe")]
    [InlineData("System.GC.Collect();", "System.GC")]
    public void ForbiddenApisAreRejected(string statement, string expected)
    {
        var result = Compile($$"""
            public static class Attempt
            {
                public static void Run()
                {
                    {{statement}}
                }
            }
            """);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Contains(expected, StringComparison.Ordinal));
    }

    [Fact]
    public void UnsafeHelperAliasesAreRejected()
    {
        var result = Compile("""
            using Helpers = System.Runtime.CompilerServices.Unsafe;

            public static class Attempt
            {
                public static string Run(object value) => Helpers.As<string>(value);
            }
            """);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Contains("System.Runtime.CompilerServices.Unsafe", StringComparison.Ordinal));
    }

    [Fact]
    public void NativeImportsAreRejected()
    {
        var result = Compile("""
            public static class Attempt
            {
                [System.Runtime.InteropServices.DllImport("libc")]
                public static extern int getpid();
            }
            """);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public void UnsafeCodeIsRejected()
    {
        var result = Compile("""
            public static class Attempt
            {
                public static unsafe int Run()
                {
                    var value = 1;
                    int* pointer = &value;
                    return *pointer;
                }
            }
            """);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public void DynamicIsRejected()
    {
        var result = Compile("""
            public static class Attempt
            {
                public static object Run(dynamic value) => value.Anything();
            }
            """);

        Assert.False(result.Succeeded);
    }

    public void Dispose() => Directory.Delete(_directory, true);

    private CompilationResult Compile(string source) => Compile(false, source);

    private CompilationResult Compile(bool optimize, string source)
    {
        var path = Path.Combine(_directory, $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(path, source);
        return ScriptCompiler.Compile("SandboxTest", [path], [], optimize, ApiAllowList.Client);
    }
}
