namespace NanosSharp.Host.Packages.Building;

internal sealed record AssemblyImage(string Name, byte[] Image, byte[]? Symbols);
