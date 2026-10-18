#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
public sealed class Widget3DValueChangeEvent : ScriptEvent
{
    /// <summary>The Entity that just had a value changed</summary>
    public Widget3D Widget3D { get; init; } = null!;

    /// <summary>The key used</summary>
    public string? Key { get; init; }

    /// <summary>The value that was set</summary>
    public object? Value { get; init; }
}
