using NanosSharp.Generator.Definitions;
using NanosSharp.Generator.Emit;
using NanosSharp.Generator.Mapping;
using NanosSharp.Generator.Model;
using NanosSharp.Generator.Native;

namespace NanosSharp.Generator;

internal static class Program
{
    private static int Main(string[] arguments)
    {
        var options = ParseOptions(arguments);
        if (!options.TryGetValue("native", out var nativePath) || !options.TryGetValue("native-client", out var clientNativePath) ||
            !options.TryGetValue("docs", out var docsDirectory) || !options.TryGetValue("output", out var outputDirectory))
        {
            Console.Error.WriteLine("usage: NanosSharp.Generator --native <server api.json> --native-client <client api.json> --docs <api repository> --output <directory> [--supplements <directory>] [--report <file>] [--revision <commit>]");
            return 1;
        }

        var report = new GeneratorReport();
        var native = NativeDump.Load(nativePath);
        var clientNative = NativeDump.Load(clientNativePath);
        var docs = DocsCatalog.Load(docsDirectory, options.GetValueOrDefault("supplements"));
        var serverModel = new ApiModelBuilder(native, ApiSide.Server, docs, report).Build();
        var clientModel = new ApiModelBuilder(clientNative, ApiSide.Client, docs, report).Build();
        var fileCount = OutputWriter.Write(ApiMerger.Merge(serverModel, clientModel), outputDirectory);

        if (options.TryGetValue("report", out var reportPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(reportPath))!);
            File.WriteAllText(reportPath, report.Render(native.ServerVersion, options.GetValueOrDefault("revision", "unknown")));
        }

        Console.WriteLine($"Generated {fileCount} files for server {native.ServerVersion} into {outputDirectory}.");
        return 0;
    }

    private static Dictionary<string, string> ParseOptions(string[] arguments)
    {
        var options = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var index = 0; index + 1 < arguments.Length; index += 2)
        {
            options[arguments[index].TrimStart('-')] = arguments[index + 1];
        }

        return options;
    }
}
