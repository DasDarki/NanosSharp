namespace NanosSharp.Generator.Native;

internal sealed class NativeClassDump
{
    public string Name { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;

    public string? Constructor { get; set; }

    public List<NativeMethodDump> Methods { get; set; } = [];

    public List<NativeMethodDump> StaticMethods { get; set; } = [];
}
