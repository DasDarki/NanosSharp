#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Interact with built-in Sky &amp; Weather system.</summary>
public static partial class Sky
{
    private static readonly StaticMethodSlot ChangeWeatherSlot = new("Sky", "ChangeWeather");
    private static readonly StaticMethodSlot DestroyAllSkySlot = new("Sky", "DestroyAllSky");
    private static readonly StaticMethodSlot GetTimeOfDaySlot = new("Sky", "GetTimeOfDay");
    private static readonly StaticMethodSlot GetWeatherSlot = new("Sky", "GetWeather");
    private static readonly StaticMethodSlot IsSpawnedSlot = new("Sky", "IsSpawned");
    private static readonly StaticMethodSlot ReconstructSlot = new("Sky", "Reconstruct");
    private static readonly StaticMethodSlot SetAnimateTimeOfDaySlot = new("Sky", "SetAnimateTimeOfDay");
    private static readonly StaticMethodSlot SetCloudCoverageSlot = new("Sky", "SetCloudCoverage");
    private static readonly StaticMethodSlot SetContrastSlot = new("Sky", "SetContrast");
    private static readonly StaticMethodSlot SetFogSlot = new("Sky", "SetFog");
    private static readonly StaticMethodSlot SetMoonAngleSlot = new("Sky", "SetMoonAngle");
    private static readonly StaticMethodSlot SetMoonGlowIntensitySlot = new("Sky", "SetMoonGlowIntensity");
    private static readonly StaticMethodSlot SetMoonLightIntensitySlot = new("Sky", "SetMoonLightIntensity");
    private static readonly StaticMethodSlot SetMoonPhaseSlot = new("Sky", "SetMoonPhase");
    private static readonly StaticMethodSlot SetMoonScaleSlot = new("Sky", "SetMoonScale");
    private static readonly StaticMethodSlot SetMoonTextureSlot = new("Sky", "SetMoonTexture");
    private static readonly StaticMethodSlot SetMoonTextureRotationSlot = new("Sky", "SetMoonTextureRotation");
    private static readonly StaticMethodSlot SetMoonVerticalOffsetSlot = new("Sky", "SetMoonVerticalOffset");
    private static readonly StaticMethodSlot SetNightBrightnessSlot = new("Sky", "SetNightBrightness");
    private static readonly StaticMethodSlot SetOverallIntensitySlot = new("Sky", "SetOverallIntensity");
    private static readonly StaticMethodSlot SetSkyModeSlot = new("Sky", "SetSkyMode");
    private static readonly StaticMethodSlot SetSunAngleSlot = new("Sky", "SetSunAngle");
    private static readonly StaticMethodSlot SetSunLightIntensitySlot = new("Sky", "SetSunLightIntensity");
    private static readonly StaticMethodSlot SetSunScaleSlot = new("Sky", "SetSunScale");
    private static readonly StaticMethodSlot SetTimeOfDaySlot = new("Sky", "SetTimeOfDay");
    private static readonly StaticMethodSlot SetVolumetricCloudColorSlot = new("Sky", "SetVolumetricCloudColor");
    private static readonly StaticMethodSlot SpawnSlot = new("Sky", "Spawn");

    /// <summary>Gets the current time</summary>
    public static object?[] TimeOfDay => GetTimeOfDay();

    /// <summary>Gets the current weather</summary>
    public static WeatherType Weather => GetWeather();

    /// <summary>Changes the Weather</summary>
    public static void ChangeWeather(WeatherType weather, double transitionTime) =>
        ScriptApi.Call(ChangeWeatherSlot, 2, weather, transitionTime);

    /// <summary>Destroys all Directional Lights, Sky Lights, Exponential Height Fogs, Volumetric Clouds, Sky Atmosphere, Ultra Dynamic Sky Actors and all Actors with the <c>Sun</c> Actor Tag from the Level</summary>
    public static void DestroyAllSky() => ScriptApi.Call(DestroyAllSkySlot, 0);

    /// <summary>Gets the current time</summary>
    public static object?[] GetTimeOfDay() => ScriptApi.CallResult<object?[]>(GetTimeOfDaySlot, 0)!;

    /// <summary>Gets the current weather</summary>
    public static WeatherType GetWeather() => ScriptApi.CallResult<WeatherType>(GetWeatherSlot, 0)!;

    /// <summary>Returns if the Ultra Dynamic Sky was spawned</summary>
    public static bool IsSpawned(bool? includingWeather = null) =>
        ScriptApi.CallResult<bool, bool?>(IsSpawnedSlot, 0, includingWeather)!;

    /// <summary>Reconstruct the Ultra Dynamic Sky Actor, forces most of the changes to be applied</summary>
    public static void Reconstruct() => ScriptApi.Call(ReconstructSlot, 0);

    /// <summary>Sets whether or not to animate the time of day</summary>
    public static void SetAnimateTimeOfDay(bool animate, double? dayLength = null, double? nightLength = null) =>
        ScriptApi.Call(SetAnimateTimeOfDaySlot, 1, animate, dayLength, nightLength);

    /// <summary>The amount of coverage for the cloud layer, from a clear sky to overcast</summary>
    public static void SetCloudCoverage(double cloudCoverage) => ScriptApi.Call(SetCloudCoverageSlot, 1, cloudCoverage);

    /// <summary>A simple contrast boost for the sky material. Cannot affect volumetric clouds</summary>
    public static void SetContrast(double contrast) => ScriptApi.Call(SetContrastSlot, 1, contrast);

    /// <summary>The fogginess of the scene. The impact this has on fog density is scaled from the Fog Density category</summary>
    public static void SetFog(double fogPercentage) => ScriptApi.Call(SetFogSlot, 1, fogPercentage);

    /// <summary>Sets the Moon Angle</summary>
    public static void SetMoonAngle(double yaw, double? pitch = null) =>
        ScriptApi.Call(SetMoonAngleSlot, 1, yaw, pitch);

    /// <summary>Sets the Moon Glow Intensity</summary>
    public static void SetMoonGlowIntensity(double glowIntensity) =>
        ScriptApi.Call(SetMoonGlowIntensitySlot, 1, glowIntensity);

    /// <summary>Sets the Moon Light Intensity</summary>
    public static void SetMoonLightIntensity(double intensity) =>
        ScriptApi.Call(SetMoonLightIntensitySlot, 1, intensity);

    /// <summary>Sets the Moon Phase, from new moon to full moon and back</summary>
    public static void SetMoonPhase(double phase) => ScriptApi.Call(SetMoonPhaseSlot, 1, phase);

    /// <summary>Sets the Moon Scale</summary>
    public static void SetMoonScale(double scale) => ScriptApi.Call(SetMoonScaleSlot, 1, scale);

    /// <summary>Sets the Moon Texture</summary>
    public static void SetMoonTexture(string texture) => ScriptApi.Call(SetMoonTextureSlot, 1, texture);

    /// <summary>Sets the Moon Texture Rotation</summary>
    public static void SetMoonTextureRotation(double rotation) =>
        ScriptApi.Call(SetMoonTextureRotationSlot, 1, rotation);

    /// <summary>Sets the Moon Vertical Offset</summary>
    public static void SetMoonVerticalOffset(double offset) => ScriptApi.Call(SetMoonVerticalOffsetSlot, 1, offset);

    /// <summary>Can be used to scale the brightness of the sky and lighting, at night</summary>
    public static void SetNightBrightness(double brightness) => ScriptApi.Call(SetNightBrightnessSlot, 1, brightness);

    /// <summary>The overall brightness of the shader</summary>
    public static void SetOverallIntensity(double intensity) => ScriptApi.Call(SetOverallIntensitySlot, 1, intensity);

    /// <summary>Sets the Sky Mode</summary>
    public static void SetSkyMode(SkyMode skyMode) => ScriptApi.Call(SetSkyModeSlot, 1, skyMode);

    /// <summary>Sets the Sun Angle</summary>
    public static void SetSunAngle(double yaw, double? pitch = null) => ScriptApi.Call(SetSunAngleSlot, 1, yaw, pitch);

    /// <summary>Sets the Sun Light Intensity</summary>
    public static void SetSunLightIntensity(double intensity) => ScriptApi.Call(SetSunLightIntensitySlot, 1, intensity);

    /// <summary>Sets the Sun Scale</summary>
    public static void SetSunScale(double radius) => ScriptApi.Call(SetSunScaleSlot, 1, radius);

    /// <summary>Sets the current time</summary>
    public static void SetTimeOfDay(int hours, int minutes, float? transitionTime = null) =>
        ScriptApi.Call(SetTimeOfDaySlot, 2, hours, minutes, transitionTime);

    /// <summary>Sets the Volumetric Cloud Color</summary>
    public static void SetVolumetricCloudColor(Color color) => ScriptApi.Call(SetVolumetricCloudColorSlot, 1, color);

    /// <summary>Replaces all Sky/Sun actors with the Ultra Dynamic Sky Actor, to be able to use the functions from this page</summary>
    public static bool Spawn(bool? spawnWeather = null, bool? findExisting = null) =>
        ScriptApi.CallResult<bool, bool?, bool?>(SpawnSlot, 0, spawnWeather, findExisting)!;
}
