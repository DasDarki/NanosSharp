#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when the local player mutes or unmutes their own microphone, either with the VOIP key (on Open Mic) or from the escape menu</summary>
public sealed class ClientVOIPSelfMuteChangeEvent : ScriptEvent
{
    /// <summary>Whether the microphone is now muted</summary>
    public bool IsMuted { get; init; }
}
