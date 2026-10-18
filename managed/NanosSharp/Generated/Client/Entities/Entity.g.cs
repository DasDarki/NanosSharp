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

    /// <summary>Calls a custom remote event directly on this entity</summary>
    public void CallRemoteEvent(string eventName, Reliability? reliability = null, params object?[] args) =>
        InvokeVariadic(CallRemoteEventSlot, [eventName, reliability], args);

    /// <summary>Gets if the local context has authority over this Entity (true if spawned by the client, false if spawned by the server)</summary>
    public bool HasAuthority() => CallResult<bool>(HasAuthoritySlot, 0)!;
}
