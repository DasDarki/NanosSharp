using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NanosSharp.Packages;

/// <summary>
/// Engine wide registry mapping a contract type to the package instance that provides it. It lives in the shared
/// NanosSharp assembly, so every package load context and the client sandbox runtime see the same registry; it
/// holds only managed instances and resolves purely in managed code, so it adds no ambient authority on the
/// client. Contracts keep a single <see cref="Type"/> identity across packages because the dependency's own
/// loaded assembly is shared with its dependents.
/// </summary>
internal static class PackageApiRegistry
{
    private readonly record struct Entry(object Instance, string Owner);

    private static readonly object Gate = new();
    private static readonly Dictionary<Type, Entry> Entries = [];

    public static void Provide(Type contract, object instance)
    {
        var owner = PackageContext.Current?.Name
            ?? throw new InvalidOperationException("PackageApi.Provide can only be called from a running package.");
        Publish(owner, contract, instance);
    }

    public static void Publish(string owner, Type contract, object instance)
    {
        lock (Gate)
        {
            if (Entries.TryGetValue(contract, out var existing) && existing.Owner != owner)
            {
                throw new InvalidOperationException(
                    $"The package API {contract.FullName} is already provided by package '{existing.Owner}'.");
            }

            Entries[contract] = new Entry(instance, owner);
        }
    }

    public static object? Resolve(Type contract)
    {
        lock (Gate)
        {
            return Entries.TryGetValue(contract, out var entry) ? entry.Instance : null;
        }
    }

    public static void ReleaseScope(string owner)
    {
        lock (Gate)
        {
            foreach (var contract in Entries.Where(pair => pair.Value.Owner == owner).Select(pair => pair.Key).ToArray())
            {
                Entries.Remove(contract);
            }
        }
    }

    [RequiresUnreferencedCode("Reads the contract interfaces of package API types through reflection.")]
    public static void PublishDeclared(string owner, IEnumerable<Type> types, Func<Type, object?> resolveInstance)
    {
        foreach (var type in types)
        {
            if (type.GetCustomAttribute<PackageApiAttribute>() is not { } attribute || resolveInstance(type) is not { } instance)
            {
                continue;
            }

            foreach (var contract in ResolveContracts(type, attribute))
            {
                Publish(owner, contract, instance);
            }
        }
    }

    [RequiresUnreferencedCode("Reads the contract interfaces of package API types through reflection.")]
    private static IEnumerable<Type> ResolveContracts(Type type, PackageApiAttribute attribute)
    {
        yield return type;
        if (attribute.Contracts.Count > 0)
        {
            foreach (var contract in attribute.Contracts)
            {
                yield return contract;
            }

            yield break;
        }

        foreach (var contract in type.GetInterfaces().Where(IsContractInterface))
        {
            yield return contract;
        }
    }

    private static bool IsContractInterface(Type type) =>
        type.IsPublic && type.Namespace is { } @namespace &&
        !@namespace.StartsWith("System", StringComparison.Ordinal) &&
        !@namespace.StartsWith("NanosSharp", StringComparison.Ordinal);
}
