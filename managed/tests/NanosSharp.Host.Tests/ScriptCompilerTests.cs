using NanosSharp.Host.Compilation;

namespace NanosSharp.Host.Tests;

public sealed class ScriptCompilerTests : IDisposable
{
    private readonly string _directory = Directory.CreateTempSubdirectory("nanossharp-compile-").FullName;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OrdinaryCodeCompiles(bool optimize)
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
    public void DependencyInjectionCompiles()
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

    public void Dispose() => Directory.Delete(_directory, true);

    private CompilationResult Compile(string source) => Compile(false, source);

    private CompilationResult Compile(bool optimize, string source)
    {
        var path = Path.Combine(_directory, $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(path, source);
        return ScriptCompiler.Compile("CompileTest", [path], [], optimize);
    }
}
