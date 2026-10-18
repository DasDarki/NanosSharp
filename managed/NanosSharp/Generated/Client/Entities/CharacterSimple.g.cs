#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class CharacterSimple
{
    /// <summary>When an Animation Montage Notify begins</summary>
    public static event AnimationBeginNotifyHandler? OnAnimationBeginNotify
    {
        add => AnimationBeginNotifyEvent.Add(value);
        remove => AnimationBeginNotifyEvent.Remove(value);
    }

    /// <summary>When an Animation Montage Notify ends</summary>
    public static event AnimationEndNotifyHandler? OnAnimationEndNotify
    {
        add => AnimationEndNotifyEvent.Add(value);
        remove => AnimationEndNotifyEvent.Remove(value);
    }

    /// <summary>Called when Character stops crouching</summary>
    public static event EndCrouchHandler? OnEndCrouch
    {
        add => EndCrouchEvent.Add(value);
        remove => EndCrouchEvent.Remove(value);
    }

    /// <summary>Event fired when the character has just started jumping</summary>
    public static event JumpHandler? OnJump
    {
        add => JumpEvent.Add(value);
        remove => JumpEvent.Remove(value);
    }

    /// <summary>Called upon landing when falling</summary>
    public static event LandHandler? OnLand
    {
        add => LandEvent.Add(value);
        remove => LandEvent.Remove(value);
    }

    /// <summary>Called when the Character movement mode changes</summary>
    public static event MovementModeChangeHandler? OnMovementModeChange
    {
        add => MovementModeChangeEvent.Add(value);
        remove => MovementModeChangeEvent.Remove(value);
    }

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    /// <summary>Called when Character crouches</summary>
    public static event StartCrouchHandler? OnStartCrouch
    {
        add => StartCrouchEvent.Add(value);
        remove => StartCrouchEvent.Remove(value);
    }

    private static readonly ClassEvent<AnimationBeginNotifyHandler> AnimationBeginNotifyEvent = new("CharacterSimple", "AnimationBeginNotify");
    private static readonly ClassEvent<AnimationEndNotifyHandler> AnimationEndNotifyEvent = new("CharacterSimple", "AnimationEndNotify");
    private static readonly ClassEvent<EndCrouchHandler> EndCrouchEvent = new("CharacterSimple", "EndCrouch");
    private static readonly ClassEvent<JumpHandler> JumpEvent = new("CharacterSimple", "Jump");
    private static readonly ClassEvent<LandHandler> LandEvent = new("CharacterSimple", "Land");
    private static readonly ClassEvent<MovementModeChangeHandler> MovementModeChangeEvent = new("CharacterSimple", "MovementModeChange");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("CharacterSimple", "NetworkAuthorityChange");
    private static readonly ClassEvent<StartCrouchHandler> StartCrouchEvent = new("CharacterSimple", "StartCrouch");
    private static readonly MethodSlot ClearMorphTargetsSlot = new("ClearMorphTargets");
    private static readonly MethodSlot GetAllMorphTargetNamesSlot = new("GetAllMorphTargetNames");
    private static readonly MethodSlot GetMorphTargetSlot = new("GetMorphTarget");
    private static readonly MethodSlot SetMaterialFromCanvasSlot = new("SetMaterialFromCanvas");
    private static readonly MethodSlot SetMaterialFromSceneCaptureSlot = new("SetMaterialFromSceneCapture");
    private static readonly MethodSlot SetMaterialFromWebUISlot = new("SetMaterialFromWebUI");
    private static readonly MethodSlot SetMorphTargetSlot = new("SetMorphTarget");
    private static readonly MethodSlot SetSpeedSettingsSlot = new("SetSpeedSettings");
    private static readonly MethodSlot SetTeamSlot = new("SetTeam");

    /// <summary>Gets the Team of this Character</summary>
    public uint Team
    {
        get => GetTeam();
        set => SetTeam(value);
    }

    /// <summary>Returns a table with all morph targets available</summary>
    public IReadOnlyList<string> AllMorphTargetNames => GetAllMorphTargetNames();

    /// <summary>Clear all Morph Target that are set to this Mesh</summary>
    public void ClearMorphTargets() => Call(ClearMorphTargetsSlot, 0);

    /// <summary>Returns a table with all morph targets available</summary>
    public IReadOnlyList<string> GetAllMorphTargetNames() =>
        CallResult<IReadOnlyList<string>>(GetAllMorphTargetNamesSlot, 0)!;

    /// <summary>Returns the value of a Morph Target</summary>
    public float GetMorphTarget(string name) => CallResult<float, string>(GetMorphTargetSlot, 1, name)!;

    /// <summary>Sets the material at the specified index of this Actor to a Canvas object</summary>
    public void SetMaterialFromCanvas(Entity canvas, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromCanvasSlot, 1, canvas, index, argument3);

    /// <summary>Sets the material at the specified index of this Actor to a SceneCapture object</summary>
    public void SetMaterialFromSceneCapture(Entity sceneCapture, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromSceneCaptureSlot, 1, sceneCapture, index, argument3);

    /// <summary>Sets the material at the specified index of this Actor to a WebUI object</summary>
    public void SetMaterialFromWebUI(Entity webui, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromWebUISlot, 1, webui, index, argument3);

    /// <summary>Sets the value of a Morph Target</summary>
    public void SetMorphTarget(string name, float value) => Call(SetMorphTargetSlot, 2, name, value);

    /// <summary>Sets the Speed Settings of this Character</summary>
    public void SetSpeedSettings(
        ushort? maxWalkSpeed = null,
        ushort? maxWalkSpeedCrouched = null,
        ushort? maxFlySpeed = null,
        ushort? argument4 = null) =>
        Call(SetSpeedSettingsSlot, 0, maxWalkSpeed, maxWalkSpeedCrouched, maxFlySpeed, argument4);

    /// <summary>Sets a Team which will disable damaging same Team Members</summary>
    public void SetTeam(uint team) => Call(SetTeamSlot, 1, team);

    /// <summary>Subscribes to the AnimationBeginNotify event of this entity only.</summary>
    public ScriptSubscription SubscribeAnimationBeginNotify(AnimationBeginNotifyHandler handler) => Subscribe("AnimationBeginNotify", handler);

    /// <summary>Subscribes to the AnimationEndNotify event of this entity only.</summary>
    public ScriptSubscription SubscribeAnimationEndNotify(AnimationEndNotifyHandler handler) => Subscribe("AnimationEndNotify", handler);

    /// <summary>Subscribes to the EndCrouch event of this entity only.</summary>
    public ScriptSubscription SubscribeEndCrouch(EndCrouchHandler handler) => Subscribe("EndCrouch", handler);

    /// <summary>Subscribes to the Jump event of this entity only.</summary>
    public ScriptSubscription SubscribeJump(JumpHandler handler) => Subscribe("Jump", handler);

    /// <summary>Subscribes to the Land event of this entity only.</summary>
    public ScriptSubscription SubscribeLand(LandHandler handler) => Subscribe("Land", handler);

    /// <summary>Subscribes to the MovementModeChange event of this entity only.</summary>
    public ScriptSubscription SubscribeMovementModeChange(MovementModeChangeHandler handler) => Subscribe("MovementModeChange", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the StartCrouch event of this entity only.</summary>
    public ScriptSubscription SubscribeStartCrouch(StartCrouchHandler handler) => Subscribe("StartCrouch", handler);

    /// <summary>Called for the AnimationBeginNotify event of this entity when a subclass overrides it.</summary>
    [EventHook("AnimationBeginNotify")]
    protected virtual void HandleAnimationBeginNotify(string? notifyName, string? animationName, double triggerBeginTime, double triggerEndTime)
    {
    }

    /// <summary>Called for the AnimationEndNotify event of this entity when a subclass overrides it.</summary>
    [EventHook("AnimationEndNotify")]
    protected virtual void HandleAnimationEndNotify(string? notifyName, string? animationName, double triggerBeginTime, double triggerEndTime)
    {
    }

    /// <summary>Called for the EndCrouch event of this entity when a subclass overrides it.</summary>
    [EventHook("EndCrouch")]
    protected virtual void HandleEndCrouch()
    {
    }

    /// <summary>Called for the Jump event of this entity when a subclass overrides it.</summary>
    [EventHook("Jump")]
    protected virtual void HandleJump()
    {
    }

    /// <summary>Called for the Land event of this entity when a subclass overrides it.</summary>
    [EventHook("Land")]
    protected virtual void HandleLand()
    {
    }

    /// <summary>Called for the MovementModeChange event of this entity when a subclass overrides it.</summary>
    [EventHook("MovementModeChange")]
    protected virtual void HandleMovementModeChange(int oldMode, int newMode)
    {
    }

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(bool isNetworkAuthority)
    {
    }

    /// <summary>Called for the StartCrouch event of this entity when a subclass overrides it.</summary>
    [EventHook("StartCrouch")]
    protected virtual void HandleStartCrouch()
    {
    }

    /// <summary>When an Animation Montage Notify begins</summary>
    public delegate void AnimationBeginNotifyHandler(CharacterSimple self, string? notifyName, string? animationName, double triggerBeginTime, double triggerEndTime);

    /// <summary>When an Animation Montage Notify ends</summary>
    public delegate void AnimationEndNotifyHandler(CharacterSimple self, string? notifyName, string? animationName, double triggerBeginTime, double triggerEndTime);

    /// <summary>Called when Character stops crouching</summary>
    public delegate void EndCrouchHandler(CharacterSimple self);

    /// <summary>Event fired when the character has just started jumping</summary>
    public delegate void JumpHandler(CharacterSimple self);

    /// <summary>Called upon landing when falling</summary>
    public delegate void LandHandler(CharacterSimple self);

    /// <summary>Called when the Character movement mode changes</summary>
    public delegate void MovementModeChangeHandler(CharacterSimple self, int oldMode, int newMode);

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public delegate void NetworkAuthorityChangeHandler(CharacterSimple self, bool isNetworkAuthority);

    /// <summary>Called when Character crouches</summary>
    public delegate void StartCrouchHandler(CharacterSimple self);
}
