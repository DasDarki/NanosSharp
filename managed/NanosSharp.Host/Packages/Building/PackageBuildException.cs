namespace NanosSharp.Host.Packages.Building;

internal sealed class PackageBuildException(string message, bool isTransient = false) : Exception(message)
{
    public bool IsTransient { get; } = isTransient;
}
