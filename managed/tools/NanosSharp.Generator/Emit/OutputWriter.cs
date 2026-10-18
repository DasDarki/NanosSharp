using NanosSharp.Generator.Model;

namespace NanosSharp.Generator.Emit;

internal static class OutputWriter
{
    private const string GeneratedExtension = ".g.cs";
    private const string SharedFolder = "Shared";
    private const string ServerFolder = "Server";
    private const string ClientFolder = "Client";
    private const string SharedCatalogMethod = "AddSharedDescriptors";
    private const string SideCatalogMethod = "AddSideDescriptors";

    public static int Write(PartitionedApi api, string outputDirectory)
    {
        var files = new SortedDictionary<string, string>(StringComparer.Ordinal);
        AddPartition(files, SharedFolder, api.Shared, SharedCatalogMethod);
        AddPartition(files, ServerFolder, api.Server, SideCatalogMethod);
        AddPartition(files, ClientFolder, api.Client, SideCatalogMethod);

        RemoveStaleFiles(outputDirectory, files.Keys);
        foreach (var (relativePath, content) in files)
        {
            var path = Path.Combine(outputDirectory, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            WriteIfChanged(path, content);
        }

        return files.Count;
    }

    private static void AddPartition(SortedDictionary<string, string> files, string partition, ApiModel model, string catalogMethod)
    {
        foreach (var type in model.Types)
        {
            var folder = type.Namespace.Split('.').Last();
            files[Path.Combine(partition, folder, type.Name + GeneratedExtension)] = TypeEmitter.Emit(type);
        }

        foreach (var (owner, eventModel) in EventClassEmitter.Collect(model))
        {
            files[Path.Combine(partition, "Eventing", EventClassEmitter.ClassName(owner, eventModel) + GeneratedExtension)] =
                EventClassEmitter.EmitEvent(owner, eventModel);
        }

        files[Path.Combine(partition, "Eventing", "ScriptEventCatalog" + GeneratedExtension)] =
            EventClassEmitter.EmitCatalog(model, catalogMethod);

        foreach (var enumModel in model.Enums)
        {
            files[Path.Combine(partition, "Enums", enumModel.Name + GeneratedExtension)] = EnumEmitter.Emit(enumModel);
        }
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

        var directories = Directory.EnumerateDirectories(outputDirectory, "*", SearchOption.AllDirectories)
            .OrderByDescending(directory => directory.Length);
        foreach (var directory in directories.ToArray())
        {
            if (!Directory.EnumerateFileSystemEntries(directory).Any())
            {
                Directory.Delete(directory);
            }
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
