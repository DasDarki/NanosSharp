namespace NanosSharp.Generator.Mapping;

internal sealed class GeneratorReport
{
    private readonly SortedDictionary<string, SortedSet<string>> _sections = new(StringComparer.Ordinal);

    public void Add(string section, string entry)
    {
        if (!_sections.TryGetValue(section, out var entries))
        {
            entries = new SortedSet<string>(StringComparer.Ordinal);
            _sections[section] = entries;
        }

        entries.Add(entry);
    }

    public string Render(string serverVersion, string docsRevision)
    {
        var lines = new List<string>
        {
            "# NanosSharp API generator report",
            string.Empty,
            $"Server version: {serverVersion}",
            $"API definitions: {docsRevision}",
        };

        foreach (var (section, entries) in _sections)
        {
            lines.Add(string.Empty);
            lines.Add($"## {section} ({entries.Count})");
            lines.Add(string.Empty);
            lines.AddRange(entries.Select(entry => $"- {entry}"));
        }

        return string.Join('\n', lines) + "\n";
    }
}
