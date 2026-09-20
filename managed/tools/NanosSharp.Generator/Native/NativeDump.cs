using System.Text.Json;

namespace NanosSharp.Generator.Native;

internal sealed class NativeDump
{
    public string ServerVersion { get; set; } = string.Empty;

    public List<NativeClassDump> Classes { get; set; } = [];

    public static NativeDump Load(string path) =>
        JsonSerializer.Deserialize<NativeDump>(File.ReadAllText(path)) ?? throw new InvalidDataException($"{path} is empty.");
}
