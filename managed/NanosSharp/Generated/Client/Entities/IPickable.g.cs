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
}
