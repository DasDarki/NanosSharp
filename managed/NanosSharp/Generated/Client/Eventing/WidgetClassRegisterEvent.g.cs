#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
public sealed class WidgetClassRegisterEvent : ScriptEvent
{
    /// <summary>The inherited Class</summary>
    public object? Class { get; init; }
}
