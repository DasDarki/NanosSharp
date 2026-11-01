using System.Runtime.Loader;
using System.Text;
using NanosSharp.Host.Compilation;
using NanosSharp.Host.Packages;

namespace NanosSharp.Host.Tests;

public class ClientAssemblyObfuscatorTests
{
    private const string Source = """
        namespace Probe;

        public static class T
        {
            public static string Secret() => "hello secret value";

            public static string Concatenated() => string.Concat("alpha", "beta");
        }
        """;

    [Fact]
    public void ObfuscationHidesLiteralsButPreservesRuntimeValues()
    {
        var directory = Directory.CreateTempSubdirectory("nanossharp-obf-");
        try
        {
            var file = Path.Combine(directory.FullName, "T.cs");
            File.WriteAllText(file, Source);
            var result = ScriptCompiler.Compile("Probe", [file], [], optimize: true);
            Assert.True(result.Succeeded, string.Join("\n", result.Diagnostics));

            var obfuscated = ClientAssemblyObfuscator.Obfuscate(result.Assembly!);

            Assert.True(result.Assembly!.AsSpan().IndexOf(Encoding.Unicode.GetBytes("hello secret value")) >= 0);
            Assert.True(obfuscated.AsSpan().IndexOf(Encoding.Unicode.GetBytes("hello secret value")) < 0);

            var context = new AssemblyLoadContext("obfuscation-test", isCollectible: true);
            try
            {
                var type = context.LoadFromStream(new MemoryStream(obfuscated)).GetType("Probe.T");
                Assert.NotNull(type);
                Assert.Equal("hello secret value", type!.GetMethod("Secret")!.Invoke(null, null));
                Assert.Equal("alphabeta", type.GetMethod("Concatenated")!.Invoke(null, null));
            }
            finally
            {
                context.Unload();
            }
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }
}
