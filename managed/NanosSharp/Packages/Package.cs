using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop;
using NanosSharp.Interop.Reflection;
using NanosSharp.Interop.Values;

namespace NanosSharp.Packages;

/// <summary>Base class for C# package entry points, every non abstract subclass is started with its package.</summary>
public abstract unsafe class Package
{
    private const string ScriptClassName = "Package_S";

    /// <summary>Name of the package.</summary>
    public string Name => Scope.Name;

    /// <summary>Absolute path of the package directory.</summary>
    public string Directory => Scope.Directory;

    /// <summary>Title from the Package.toml.</summary>
    public string Title => ScriptEntity.Invoke<string>("GetTitle")!;

    /// <summary>Version from the Package.toml.</summary>
    public string Version => ScriptEntity.Invoke<string>("GetVersion")!;

    /// <summary>Compatibility version from the Package.toml.</summary>
    public string CompatibilityVersion => ScriptEntity.Invoke<string>("GetCompatibilityVersion")!;

    /// <summary>Type of the package.</summary>
    public PackageType PackageType => ScriptEntity.Invoke<PackageType>("GetType");

    /// <summary>Whether the package is currently unloading.</summary>
    public bool IsUnloading => ScriptEntity.Invoke<bool>("IsUnloading");

    internal PackageScope Scope { get; set; } = null!;

    private Entity? _scriptEntity;

    private Entity ScriptEntity
    {
        get
        {
            if (_scriptEntity is { IsValid: true })
            {
                return _scriptEntity;
            }

            _scriptEntity = ResolveScriptEntity() ?? throw new InvalidOperationException($"Package {Name} has no scripting entity.");
            return _scriptEntity;
        }
    }

    private Entity? ResolveScriptEntity()
    {
        var classId = NativeClassRegistry.Get(ScriptClassName).Id;
        var instance = NativeRuntime.Api.PackageGetEntity(Scope.Handle);
        if (instance != 0)
        {
            return EntityRegistry.Resolve(classId, instance);
        }

        return EntityRegistry.All.FirstOrDefault(entity =>
            entity.IsValid && entity.ClassName == ScriptClassName && entity.Invoke<string>("GetName") == Name);
    }

    /// <summary>Lists files relative to the package, optionally inside a folder and filtered by extension like ".cs".</summary>
    public IReadOnlyList<string> GetFiles(string? path = null, string? extension = null) =>
        List(path, folder => System.IO.Directory.EnumerateFiles(folder, "*" + extension, SearchOption.AllDirectories));

    /// <summary>Lists directories relative to the package, optionally inside a folder.</summary>
    public IReadOnlyList<string> GetDirectories(string? path = null) =>
        List(path, folder => System.IO.Directory.EnumerateDirectories(folder, "*", SearchOption.AllDirectories));

    /// <summary>Reads all persistent data of the package.</summary>
    public IReadOnlyDictionary<object, object?> GetPersistentData() =>
        ScriptEntity.Invoke<Dictionary<object, object?>>("GetPersistentData_I") ?? [];

    /// <summary>Reads one persistent value of the package.</summary>
    public T? GetPersistentData<T>(string key) =>
        GetPersistentData().TryGetValue(key, out var value) ? ValueConverter.Convert<T>(value) : default;

    /// <summary>Stores a persistent value, it is written to disk by nanos world.</summary>
    public void SetPersistentData(string key, object? value) => ScriptEntity.Invoke("SetPersistentData_I", key, value);

    /// <summary>Writes the persistent data to disk immediately.</summary>
    public void FlushPersistentData() => ScriptEntity.Invoke("FlushPersistentData");

    /// <summary>Called on the main thread after the package was loaded.</summary>
    protected internal virtual void OnStart()
    {
    }

    /// <summary>Called on the main thread before the package is unloaded.</summary>
    protected internal virtual void OnStop()
    {
    }

    private string[] List(string? path, Func<string, IEnumerable<string>> enumerate)
    {
        var folder = Path.GetFullPath(Path.Combine(Directory, path ?? string.Empty));
        if (!System.IO.Directory.Exists(folder))
        {
            return [];
        }

        return enumerate(folder)
            .Select(entry => Path.GetRelativePath(Directory, entry).Replace(Path.DirectorySeparatorChar, '/'))
            .Order(StringComparer.Ordinal)
            .ToArray();
    }
}
