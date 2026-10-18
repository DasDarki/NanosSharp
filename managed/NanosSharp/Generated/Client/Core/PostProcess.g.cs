#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Interaction with Post Process effects.</summary>
public static partial class PostProcess
{
    private static readonly StaticMethodSlot RemoveMaterialSlot = new("PostProcess", "RemoveMaterial");
    private static readonly StaticMethodSlot SetBloomSlot = new("PostProcess", "SetBloom");
    private static readonly StaticMethodSlot SetChromaticAberrationSlot = new("PostProcess", "SetChromaticAberration");
    private static readonly StaticMethodSlot SetExposureSlot = new("PostProcess", "SetExposure");
    private static readonly StaticMethodSlot SetFilmSlot = new("PostProcess", "SetFilm");
    private static readonly StaticMethodSlot SetGlobalContrastSlot = new("PostProcess", "SetGlobalContrast");
    private static readonly StaticMethodSlot SetGlobalGainSlot = new("PostProcess", "SetGlobalGain");
    private static readonly StaticMethodSlot SetGlobalGammaSlot = new("PostProcess", "SetGlobalGamma");
    private static readonly StaticMethodSlot SetGlobalOffsetSlot = new("PostProcess", "SetGlobalOffset");
    private static readonly StaticMethodSlot SetGlobalSaturationSlot = new("PostProcess", "SetGlobalSaturation");
    private static readonly StaticMethodSlot SetImageEffectsSlot = new("PostProcess", "SetImageEffects");
    private static readonly StaticMethodSlot SetLookupTableSlot = new("PostProcess", "SetLookupTable");
    private static readonly StaticMethodSlot SetMaterialSlot = new("PostProcess", "SetMaterial");

    /// <summary>Removes the current Post Process Material</summary>
    public static void RemoveMaterial() => ScriptApi.Call(RemoveMaterialSlot, 0);

    /// <summary>Set Post Process Bloom Settings</summary>
    public static void SetBloom(float? intensity = null, float? threshold = null) =>
        ScriptApi.Call(SetBloomSlot, 0, intensity, threshold);

    /// <summary>Set Post Process Chromatic Aberration Settings</summary>
    public static void SetChromaticAberration(float? intensity = null, float? startOffset = null) =>
        ScriptApi.Call(SetChromaticAberrationSlot, 0, intensity, startOffset);

    /// <summary>Set Post Process Exposure Settings</summary>
    public static void SetExposure(
        float? exposureCompensation = null,
        float? minEv100 = null,
        float? maxEv100 = null,
        float? lowPercent = null,
        float? highPercent = null) =>
        ScriptApi.Call(SetExposureSlot, 0, exposureCompensation, minEv100, maxEv100, lowPercent, highPercent);

    /// <summary>Set Post Process Film Settings</summary>
    public static void SetFilm(
        float? slope = null,
        float? toe = null,
        float? shoulder = null,
        float? blackClip = null,
        float? whiteClip = null) =>
        ScriptApi.Call(SetFilmSlot, 0, slope, toe, shoulder, blackClip, whiteClip);

    /// <summary>Set Post Process Global Contrast</summary>
    public static void SetGlobalContrast(Color contrast) => ScriptApi.Call(SetGlobalContrastSlot, 1, contrast);

    /// <summary>Set Post Process Global Gain</summary>
    public static void SetGlobalGain(Color gain) => ScriptApi.Call(SetGlobalGainSlot, 1, gain);

    /// <summary>Set Post Process Global Gamma</summary>
    public static void SetGlobalGamma(Color gamma) => ScriptApi.Call(SetGlobalGammaSlot, 1, gamma);

    /// <summary>Set Post Process Global Offset</summary>
    public static void SetGlobalOffset(Color offset) => ScriptApi.Call(SetGlobalOffsetSlot, 1, offset);

    /// <summary>Set Post Process Saturation Colors. Use Alpha for overall Saturation intensity</summary>
    public static void SetGlobalSaturation(Color color) => ScriptApi.Call(SetGlobalSaturationSlot, 1, color);

    /// <summary>Set Post Process Image Effect Settings</summary>
    public static void SetImageEffects(float? vignetteIntensity = null, float? filmGrainIntensity = null) =>
        ScriptApi.Call(SetImageEffectsSlot, 0, vignetteIntensity, filmGrainIntensity);

    /// <summary>Set Post Process Lookup Table (LUT) Texture</summary>
    public static void SetLookupTable(string texturePath) => ScriptApi.Call(SetLookupTableSlot, 1, texturePath);

    /// <summary>Sets a Post Process Material</summary>
    public static void SetMaterial(string materialPath) => ScriptApi.Call(SetMaterialSlot, 1, materialPath);
}
