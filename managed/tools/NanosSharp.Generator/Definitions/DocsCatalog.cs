using System.Text.Json;

namespace NanosSharp.Generator.Definitions;

internal sealed class DocsCatalog
{
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

    public DocsClass? FindClass(string name) => Classes.GetValueOrDefault(name) ?? StaticClasses.GetValueOrDefault(name);

    public bool IsStaticClass(string name) => StaticClasses.ContainsKey(name);

    public static DocsCatalog Load(string directory)
    {
        using var index = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, "APIFiles.json")));
        return new DocsCatalog
        {
            Classes = LoadSection(directory, index.RootElement, "Classes"),
            StaticClasses = LoadSection(directory, index.RootElement, "StaticClasses"),
            Enums = Read<Dictionary<string, DocsEnum>>(Path.Combine(directory, "Enums.json")),
        };
    }

    private static Dictionary<string, DocsClass> LoadSection(string directory, JsonElement index, string section) =>
        index.GetProperty(section).EnumerateObject()
            .ToDictionary(entry => entry.Name, entry => Read<DocsClass>(Path.Combine(directory, section, entry.Value.GetString()!)),
                StringComparer.Ordinal);

    private static T Read<T>(string path) =>
        JsonSerializer.Deserialize<T>(File.ReadAllText(path), Options) ?? throw new InvalidDataException($"{path} is empty.");
}
