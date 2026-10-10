using System.Text.Json;

namespace NanosSharp.Generator.Definitions;

internal sealed class DocsCatalog
{
    private const string SharedFunctionsFile = "SharedFunctions.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public required IReadOnlyDictionary<string, DocsClass> Classes { get; init; }

    public required IReadOnlyDictionary<string, DocsClass> StaticClasses { get; init; }

    public required IReadOnlyDictionary<string, DocsEnum> Enums { get; init; }

    public IReadOnlyList<DocsFunction> SharedFunctions { get; init; } = [];

    public DocsClass? FindClass(string name) => Classes.GetValueOrDefault(name) ?? StaticClasses.GetValueOrDefault(name);

    public bool IsStaticClass(string name) => StaticClasses.ContainsKey(name);

    public DocsFunction? FindBorrowedFunction(string name, int parameterCount) =>
        SharedFunctions
            .Concat(Classes.OrderBy(entry => entry.Key, StringComparer.Ordinal).SelectMany(entry => entry.Value.Functions))
            .FirstOrDefault(function => function.Name == name && function.Parameters.Count == parameterCount);

    public static DocsCatalog Load(string directory, string? supplementDirectory)
    {
        using var index = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, "APIFiles.json")));
        var classes = LoadSection(directory, index.RootElement, "Classes");
        var staticClasses = LoadSection(directory, index.RootElement, "StaticClasses");
        foreach (var utilityClass in LoadSection(directory, index.RootElement, "UtilityClasses").Values)
        {
            staticClasses.TryAdd(utilityClass.Name, utilityClass);
        }

        if (supplementDirectory is not null)
        {
            MergeSupplements(classes, Path.Combine(supplementDirectory, "Classes"));
            MergeSupplements(staticClasses, Path.Combine(supplementDirectory, "StaticClasses"));
        }

        ResolveDocumentationCopies(classes);
        var sharedPath = supplementDirectory is null ? null : Path.Combine(supplementDirectory, SharedFunctionsFile);
        return new DocsCatalog
        {
            SharedFunctions = sharedPath is not null && File.Exists(sharedPath) ? Read<DocsClass>(sharedPath).Functions : [],
            Classes = classes,
            StaticClasses = staticClasses,
            Enums = Read<Dictionary<string, DocsEnum>>(Path.Combine(directory, "Enums.json")),
        };
    }

    private static Dictionary<string, DocsClass> LoadSection(string directory, JsonElement index, string section) =>
        index.GetProperty(section).EnumerateObject()
            .ToDictionary(entry => entry.Name, entry => Read<DocsClass>(Path.Combine(directory, section, entry.Value.GetString()!)),
                StringComparer.Ordinal);

    private static void MergeSupplements(Dictionary<string, DocsClass> classes, string directory)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        foreach (var path in Directory.GetFiles(directory, "*.json").Order(StringComparer.Ordinal))
        {
            var supplement = Read<DocsClass>(path);
            if (!classes.TryGetValue(supplement.Name, out var existing))
            {
                classes[supplement.Name] = supplement;
                continue;
            }

            AddMissing(existing.Constructors, supplement.Constructors, function => function.Name);
            AddMissing(existing.Functions, supplement.Functions, function => function.Name);
            AddMissing(existing.StaticFunctions, supplement.StaticFunctions, function => function.Name);
            AddMissing(existing.Events, supplement.Events, docsEvent => docsEvent.Name);
        }
    }

    private static void ResolveDocumentationCopies(Dictionary<string, DocsClass> classes)
    {
        foreach (var docsClass in classes.Values.Where(docsClass => docsClass.DocumentationOf is not null))
        {
            var source = classes.GetValueOrDefault(docsClass.DocumentationOf!) ??
                throw new InvalidDataException($"{docsClass.Name} copies the unknown class {docsClass.DocumentationOf}.");
            docsClass.Description ??= source.Description;
            docsClass.Inheritance = docsClass.Inheritance.Count > 0 ? docsClass.Inheritance : [.. source.Inheritance];
            AddMissing(docsClass.Constructors, source.Constructors, function => function.Name);
            AddMissing(docsClass.Functions, source.Functions, function => function.Name);
            AddMissing(docsClass.StaticFunctions, source.StaticFunctions, function => function.Name);
            AddMissing(docsClass.Events, source.Events, docsEvent => docsEvent.Name);
        }
    }

    private static void AddMissing<T>(List<T> target, IEnumerable<T> additions, Func<T, string> name)
    {
        var existing = target.Select(name).ToHashSet(StringComparer.Ordinal);
        target.AddRange(additions.Where(addition => !existing.Contains(name(addition))));
    }

    private static T Read<T>(string path) =>
        JsonSerializer.Deserialize<T>(File.ReadAllText(path), Options) ?? throw new InvalidDataException($"{path} is empty.");
}
