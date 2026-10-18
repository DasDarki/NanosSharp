#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Value is changed with <c>Client.SetValue()</c> or <c>Server.SetValue()</c> (for synced values)</summary>
public sealed class ClientValueChangeEvent : ScriptEvent
{
    /// <summary>The key that changed</summary>
    public string? Key { get; init; }

    /// <summary>The new value</summary>
    public object? Value { get; init; }
}
