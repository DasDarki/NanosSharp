#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial interface IVehicle
{

    /// <summary>Checks if the engine is started</summary>
    bool EngineStarted { get; }
}
