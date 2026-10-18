#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when any package is loaded</summary>
public sealed class ClientPackageLoadEvent : ScriptEvent
{
    /// <summary>The package name (its folder name)</summary>
    public string? PackageName { get; init; }
}
