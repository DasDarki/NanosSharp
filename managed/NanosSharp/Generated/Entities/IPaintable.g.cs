#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Base class for all Paintable entities. This class provides customization for materials, exposing common functions to allow you to set custom material parameters, including loading textures from disk.</summary>
public partial interface IPaintable
{
    /// <summary>Gets a Color parameter from this Actor's material</summary>
    Color? GetMaterialColorParameter(string parameterName, int? index = null, string? attachableId = null);

    /// <summary>Gets a Scalar parameter from this Actor's material</summary>
    float? GetMaterialScalarParameter(string parameterName, int? index = null, string? attachableId = null);

    /// <summary>Gets a Texture parameter from this Actor's material</summary>
    string? GetMaterialTextureParameter(string parameterName, int? index = null, string? attachableId = null);

    /// <summary>Gets a Vector parameter from this Actor's material</summary>
    Vector? GetMaterialVectorParameter(string parameterName, int? index = null, string? attachableId = null);

    /// <summary>Resets the material from the specified index to the original one</summary>
    void ResetMaterial(int? index = null, string? attachableId = null);

    /// <summary>Sets the material at the specified index of this Actor</summary>
    void SetMaterial(string materialPath, int? index = null, string? attachableId = null);

    /// <summary>Sets a Color parameter in this Actor's material</summary>
    void SetMaterialColorParameter(string parameterName, Color color, int? index = null, string? attachableId = null);

    /// <summary>Sets a Scalar parameter in this Actor's material</summary>
    void SetMaterialScalarParameter(string parameterName, float scalar, int? index = null, string? attachableId = null);

    /// <summary>Sets a texture parameter in this Actor's material to an image</summary>
    void SetMaterialTextureParameter(string parameterName, string texturePath, int? index = null, string? attachableId = null);

    /// <summary>Sets a Vector parameter in this Actor's material</summary>
    void SetMaterialVectorParameter(string parameterName, Vector vector, int? index = null, string? attachableId = null);

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    void SetPhysicalMaterial(string physicalMaterialPath);
}
