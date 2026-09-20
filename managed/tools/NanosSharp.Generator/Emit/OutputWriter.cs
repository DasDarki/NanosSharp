using NanosSharp.Generator.Model;

namespace NanosSharp.Generator.Emit;

internal static class OutputWriter
{
    private const string GeneratedExtension = ".g.cs";

    public static int Write(ApiModel model, string outputDirectory)
    {
        var files = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var type in model.Types)
        {
            var folder = type.Namespace.Split('.').Last();
            files[Path.Combine(folder, type.Name + GeneratedExtension)] = TypeEmitter.Emit(type);
        }

        foreach (var enumModel in model.Enums)
        {
            files[Path.Combine("Enums", enumModel.Name + GeneratedExtension)] = EnumEmitter.Emit(enumModel);
        }

        RemoveStaleFiles(outputDirectory, files.Keys);
        foreach (var (relativePath, content) in files)
        {
            var path = Path.Combine(outputDirectory, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            WriteIfChanged(path, content);
        }

        return files.Count;
    }

    private static void RemoveStaleFiles(string outputDirectory, IEnumerable<string> expected)
    {
        if (!Directory.Exists(outputDirectory))
        {
            return;
        }

        var expectedPaths = expected.Select(path => Path.GetFullPath(Path.Combine(outputDirectory, path))).ToHashSet(StringComparer.Ordinal);
        var stale = Directory.EnumerateFiles(outputDirectory, "*" + GeneratedExtension, SearchOption.AllDirectories)
            .Where(path => !expectedPaths.Contains(Path.GetFullPath(path)));
        foreach (var path in stale.ToArray())
        {
            File.Delete(path);
        }
    }

    private static void WriteIfChanged(string path, string content)
    {
        if (File.Exists(path) && File.ReadAllText(path) == content)
        {
            return;
        }

        File.WriteAllText(path, content);
    }
}
