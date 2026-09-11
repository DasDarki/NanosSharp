namespace NanosSharp.Packages;

/// <summary>Base class for C# package entry points, every non abstract subclass is started with its package.</summary>
public abstract class Package
{
    /// <summary>Name of the package.</summary>
    public string Name => Scope.Name;

    /// <summary>Absolute path of the package directory.</summary>
    public string Directory => Scope.Directory;

    internal PackageScope Scope { get; set; } = null!;

    /// <summary>Called on the main thread after the package was loaded.</summary>
    protected internal virtual void OnStart()
    {
    }

    /// <summary>Called on the main thread before the package is unloaded.</summary>
    protected internal virtual void OnStop()
    {
    }
}
