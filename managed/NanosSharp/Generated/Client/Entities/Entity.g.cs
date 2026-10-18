#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Entity
{
    private static readonly MethodSlot CallRemoteEventSlot = new("CallRemoteEvent");
    private static readonly MethodSlot HasAuthoritySlot = new("HasAuthority");

    /// <summary>Calls a custom remote event directly on this entity to a specific Player</summary>
    public void CallRemoteEvent(string eventName, Reliability? player = null, params object?[] reliability) =>
        InvokeVariadic(CallRemoteEventSlot, [eventName, player], reliability);

    /// <summary>Gets if the local context has authority over this Entity (true if spawned by the client, false if spawned by the server)</summary>
    public bool HasAuthority() => CallResult<bool>(HasAuthoritySlot, 0)!;
}
