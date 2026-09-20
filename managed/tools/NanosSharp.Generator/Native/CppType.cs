namespace NanosSharp.Generator.Native;

internal sealed record CppType(string Name, IReadOnlyList<CppType> Arguments, bool IsPointer, bool IsReference)
{
    public CppType Argument(int index) => Arguments[index];

    public bool Is(string name) => Name == name;

    public override string ToString() =>
        Arguments.Count == 0 ? Name : $"{Name}<{string.Join(", ", Arguments)}>";
}
