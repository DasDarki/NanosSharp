#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Configure, send and intercept chat messages.</summary>
public static partial class Chat
{
    /// <summary>Called when a player submits a message in the chat</summary>
    public static event PlayerSubmitHandler? OnPlayerSubmit
    {
        add => PlayerSubmitEvent.Add(value);
        remove => PlayerSubmitEvent.Remove(value);
    }

    private static readonly ClassEvent<PlayerSubmitHandler> PlayerSubmitEvent = new("Chat", "PlayerSubmit");

    /// <summary>Called when a player submits a message in the chat</summary>
    public delegate bool PlayerSubmitHandler(string? message, Player? player);
}
