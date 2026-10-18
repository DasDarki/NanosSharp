#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial interface IPickable
{

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    HandlingMode HandlingMode { get; }

    /// <summary>Pulls the usage of this Pickable (will start firing if this is a weapon)</summary>
    void PullUse(float? releaseUseAfter = null);

    /// <summary>Releases the usage of this Pickable (will stop firing if this is a weapon)</summary>
    void ReleaseUse();

    /// <summary>Sets if this Pickable can be picked up from ground by the player</summary>
    void SetPickable(bool isPickable);
}
