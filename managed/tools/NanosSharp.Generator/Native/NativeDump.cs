using System.Text.Json;

namespace NanosSharp.Generator.Native;

internal sealed class NativeDump
{
    private const string MsvcMarker = "__ptr64";

    public string ServerVersion { get; set; } = string.Empty;

    public List<NativeClassDump> Classes { get; set; } = [];

    public bool IsMsvc => Classes.SelectMany(nativeClass => nativeClass.Methods).Any(method => method.Signature.Contains(MsvcMarker));

    public static NativeDump Load(string path) =>
        JsonSerializer.Deserialize<NativeDump>(File.ReadAllText(path)) ?? throw new InvalidDataException($"{path} is empty.");
}
