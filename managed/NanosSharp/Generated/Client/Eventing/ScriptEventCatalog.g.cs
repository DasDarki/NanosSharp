#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

internal static partial class ScriptEventCatalog
{
    static partial void AddSideDescriptors(Dictionary<Type, ScriptEventDescriptor> descriptors)
    {
        descriptors[typeof(BillboardClassRegisterEvent)] = new("Billboard", "ClassRegister", arguments => new BillboardClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(BillboardDestroyEvent)] = new("Billboard", "Destroy", arguments => new BillboardDestroyEvent
        {
            Billboard = ScriptEventArguments.Get<Billboard>(arguments, 0)!,
        });
        descriptors[typeof(BillboardEnterWaterEvent)] = new("Billboard", "EnterWater", arguments => new BillboardEnterWaterEvent
        {
            Billboard = ScriptEventArguments.Get<Billboard>(arguments, 0)!,
        });
        descriptors[typeof(BillboardLeaveWaterEvent)] = new("Billboard", "LeaveWater", arguments => new BillboardLeaveWaterEvent
        {
            Billboard = ScriptEventArguments.Get<Billboard>(arguments, 0)!,
        });
        descriptors[typeof(BillboardNetworkAuthorityChangeEvent)] = new("Billboard", "NetworkAuthorityChange", arguments => new BillboardNetworkAuthorityChangeEvent
        {
            Billboard = ScriptEventArguments.Get<Billboard>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(BillboardSpawnEvent)] = new("Billboard", "Spawn", arguments => new BillboardSpawnEvent
        {
            Billboard = ScriptEventArguments.Get<Billboard>(arguments, 0)!,
        });
        descriptors[typeof(BillboardValueChangeEvent)] = new("Billboard", "ValueChange", arguments => new BillboardValueChangeEvent
        {
            Billboard = ScriptEventArguments.Get<Billboard>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(BlueprintNetworkAuthorityChangeEvent)] = new("Blueprint", "NetworkAuthorityChange", arguments => new BlueprintNetworkAuthorityChangeEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(CableNetworkAuthorityChangeEvent)] = new("Cable", "NetworkAuthorityChange", arguments => new CableNetworkAuthorityChangeEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(CanvasClassRegisterEvent)] = new("Canvas", "ClassRegister", arguments => new CanvasClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(CanvasDestroyEvent)] = new("Canvas", "Destroy", arguments => new CanvasDestroyEvent
        {
            Canvas = ScriptEventArguments.Get<Canvas>(arguments, 0)!,
        });
        descriptors[typeof(CanvasSpawnEvent)] = new("Canvas", "Spawn", arguments => new CanvasSpawnEvent
        {
            Canvas = ScriptEventArguments.Get<Canvas>(arguments, 0)!,
        });
        descriptors[typeof(CanvasUpdateEvent)] = new("Canvas", "Update", arguments => new CanvasUpdateEvent
        {
            Canvas = ScriptEventArguments.Get<Canvas>(arguments, 0)!,
            Width = ScriptEventArguments.Get<int>(arguments, 1)!,
            Height = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CanvasValueChangeEvent)] = new("Canvas", "ValueChange", arguments => new CanvasValueChangeEvent
        {
            Canvas = ScriptEventArguments.Get<Canvas>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(CharacterAnimationBeginNotifyEvent)] = new("Character", "AnimationBeginNotify", arguments => new CharacterAnimationBeginNotifyEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            NotifyName = ScriptEventArguments.Get<string?>(arguments, 1),
            AnimationName = ScriptEventArguments.Get<string?>(arguments, 2),
            TriggerBeginTime = ScriptEventArguments.Get<double>(arguments, 3)!,
            TriggerEndTime = ScriptEventArguments.Get<double>(arguments, 4)!,
        });
        descriptors[typeof(CharacterAnimationEndNotifyEvent)] = new("Character", "AnimationEndNotify", arguments => new CharacterAnimationEndNotifyEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            NotifyName = ScriptEventArguments.Get<string?>(arguments, 1),
            AnimationName = ScriptEventArguments.Get<string?>(arguments, 2),
            TriggerBeginTime = ScriptEventArguments.Get<double>(arguments, 3)!,
            TriggerEndTime = ScriptEventArguments.Get<double>(arguments, 4)!,
        });
        descriptors[typeof(CharacterNetworkAuthorityChangeEvent)] = new("Character", "NetworkAuthorityChange", arguments => new CharacterNetworkAuthorityChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(CharacterGASPAnimationBeginNotifyEvent)] = new("CharacterGASP", "AnimationBeginNotify", arguments => new CharacterGASPAnimationBeginNotifyEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            NotifyName = ScriptEventArguments.Get<string?>(arguments, 1),
            AnimationName = ScriptEventArguments.Get<string?>(arguments, 2),
            TriggerBeginTime = ScriptEventArguments.Get<double>(arguments, 3)!,
            TriggerEndTime = ScriptEventArguments.Get<double>(arguments, 4)!,
        });
        descriptors[typeof(CharacterGASPAnimationEndNotifyEvent)] = new("CharacterGASP", "AnimationEndNotify", arguments => new CharacterGASPAnimationEndNotifyEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            NotifyName = ScriptEventArguments.Get<string?>(arguments, 1),
            AnimationName = ScriptEventArguments.Get<string?>(arguments, 2),
            TriggerBeginTime = ScriptEventArguments.Get<double>(arguments, 3)!,
            TriggerEndTime = ScriptEventArguments.Get<double>(arguments, 4)!,
        });
        descriptors[typeof(CharacterGASPNetworkAuthorityChangeEvent)] = new("CharacterGASP", "NetworkAuthorityChange", arguments => new CharacterGASPNetworkAuthorityChangeEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(CharacterSimpleAnimationBeginNotifyEvent)] = new("CharacterSimple", "AnimationBeginNotify", arguments => new CharacterSimpleAnimationBeginNotifyEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            NotifyName = ScriptEventArguments.Get<string?>(arguments, 1),
            AnimationName = ScriptEventArguments.Get<string?>(arguments, 2),
            TriggerBeginTime = ScriptEventArguments.Get<double>(arguments, 3)!,
            TriggerEndTime = ScriptEventArguments.Get<double>(arguments, 4)!,
        });
        descriptors[typeof(CharacterSimpleAnimationEndNotifyEvent)] = new("CharacterSimple", "AnimationEndNotify", arguments => new CharacterSimpleAnimationEndNotifyEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            NotifyName = ScriptEventArguments.Get<string?>(arguments, 1),
            AnimationName = ScriptEventArguments.Get<string?>(arguments, 2),
            TriggerBeginTime = ScriptEventArguments.Get<double>(arguments, 3)!,
            TriggerEndTime = ScriptEventArguments.Get<double>(arguments, 4)!,
        });
        descriptors[typeof(CharacterSimpleEndCrouchEvent)] = new("CharacterSimple", "EndCrouch", arguments => new CharacterSimpleEndCrouchEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        });
        descriptors[typeof(CharacterSimpleJumpEvent)] = new("CharacterSimple", "Jump", arguments => new CharacterSimpleJumpEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        });
        descriptors[typeof(CharacterSimpleLandEvent)] = new("CharacterSimple", "Land", arguments => new CharacterSimpleLandEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        });
        descriptors[typeof(CharacterSimpleMovementModeChangeEvent)] = new("CharacterSimple", "MovementModeChange", arguments => new CharacterSimpleMovementModeChangeEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            OldMode = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewMode = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterSimpleNetworkAuthorityChangeEvent)] = new("CharacterSimple", "NetworkAuthorityChange", arguments => new CharacterSimpleNetworkAuthorityChangeEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(CharacterSimpleStartCrouchEvent)] = new("CharacterSimple", "StartCrouch", arguments => new CharacterSimpleStartCrouchEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        });
        descriptors[typeof(ChatChatEntryEvent)] = new("Chat", "ChatEntry", arguments => new ChatChatEntryEvent
        {
            Message = ScriptEventArguments.Get<string?>(arguments, 0),
            Player = ScriptEventArguments.Get<Player?>(arguments, 1),
        });
        descriptors[typeof(ChatCloseEvent)] = new("Chat", "Close", arguments => new ChatCloseEvent
        {
        });
        descriptors[typeof(ChatOpenEvent)] = new("Chat", "Open", arguments => new ChatOpenEvent
        {
        });
        descriptors[typeof(ClientLanguageChangeEvent)] = new("Client", "LanguageChange", arguments => new ClientLanguageChangeEvent
        {
            Language = ScriptEventArguments.Get<string?>(arguments, 0),
        });
        descriptors[typeof(ClientPackageLoadEvent)] = new("Client", "PackageLoad", arguments => new ClientPackageLoadEvent
        {
            PackageName = ScriptEventArguments.Get<string?>(arguments, 0),
        });
        descriptors[typeof(ClientPackageUnloadEvent)] = new("Client", "PackageUnload", arguments => new ClientPackageUnloadEvent
        {
            PackageName = ScriptEventArguments.Get<string?>(arguments, 0),
        });
        descriptors[typeof(ClientSpawnLocalPlayerEvent)] = new("Client", "SpawnLocalPlayer", arguments => new ClientSpawnLocalPlayerEvent
        {
            LocalPlayer = ScriptEventArguments.Get<Player?>(arguments, 0),
        });
        descriptors[typeof(ClientTickEvent)] = new("Client", "Tick", arguments => new ClientTickEvent
        {
            DeltaTime = ScriptEventArguments.Get<float>(arguments, 0)!,
        });
        descriptors[typeof(ClientVOIPSelfMuteChangeEvent)] = new("Client", "VOIPSelfMuteChange", arguments => new ClientVOIPSelfMuteChangeEvent
        {
            IsMuted = ScriptEventArguments.Get<bool>(arguments, 0)!,
        });
        descriptors[typeof(ClientValueChangeEvent)] = new("Client", "ValueChange", arguments => new ClientValueChangeEvent
        {
            Key = ScriptEventArguments.Get<string?>(arguments, 0),
            Value = ScriptEventArguments.Get<object?>(arguments, 1),
        });
        descriptors[typeof(ClientWindowFocusChangeEvent)] = new("Client", "WindowFocusChange", arguments => new ClientWindowFocusChangeEvent
        {
            IsFocused = ScriptEventArguments.Get<bool>(arguments, 0)!,
        });
        descriptors[typeof(DecalClassRegisterEvent)] = new("Decal", "ClassRegister", arguments => new DecalClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(DecalDestroyEvent)] = new("Decal", "Destroy", arguments => new DecalDestroyEvent
        {
            Decal = ScriptEventArguments.Get<Decal>(arguments, 0)!,
        });
        descriptors[typeof(DecalEnterWaterEvent)] = new("Decal", "EnterWater", arguments => new DecalEnterWaterEvent
        {
            Decal = ScriptEventArguments.Get<Decal>(arguments, 0)!,
        });
        descriptors[typeof(DecalLeaveWaterEvent)] = new("Decal", "LeaveWater", arguments => new DecalLeaveWaterEvent
        {
            Decal = ScriptEventArguments.Get<Decal>(arguments, 0)!,
        });
        descriptors[typeof(DecalNetworkAuthorityChangeEvent)] = new("Decal", "NetworkAuthorityChange", arguments => new DecalNetworkAuthorityChangeEvent
        {
            Decal = ScriptEventArguments.Get<Decal>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(DecalSpawnEvent)] = new("Decal", "Spawn", arguments => new DecalSpawnEvent
        {
            Decal = ScriptEventArguments.Get<Decal>(arguments, 0)!,
        });
        descriptors[typeof(DecalValueChangeEvent)] = new("Decal", "ValueChange", arguments => new DecalValueChangeEvent
        {
            Decal = ScriptEventArguments.Get<Decal>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(GizmoClassRegisterEvent)] = new("Gizmo", "ClassRegister", arguments => new GizmoClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(GizmoDestroyEvent)] = new("Gizmo", "Destroy", arguments => new GizmoDestroyEvent
        {
            Gizmo = ScriptEventArguments.Get<Gizmo>(arguments, 0)!,
        });
        descriptors[typeof(GizmoEnterWaterEvent)] = new("Gizmo", "EnterWater", arguments => new GizmoEnterWaterEvent
        {
            Gizmo = ScriptEventArguments.Get<Gizmo>(arguments, 0)!,
        });
        descriptors[typeof(GizmoLeaveWaterEvent)] = new("Gizmo", "LeaveWater", arguments => new GizmoLeaveWaterEvent
        {
            Gizmo = ScriptEventArguments.Get<Gizmo>(arguments, 0)!,
        });
        descriptors[typeof(GizmoNetworkAuthorityChangeEvent)] = new("Gizmo", "NetworkAuthorityChange", arguments => new GizmoNetworkAuthorityChangeEvent
        {
            Gizmo = ScriptEventArguments.Get<Gizmo>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(GizmoSpawnEvent)] = new("Gizmo", "Spawn", arguments => new GizmoSpawnEvent
        {
            Gizmo = ScriptEventArguments.Get<Gizmo>(arguments, 0)!,
        });
        descriptors[typeof(GizmoTransformEvent)] = new("Gizmo", "Transform", arguments => new GizmoTransformEvent
        {
            Gizmo = ScriptEventArguments.Get<Gizmo>(arguments, 0)!,
            Location = ScriptEventArguments.Get<Vector>(arguments, 1)!,
            Rotation = ScriptEventArguments.Get<Rotator>(arguments, 2)!,
            Scale = ScriptEventArguments.Get<Vector>(arguments, 3)!,
        });
        descriptors[typeof(GizmoValueChangeEvent)] = new("Gizmo", "ValueChange", arguments => new GizmoValueChangeEvent
        {
            Gizmo = ScriptEventArguments.Get<Gizmo>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(GrenadeNetworkAuthorityChangeEvent)] = new("Grenade", "NetworkAuthorityChange", arguments => new GrenadeNetworkAuthorityChangeEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(InputKeyBindingChangeEvent)] = new("Input_S", "KeyBindingChange", arguments => new InputKeyBindingChangeEvent
        {
            BindingName = ScriptEventArguments.Get<string?>(arguments, 0),
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Scale = ScriptEventArguments.Get<float>(arguments, 2)!,
        });
        descriptors[typeof(InputKeyDownEvent)] = new("Input_S", "KeyDown", arguments => new InputKeyDownEvent
        {
            KeyName = ScriptEventArguments.Get<string?>(arguments, 0),
            Delta = ScriptEventArguments.Get<float>(arguments, 1)!,
        });
        descriptors[typeof(InputKeyPressEvent)] = new("Input_S", "KeyPress", arguments => new InputKeyPressEvent
        {
            KeyName = ScriptEventArguments.Get<string?>(arguments, 0),
            Delta = ScriptEventArguments.Get<float>(arguments, 1)!,
        });
        descriptors[typeof(InputKeyUpEvent)] = new("Input_S", "KeyUp", arguments => new InputKeyUpEvent
        {
            KeyName = ScriptEventArguments.Get<string?>(arguments, 0),
            Delta = ScriptEventArguments.Get<float>(arguments, 1)!,
        });
        descriptors[typeof(InputMouseDownEvent)] = new("Input_S", "MouseDown", arguments => new InputMouseDownEvent
        {
            KeyName = ScriptEventArguments.Get<string?>(arguments, 0),
            MouseX = ScriptEventArguments.Get<float>(arguments, 1)!,
            MouseY = ScriptEventArguments.Get<float>(arguments, 2)!,
        });
        descriptors[typeof(InputMouseEnableEvent)] = new("Input_S", "MouseEnable", arguments => new InputMouseEnableEvent
        {
            IsEnabled = ScriptEventArguments.Get<bool>(arguments, 0)!,
        });
        descriptors[typeof(InputMouseMoveEvent)] = new("Input_S", "MouseMove", arguments => new InputMouseMoveEvent
        {
            CursorDeltaX = ScriptEventArguments.Get<float>(arguments, 0)!,
            CursorDeltaY = ScriptEventArguments.Get<float>(arguments, 1)!,
            MouseX = ScriptEventArguments.Get<float>(arguments, 2)!,
            MouseY = ScriptEventArguments.Get<float>(arguments, 3)!,
        });
        descriptors[typeof(InputMouseScrollEvent)] = new("Input_S", "MouseScroll", arguments => new InputMouseScrollEvent
        {
            MouseX = ScriptEventArguments.Get<float>(arguments, 0)!,
            MouseY = ScriptEventArguments.Get<float>(arguments, 1)!,
            Delta = ScriptEventArguments.Get<float>(arguments, 2)!,
        });
        descriptors[typeof(InputMouseUpEvent)] = new("Input_S", "MouseUp", arguments => new InputMouseUpEvent
        {
            KeyName = ScriptEventArguments.Get<string?>(arguments, 0),
            MouseX = ScriptEventArguments.Get<float>(arguments, 1)!,
            MouseY = ScriptEventArguments.Get<float>(arguments, 2)!,
        });
        descriptors[typeof(InstancedStaticMeshNetworkAuthorityChangeEvent)] = new("InstancedStaticMesh", "NetworkAuthorityChange", arguments => new InstancedStaticMeshNetworkAuthorityChangeEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(LevelStreamLevelBeginPauseEvent)] = new("Level", "StreamLevelBeginPause", arguments => new LevelStreamLevelBeginPauseEvent
        {
        });
        descriptors[typeof(LevelStreamLevelEndPauseEvent)] = new("Level", "StreamLevelEndPause", arguments => new LevelStreamLevelEndPauseEvent
        {
        });
        descriptors[typeof(LevelStreamLevelHideEvent)] = new("Level", "StreamLevelHide", arguments => new LevelStreamLevelHideEvent
        {
            LevelName = ScriptEventArguments.Get<string?>(arguments, 0),
        });
        descriptors[typeof(LevelStreamLevelLoadEvent)] = new("Level", "StreamLevelLoad", arguments => new LevelStreamLevelLoadEvent
        {
            LevelName = ScriptEventArguments.Get<string?>(arguments, 0),
        });
        descriptors[typeof(LevelStreamLevelShowEvent)] = new("Level", "StreamLevelShow", arguments => new LevelStreamLevelShowEvent
        {
            LevelName = ScriptEventArguments.Get<string?>(arguments, 0),
        });
        descriptors[typeof(LevelStreamLevelUnloadEvent)] = new("Level", "StreamLevelUnload", arguments => new LevelStreamLevelUnloadEvent
        {
            LevelName = ScriptEventArguments.Get<string?>(arguments, 0),
        });
        descriptors[typeof(LightNetworkAuthorityChangeEvent)] = new("Light", "NetworkAuthorityChange", arguments => new LightNetworkAuthorityChangeEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(MeleeNetworkAuthorityChangeEvent)] = new("Melee", "NetworkAuthorityChange", arguments => new MeleeNetworkAuthorityChangeEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(ParticleNetworkAuthorityChangeEvent)] = new("Particle", "NetworkAuthorityChange", arguments => new ParticleNetworkAuthorityChangeEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(PropNetworkAuthorityChangeEvent)] = new("Prop", "NetworkAuthorityChange", arguments => new PropNetworkAuthorityChangeEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(SceneCaptureCaptureEvent)] = new("SceneCapture", "Capture", arguments => new SceneCaptureCaptureEvent
        {
            SceneCapture = ScriptEventArguments.Get<SceneCapture>(arguments, 0)!,
        });
        descriptors[typeof(SceneCaptureClassRegisterEvent)] = new("SceneCapture", "ClassRegister", arguments => new SceneCaptureClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(SceneCaptureDestroyEvent)] = new("SceneCapture", "Destroy", arguments => new SceneCaptureDestroyEvent
        {
            SceneCapture = ScriptEventArguments.Get<SceneCapture>(arguments, 0)!,
        });
        descriptors[typeof(SceneCaptureEnterWaterEvent)] = new("SceneCapture", "EnterWater", arguments => new SceneCaptureEnterWaterEvent
        {
            SceneCapture = ScriptEventArguments.Get<SceneCapture>(arguments, 0)!,
        });
        descriptors[typeof(SceneCaptureLeaveWaterEvent)] = new("SceneCapture", "LeaveWater", arguments => new SceneCaptureLeaveWaterEvent
        {
            SceneCapture = ScriptEventArguments.Get<SceneCapture>(arguments, 0)!,
        });
        descriptors[typeof(SceneCaptureNetworkAuthorityChangeEvent)] = new("SceneCapture", "NetworkAuthorityChange", arguments => new SceneCaptureNetworkAuthorityChangeEvent
        {
            SceneCapture = ScriptEventArguments.Get<SceneCapture>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(SceneCaptureSpawnEvent)] = new("SceneCapture", "Spawn", arguments => new SceneCaptureSpawnEvent
        {
            SceneCapture = ScriptEventArguments.Get<SceneCapture>(arguments, 0)!,
        });
        descriptors[typeof(SceneCaptureValueChangeEvent)] = new("SceneCapture", "ValueChange", arguments => new SceneCaptureValueChangeEvent
        {
            SceneCapture = ScriptEventArguments.Get<SceneCapture>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(SoundClassRegisterEvent)] = new("Sound", "ClassRegister", arguments => new SoundClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(SoundDestroyEvent)] = new("Sound", "Destroy", arguments => new SoundDestroyEvent
        {
            Sound = ScriptEventArguments.Get<Sound>(arguments, 0)!,
        });
        descriptors[typeof(SoundEnterWaterEvent)] = new("Sound", "EnterWater", arguments => new SoundEnterWaterEvent
        {
            Sound = ScriptEventArguments.Get<Sound>(arguments, 0)!,
        });
        descriptors[typeof(SoundLeaveWaterEvent)] = new("Sound", "LeaveWater", arguments => new SoundLeaveWaterEvent
        {
            Sound = ScriptEventArguments.Get<Sound>(arguments, 0)!,
        });
        descriptors[typeof(SoundNetworkAuthorityChangeEvent)] = new("Sound", "NetworkAuthorityChange", arguments => new SoundNetworkAuthorityChangeEvent
        {
            Sound = ScriptEventArguments.Get<Sound>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(SoundSpawnEvent)] = new("Sound", "Spawn", arguments => new SoundSpawnEvent
        {
            Sound = ScriptEventArguments.Get<Sound>(arguments, 0)!,
        });
        descriptors[typeof(SoundValueChangeEvent)] = new("Sound", "ValueChange", arguments => new SoundValueChangeEvent
        {
            Sound = ScriptEventArguments.Get<Sound>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(StaticMeshNetworkAuthorityChangeEvent)] = new("StaticMesh", "NetworkAuthorityChange", arguments => new StaticMeshNetworkAuthorityChangeEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(Text3DNetworkAuthorityChangeEvent)] = new("Text3D", "NetworkAuthorityChange", arguments => new Text3DNetworkAuthorityChangeEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(TextRenderNetworkAuthorityChangeEvent)] = new("TextRender", "NetworkAuthorityChange", arguments => new TextRenderNetworkAuthorityChangeEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(TriggerNetworkAuthorityChangeEvent)] = new("Trigger", "NetworkAuthorityChange", arguments => new TriggerNetworkAuthorityChangeEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(VehicleWaterNetworkAuthorityChangeEvent)] = new("VehicleWater", "NetworkAuthorityChange", arguments => new VehicleWaterNetworkAuthorityChangeEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(VehicleWheeledNetworkAuthorityChangeEvent)] = new("VehicleWheeled", "NetworkAuthorityChange", arguments => new VehicleWheeledNetworkAuthorityChangeEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(ViewportResizeEvent)] = new("Viewport", "Resize", arguments => new ViewportResizeEvent
        {
            NewSize = ScriptEventArguments.Get<Vector2D>(arguments, 0)!,
        });
        descriptors[typeof(WeaponBulletHitEvent)] = new("Weapon", "BulletHit", arguments => new WeaponBulletHitEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            ImpactPoint = ScriptEventArguments.Get<Vector>(arguments, 1)!,
            ImpactNormal = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 3)!,
            ActorHit = ScriptEventArguments.Get<Actor?>(arguments, 4),
        });
        descriptors[typeof(WeaponNetworkAuthorityChangeEvent)] = new("Weapon", "NetworkAuthorityChange", arguments => new WeaponNetworkAuthorityChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(WebUIClassRegisterEvent)] = new("WebUI", "ClassRegister", arguments => new WebUIClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(WebUIDestroyEvent)] = new("WebUI", "Destroy", arguments => new WebUIDestroyEvent
        {
            WebUI = ScriptEventArguments.Get<WebUI>(arguments, 0)!,
        });
        descriptors[typeof(WebUIFailEvent)] = new("WebUI", "Fail", arguments => new WebUIFailEvent
        {
            WebUI = ScriptEventArguments.Get<WebUI>(arguments, 0)!,
            ErrorCode = ScriptEventArguments.Get<int>(arguments, 1)!,
            Message = ScriptEventArguments.Get<string?>(arguments, 2),
        });
        descriptors[typeof(WebUIReadyEvent)] = new("WebUI", "Ready", arguments => new WebUIReadyEvent
        {
            WebUI = ScriptEventArguments.Get<WebUI>(arguments, 0)!,
        });
        descriptors[typeof(WebUISpawnEvent)] = new("WebUI", "Spawn", arguments => new WebUISpawnEvent
        {
            WebUI = ScriptEventArguments.Get<WebUI>(arguments, 0)!,
        });
        descriptors[typeof(WebUIValueChangeEvent)] = new("WebUI", "ValueChange", arguments => new WebUIValueChangeEvent
        {
            WebUI = ScriptEventArguments.Get<WebUI>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(WidgetClassRegisterEvent)] = new("Widget", "ClassRegister", arguments => new WidgetClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(WidgetDestroyEvent)] = new("Widget", "Destroy", arguments => new WidgetDestroyEvent
        {
            Widget = ScriptEventArguments.Get<Widget>(arguments, 0)!,
        });
        descriptors[typeof(WidgetSpawnEvent)] = new("Widget", "Spawn", arguments => new WidgetSpawnEvent
        {
            Widget = ScriptEventArguments.Get<Widget>(arguments, 0)!,
        });
        descriptors[typeof(WidgetValueChangeEvent)] = new("Widget", "ValueChange", arguments => new WidgetValueChangeEvent
        {
            Widget = ScriptEventArguments.Get<Widget>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(Widget3DClassRegisterEvent)] = new("Widget3D", "ClassRegister", arguments => new Widget3DClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(Widget3DDestroyEvent)] = new("Widget3D", "Destroy", arguments => new Widget3DDestroyEvent
        {
            Widget3D = ScriptEventArguments.Get<Widget3D>(arguments, 0)!,
        });
        descriptors[typeof(Widget3DEnterWaterEvent)] = new("Widget3D", "EnterWater", arguments => new Widget3DEnterWaterEvent
        {
            Widget3D = ScriptEventArguments.Get<Widget3D>(arguments, 0)!,
        });
        descriptors[typeof(Widget3DLeaveWaterEvent)] = new("Widget3D", "LeaveWater", arguments => new Widget3DLeaveWaterEvent
        {
            Widget3D = ScriptEventArguments.Get<Widget3D>(arguments, 0)!,
        });
        descriptors[typeof(Widget3DNetworkAuthorityChangeEvent)] = new("Widget3D", "NetworkAuthorityChange", arguments => new Widget3DNetworkAuthorityChangeEvent
        {
            Widget3D = ScriptEventArguments.Get<Widget3D>(arguments, 0)!,
            IsNetworkAuthority = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(Widget3DSpawnEvent)] = new("Widget3D", "Spawn", arguments => new Widget3DSpawnEvent
        {
            Widget3D = ScriptEventArguments.Get<Widget3D>(arguments, 0)!,
        });
        descriptors[typeof(Widget3DValueChangeEvent)] = new("Widget3D", "ValueChange", arguments => new Widget3DValueChangeEvent
        {
            Widget3D = ScriptEventArguments.Get<Widget3D>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
    }
}
