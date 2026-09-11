namespace NanosSharp.Host.Compilation;

internal sealed record CompilationResult(byte[]? Assembly, byte[]? Symbols, IReadOnlyList<string> Diagnostics)
{
    public bool Succeeded => Assembly is not null;
}
