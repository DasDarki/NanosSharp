#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when any package is unloaded</summary>
public sealed class ServerPackageUnloadEvent : ScriptEvent
{
    /// <summary>The package name (its folder name)</summary>
    public string? PackageName { get; init; }
}
