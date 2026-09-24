#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the server is restarting</summary>
public sealed class ServerRestartEvent : ScriptEvent
{
}
