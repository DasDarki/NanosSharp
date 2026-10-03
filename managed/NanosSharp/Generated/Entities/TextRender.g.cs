#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A Text Render class is useful for spawning Texts in 3D world.</summary>
[ScriptClass("TextRender")]
public partial class TextRender : Actor
{
    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public static event ClassRegisterHandler? OnClassRegister
    {
        add => ClassRegisterEvent.Add(value);
        remove => ClassRegisterEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is destroyed</summary>
    public static event DestroyHandler? OnDestroy
    {
        add => DestroyEvent.Add(value);
        remove => DestroyEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor enters a water body</summary>
    public static event EnterWaterHandler? OnEnterWater
    {
        add => EnterWaterEvent.Add(value);
        remove => EnterWaterEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public static event LeaveWaterHandler? OnLeaveWater
    {
        add => LeaveWaterEvent.Add(value);
        remove => LeaveWaterEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public static event SpawnHandler? OnSpawn
    {
        add => SpawnEvent.Add(value);
        remove => SpawnEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("TextRender", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("TextRender", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("TextRender", "DimensionChange");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("TextRender", "EnterWater");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("TextRender", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("TextRender", "NetworkAuthorityChange");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("TextRender", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("TextRender", "ValueChange");
    private static readonly MethodSlot GetColorSlot = new("GetColor");
    private static readonly MethodSlot GetTextSlot = new("GetText");
    private static readonly MethodSlot GetWordSizeSlot = new("GetWordSize");
    private static readonly MethodSlot SetAlignmentSlot = new("SetAlignment");
    private static readonly MethodSlot SetColorSlot = new("SetColor");
    private static readonly MethodSlot SetFontSlot = new("SetFont");
    private static readonly MethodSlot SetSpacingAdjustSlot = new("SetSpacingAdjust");
    private static readonly MethodSlot SetTextSlot = new("SetText");
    private static readonly MethodSlot SetWordSizeSlot = new("SetWordSize");

    /// <summary>Gets the current Text Color</summary>
    public Color Color
    {
        get => GetColor();
        set => SetColor(value);
    }

    /// <summary>Gets the current Text</summary>
    public string Text
    {
        get => GetText();
        set => SetText(value);
    }

    /// <summary>Gets the current Word Size</summary>
    public float WordSize
    {
        get => GetWordSize();
        set => SetWordSize(value);
    }

    /// <summary>Spawns a new TextRender.</summary>
    public TextRender(
        Vector location,
        Rotator rotation,
        string text,
        float? wordSize = null,
        Color? color = null,
        TextRenderRenderingType? renderingType = null,
        TextRenderHorizontalAlignment? horizontalAlignment = null,
        TextRenderVerticalAlignment? verticalAlignment = null,
        string? fontAsset = null,
        bool? castShadow = null)
    {
        SpawnSelf("TextRender", ScriptArguments.Trim([location, rotation, text, wordSize, color, renderingType, horizontalAlignment, verticalAlignment, fontAsset, castShadow], 3));
    }

    protected TextRender()
    {
    }

    /// <summary>Gets the current Text Color</summary>
    public Color GetColor() => Invoke<Color>(GetColorSlot)!;

    /// <summary>Gets the current Text</summary>
    public string GetText() => Invoke<string>(GetTextSlot)!;

    /// <summary>Gets the current Word Size</summary>
    public float GetWordSize() => Invoke<float>(GetWordSizeSlot)!;

    /// <summary>Sets the Text alignment</summary>
    public void SetAlignment(
        TextRenderHorizontalAlignment horizontalAlignment,
        TextRenderVerticalAlignment verticalAlignment) =>
        Invoke(SetAlignmentSlot, horizontalAlignment, verticalAlignment);

    /// <summary>Sets the Text Color</summary>
    public void SetColor(Color color) => Invoke(SetColorSlot, color);

    /// <summary>Sets the Font. The font must be pre-cooked as an offline cached, distance field font.</summary>
    public void SetFont(string fontAsset) => Invoke(SetFontSlot, fontAsset);

    /// <summary>Sets the horizontal and vertical spacing adjust values</summary>
    public void SetSpacingAdjust(float horizontalSpacing, float verticalSpacing) =>
        Invoke(SetSpacingAdjustSlot, horizontalSpacing, verticalSpacing);

    /// <summary>Sets the Text</summary>
    public void SetText(string text) => Invoke(SetTextSlot, text);

    /// <summary>Sets the Word Size</summary>
    public void SetWordSize(float wordSize) => Invoke(SetWordSizeSlot, wordSize);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(TextRender self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(TextRender self, int oldDimension, int newDimension);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(TextRender self);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(TextRender self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(TextRender self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(TextRender self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(TextRender self, string? key, object? value);
}
