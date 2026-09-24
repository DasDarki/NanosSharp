#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered once when the server has started and all packages are loaded</summary>
public sealed class ServerStartEvent : ScriptEvent
{
}
