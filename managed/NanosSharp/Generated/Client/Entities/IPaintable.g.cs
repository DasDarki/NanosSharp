#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial interface IPaintable
{

    /// <summary>Sets the material at the specified index of this Actor to a Canvas object</summary>
    void SetMaterialFromCanvas(Entity canvas, int? index = null, string? argument3 = null);

    /// <summary>Sets the material at the specified index of this Actor to a SceneCapture object</summary>
    void SetMaterialFromSceneCapture(Entity sceneCapture, int? index = null, string? argument3 = null);

    /// <summary>Sets the material at the specified index of this Actor to a WebUI object</summary>
    void SetMaterialFromWebUI(Entity webui, int? index = null, string? argument3 = null);
}
