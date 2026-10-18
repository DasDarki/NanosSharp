#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

internal static partial class ScriptEventCatalog
{
    static partial void AddSharedDescriptors(Dictionary<Type, ScriptEventDescriptor> descriptors)
    {
        descriptors[typeof(BlueprintClassRegisterEvent)] = new("Blueprint", "ClassRegister", arguments => new BlueprintClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(BlueprintDestroyEvent)] = new("Blueprint", "Destroy", arguments => new BlueprintDestroyEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
        });
        descriptors[typeof(BlueprintEnterWaterEvent)] = new("Blueprint", "EnterWater", arguments => new BlueprintEnterWaterEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
        });
        descriptors[typeof(BlueprintLeaveWaterEvent)] = new("Blueprint", "LeaveWater", arguments => new BlueprintLeaveWaterEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
        });
        descriptors[typeof(BlueprintSpawnEvent)] = new("Blueprint", "Spawn", arguments => new BlueprintSpawnEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
        });
        descriptors[typeof(BlueprintValueChangeEvent)] = new("Blueprint", "ValueChange", arguments => new BlueprintValueChangeEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(CableClassRegisterEvent)] = new("Cable", "ClassRegister", arguments => new CableClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(CableDestroyEvent)] = new("Cable", "Destroy", arguments => new CableDestroyEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
        });
        descriptors[typeof(CableEnterWaterEvent)] = new("Cable", "EnterWater", arguments => new CableEnterWaterEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
        });
        descriptors[typeof(CableLeaveWaterEvent)] = new("Cable", "LeaveWater", arguments => new CableLeaveWaterEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
        });
        descriptors[typeof(CableSpawnEvent)] = new("Cable", "Spawn", arguments => new CableSpawnEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
        });
        descriptors[typeof(CableValueChangeEvent)] = new("Cable", "ValueChange", arguments => new CableValueChangeEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(CharacterAttackEvent)] = new("Character", "Attack", arguments => new CharacterAttackEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Melee = ScriptEventArguments.Get<Melee?>(arguments, 1),
        });
        descriptors[typeof(CharacterAttemptReloadEvent)] = new("Character", "AttemptReload", arguments => new CharacterAttemptReloadEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Weapon = ScriptEventArguments.Get<Weapon?>(arguments, 1),
        });
        descriptors[typeof(CharacterClassRegisterEvent)] = new("Character", "ClassRegister", arguments => new CharacterClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(CharacterDeathEvent)] = new("Character", "Death", arguments => new CharacterDeathEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            LastDamageTaken = ScriptEventArguments.Get<int>(arguments, 1)!,
            LastBoneDamaged = ScriptEventArguments.Get<string?>(arguments, 2),
            DamageTypeReason = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            HitFromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        });
        descriptors[typeof(CharacterDestroyEvent)] = new("Character", "Destroy", arguments => new CharacterDestroyEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        });
        descriptors[typeof(CharacterDropEvent)] = new("Character", "Drop", arguments => new CharacterDropEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Object = ScriptEventArguments.Get<Entity?>(arguments, 1),
            TriggeredByPlayer = ScriptEventArguments.Get<bool>(arguments, 2)!,
        });
        descriptors[typeof(CharacterEnterVehicleEvent)] = new("Character", "EnterVehicle", arguments => new CharacterEnterVehicleEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterEnterWaterEvent)] = new("Character", "EnterWater", arguments => new CharacterEnterWaterEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        });
        descriptors[typeof(CharacterGrabPropEvent)] = new("Character", "GrabProp", arguments => new CharacterGrabPropEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Prop = ScriptEventArguments.Get<Prop?>(arguments, 1),
        });
        descriptors[typeof(CharacterHealthChangeEvent)] = new("Character", "HealthChange", arguments => new CharacterHealthChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            OldHealth = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewHealth = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterHighlightEvent)] = new("Character", "Highlight", arguments => new CharacterHighlightEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            IsHighlighted = ScriptEventArguments.Get<bool>(arguments, 1)!,
            Object = ScriptEventArguments.Get<Prop?>(arguments, 2),
        });
        descriptors[typeof(CharacterLeaveVehicleEvent)] = new("Character", "LeaveVehicle", arguments => new CharacterLeaveVehicleEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterLeaveWaterEvent)] = new("Character", "LeaveWater", arguments => new CharacterLeaveWaterEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        });
        descriptors[typeof(CharacterMoveCompleteEvent)] = new("Character", "MoveComplete", arguments => new CharacterMoveCompleteEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Succeeded = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(CharacterPickUpEvent)] = new("Character", "PickUp", arguments => new CharacterPickUpEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Object = ScriptEventArguments.Get<Entity?>(arguments, 1),
        });
        descriptors[typeof(CharacterPossessEvent)] = new("Character", "Possess", arguments => new CharacterPossessEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Player = ScriptEventArguments.Get<Player?>(arguments, 1),
        });
        descriptors[typeof(CharacterPullUseEvent)] = new("Character", "PullUse", arguments => new CharacterPullUseEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Pickable = ScriptEventArguments.Get<Entity?>(arguments, 1),
        });
        descriptors[typeof(CharacterPunchEvent)] = new("Character", "Punch", arguments => new CharacterPunchEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        });
        descriptors[typeof(CharacterRagdollModeChangeEvent)] = new("Character", "RagdollModeChange", arguments => new CharacterRagdollModeChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            OldState = ScriptEventArguments.Get<bool>(arguments, 1)!,
            NewState = ScriptEventArguments.Get<bool>(arguments, 2)!,
        });
        descriptors[typeof(CharacterReleaseUseEvent)] = new("Character", "ReleaseUse", arguments => new CharacterReleaseUseEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Pickable = ScriptEventArguments.Get<Entity?>(arguments, 1),
        });
        descriptors[typeof(CharacterReloadEvent)] = new("Character", "Reload", arguments => new CharacterReloadEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Weapon = ScriptEventArguments.Get<Weapon?>(arguments, 1),
            AmmoToReload = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterRespawnEvent)] = new("Character", "Respawn", arguments => new CharacterRespawnEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        });
        descriptors[typeof(CharacterSpawnEvent)] = new("Character", "Spawn", arguments => new CharacterSpawnEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        });
        descriptors[typeof(CharacterTakeDamageEvent)] = new("Character", "TakeDamage", arguments => new CharacterTakeDamageEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<object?>(arguments, 6),
        });
        descriptors[typeof(CharacterUnGrabPropEvent)] = new("Character", "UnGrabProp", arguments => new CharacterUnGrabPropEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Prop = ScriptEventArguments.Get<Prop?>(arguments, 1),
        });
        descriptors[typeof(CharacterUnPossessEvent)] = new("Character", "UnPossess", arguments => new CharacterUnPossessEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            OldPlayer = ScriptEventArguments.Get<Player?>(arguments, 1),
        });
        descriptors[typeof(CharacterValueChangeEvent)] = new("Character", "ValueChange", arguments => new CharacterValueChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(CharacterGASPAttackEvent)] = new("CharacterGASP", "Attack", arguments => new CharacterGASPAttackEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Melee = ScriptEventArguments.Get<Melee?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPAttemptReloadEvent)] = new("CharacterGASP", "AttemptReload", arguments => new CharacterGASPAttemptReloadEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Weapon = ScriptEventArguments.Get<Weapon?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPClassRegisterEvent)] = new("CharacterGASP", "ClassRegister", arguments => new CharacterGASPClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(CharacterGASPDeathEvent)] = new("CharacterGASP", "Death", arguments => new CharacterGASPDeathEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            LastDamageTaken = ScriptEventArguments.Get<int>(arguments, 1)!,
            LastBoneDamaged = ScriptEventArguments.Get<string?>(arguments, 2),
            DamageTypeReason = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            HitFromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        });
        descriptors[typeof(CharacterGASPDestroyEvent)] = new("CharacterGASP", "Destroy", arguments => new CharacterGASPDestroyEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
        });
        descriptors[typeof(CharacterGASPDropEvent)] = new("CharacterGASP", "Drop", arguments => new CharacterGASPDropEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Object = ScriptEventArguments.Get<Entity?>(arguments, 1),
            TriggeredByPlayer = ScriptEventArguments.Get<bool>(arguments, 2)!,
        });
        descriptors[typeof(CharacterGASPEnterVehicleEvent)] = new("CharacterGASP", "EnterVehicle", arguments => new CharacterGASPEnterVehicleEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterGASPEnterWaterEvent)] = new("CharacterGASP", "EnterWater", arguments => new CharacterGASPEnterWaterEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
        });
        descriptors[typeof(CharacterGASPGrabPropEvent)] = new("CharacterGASP", "GrabProp", arguments => new CharacterGASPGrabPropEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Prop = ScriptEventArguments.Get<Prop?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPHealthChangeEvent)] = new("CharacterGASP", "HealthChange", arguments => new CharacterGASPHealthChangeEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            OldHealth = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewHealth = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterGASPHighlightEvent)] = new("CharacterGASP", "Highlight", arguments => new CharacterGASPHighlightEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            IsHighlighted = ScriptEventArguments.Get<bool>(arguments, 1)!,
            Object = ScriptEventArguments.Get<Prop?>(arguments, 2),
        });
        descriptors[typeof(CharacterGASPLeaveVehicleEvent)] = new("CharacterGASP", "LeaveVehicle", arguments => new CharacterGASPLeaveVehicleEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterGASPLeaveWaterEvent)] = new("CharacterGASP", "LeaveWater", arguments => new CharacterGASPLeaveWaterEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
        });
        descriptors[typeof(CharacterGASPMoveCompleteEvent)] = new("CharacterGASP", "MoveComplete", arguments => new CharacterGASPMoveCompleteEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Succeeded = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(CharacterGASPPickUpEvent)] = new("CharacterGASP", "PickUp", arguments => new CharacterGASPPickUpEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Object = ScriptEventArguments.Get<Entity?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPPossessEvent)] = new("CharacterGASP", "Possess", arguments => new CharacterGASPPossessEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Player = ScriptEventArguments.Get<Player?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPPullUseEvent)] = new("CharacterGASP", "PullUse", arguments => new CharacterGASPPullUseEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Pickable = ScriptEventArguments.Get<Entity?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPPunchEvent)] = new("CharacterGASP", "Punch", arguments => new CharacterGASPPunchEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
        });
        descriptors[typeof(CharacterGASPRagdollModeChangeEvent)] = new("CharacterGASP", "RagdollModeChange", arguments => new CharacterGASPRagdollModeChangeEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            OldState = ScriptEventArguments.Get<bool>(arguments, 1)!,
            NewState = ScriptEventArguments.Get<bool>(arguments, 2)!,
        });
        descriptors[typeof(CharacterGASPReleaseUseEvent)] = new("CharacterGASP", "ReleaseUse", arguments => new CharacterGASPReleaseUseEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Pickable = ScriptEventArguments.Get<Entity?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPReloadEvent)] = new("CharacterGASP", "Reload", arguments => new CharacterGASPReloadEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Weapon = ScriptEventArguments.Get<Weapon?>(arguments, 1),
            AmmoToReload = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterGASPRespawnEvent)] = new("CharacterGASP", "Respawn", arguments => new CharacterGASPRespawnEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
        });
        descriptors[typeof(CharacterGASPSpawnEvent)] = new("CharacterGASP", "Spawn", arguments => new CharacterGASPSpawnEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
        });
        descriptors[typeof(CharacterGASPTakeDamageEvent)] = new("CharacterGASP", "TakeDamage", arguments => new CharacterGASPTakeDamageEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<object?>(arguments, 6),
        });
        descriptors[typeof(CharacterGASPUnGrabPropEvent)] = new("CharacterGASP", "UnGrabProp", arguments => new CharacterGASPUnGrabPropEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Prop = ScriptEventArguments.Get<Prop?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPUnPossessEvent)] = new("CharacterGASP", "UnPossess", arguments => new CharacterGASPUnPossessEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            OldPlayer = ScriptEventArguments.Get<Player?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPValueChangeEvent)] = new("CharacterGASP", "ValueChange", arguments => new CharacterGASPValueChangeEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(CharacterSimpleClassRegisterEvent)] = new("CharacterSimple", "ClassRegister", arguments => new CharacterSimpleClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(CharacterSimpleDeathEvent)] = new("CharacterSimple", "Death", arguments => new CharacterSimpleDeathEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            LastDamageTaken = ScriptEventArguments.Get<int>(arguments, 1)!,
            LastBoneDamaged = ScriptEventArguments.Get<string?>(arguments, 2),
            DamageTypeReason = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            HitFromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        });
        descriptors[typeof(CharacterSimpleDestroyEvent)] = new("CharacterSimple", "Destroy", arguments => new CharacterSimpleDestroyEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        });
        descriptors[typeof(CharacterSimpleEnterWaterEvent)] = new("CharacterSimple", "EnterWater", arguments => new CharacterSimpleEnterWaterEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        });
        descriptors[typeof(CharacterSimpleHealthChangeEvent)] = new("CharacterSimple", "HealthChange", arguments => new CharacterSimpleHealthChangeEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            OldHealth = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewHealth = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterSimpleLeaveWaterEvent)] = new("CharacterSimple", "LeaveWater", arguments => new CharacterSimpleLeaveWaterEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        });
        descriptors[typeof(CharacterSimpleMoveCompleteEvent)] = new("CharacterSimple", "MoveComplete", arguments => new CharacterSimpleMoveCompleteEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            Succeeded = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(CharacterSimplePossessEvent)] = new("CharacterSimple", "Possess", arguments => new CharacterSimplePossessEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            Player = ScriptEventArguments.Get<Player?>(arguments, 1),
        });
        descriptors[typeof(CharacterSimpleRespawnEvent)] = new("CharacterSimple", "Respawn", arguments => new CharacterSimpleRespawnEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        });
        descriptors[typeof(CharacterSimpleSpawnEvent)] = new("CharacterSimple", "Spawn", arguments => new CharacterSimpleSpawnEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        });
        descriptors[typeof(CharacterSimpleTakeDamageEvent)] = new("CharacterSimple", "TakeDamage", arguments => new CharacterSimpleTakeDamageEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<object?>(arguments, 6),
        });
        descriptors[typeof(CharacterSimpleUnPossessEvent)] = new("CharacterSimple", "UnPossess", arguments => new CharacterSimpleUnPossessEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            OldPlayer = ScriptEventArguments.Get<Player?>(arguments, 1),
        });
        descriptors[typeof(CharacterSimpleValueChangeEvent)] = new("CharacterSimple", "ValueChange", arguments => new CharacterSimpleValueChangeEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(ChatPlayerSubmitEvent)] = new("Chat", "PlayerSubmit", arguments => new ChatPlayerSubmitEvent
        {
            Message = ScriptEventArguments.Get<string?>(arguments, 0),
            Player = ScriptEventArguments.Get<Player?>(arguments, 1),
        });
        descriptors[typeof(GrenadeClassRegisterEvent)] = new("Grenade", "ClassRegister", arguments => new GrenadeClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(GrenadeDestroyEvent)] = new("Grenade", "Destroy", arguments => new GrenadeDestroyEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
        });
        descriptors[typeof(GrenadeDropEvent)] = new("Grenade", "Drop", arguments => new GrenadeDropEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            WasTriggeredByPlayer = ScriptEventArguments.Get<bool>(arguments, 2)!,
        });
        descriptors[typeof(GrenadeEnterWaterEvent)] = new("Grenade", "EnterWater", arguments => new GrenadeEnterWaterEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
        });
        descriptors[typeof(GrenadeExplodeEvent)] = new("Grenade", "Explode", arguments => new GrenadeExplodeEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
        });
        descriptors[typeof(GrenadeHitEvent)] = new("Grenade", "Hit", arguments => new GrenadeHitEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        });
        descriptors[typeof(GrenadeLeaveWaterEvent)] = new("Grenade", "LeaveWater", arguments => new GrenadeLeaveWaterEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
        });
        descriptors[typeof(GrenadePickUpEvent)] = new("Grenade", "PickUp", arguments => new GrenadePickUpEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(GrenadePullUseEvent)] = new("Grenade", "PullUse", arguments => new GrenadePullUseEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(GrenadeReleaseUseEvent)] = new("Grenade", "ReleaseUse", arguments => new GrenadeReleaseUseEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(GrenadeSpawnEvent)] = new("Grenade", "Spawn", arguments => new GrenadeSpawnEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
        });
        descriptors[typeof(GrenadeThrowEvent)] = new("Grenade", "Throw", arguments => new GrenadeThrowEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Handler = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(GrenadeValueChangeEvent)] = new("Grenade", "ValueChange", arguments => new GrenadeValueChangeEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(InstancedStaticMeshClassRegisterEvent)] = new("InstancedStaticMesh", "ClassRegister", arguments => new InstancedStaticMeshClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(InstancedStaticMeshDestroyEvent)] = new("InstancedStaticMesh", "Destroy", arguments => new InstancedStaticMeshDestroyEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
        });
        descriptors[typeof(InstancedStaticMeshEnterWaterEvent)] = new("InstancedStaticMesh", "EnterWater", arguments => new InstancedStaticMeshEnterWaterEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
        });
        descriptors[typeof(InstancedStaticMeshLeaveWaterEvent)] = new("InstancedStaticMesh", "LeaveWater", arguments => new InstancedStaticMeshLeaveWaterEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
        });
        descriptors[typeof(InstancedStaticMeshSpawnEvent)] = new("InstancedStaticMesh", "Spawn", arguments => new InstancedStaticMeshSpawnEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
        });
        descriptors[typeof(InstancedStaticMeshValueChangeEvent)] = new("InstancedStaticMesh", "ValueChange", arguments => new InstancedStaticMeshValueChangeEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(LightClassRegisterEvent)] = new("Light", "ClassRegister", arguments => new LightClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(LightDestroyEvent)] = new("Light", "Destroy", arguments => new LightDestroyEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
        });
        descriptors[typeof(LightEnterWaterEvent)] = new("Light", "EnterWater", arguments => new LightEnterWaterEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
        });
        descriptors[typeof(LightLeaveWaterEvent)] = new("Light", "LeaveWater", arguments => new LightLeaveWaterEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
        });
        descriptors[typeof(LightSpawnEvent)] = new("Light", "Spawn", arguments => new LightSpawnEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
        });
        descriptors[typeof(LightValueChangeEvent)] = new("Light", "ValueChange", arguments => new LightValueChangeEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(MeleeAttackEvent)] = new("Melee", "Attack", arguments => new MeleeAttackEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Handler = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(MeleeClassRegisterEvent)] = new("Melee", "ClassRegister", arguments => new MeleeClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(MeleeDestroyEvent)] = new("Melee", "Destroy", arguments => new MeleeDestroyEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
        });
        descriptors[typeof(MeleeDropEvent)] = new("Melee", "Drop", arguments => new MeleeDropEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            WasTriggeredByPlayer = ScriptEventArguments.Get<bool>(arguments, 2)!,
        });
        descriptors[typeof(MeleeEnterWaterEvent)] = new("Melee", "EnterWater", arguments => new MeleeEnterWaterEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
        });
        descriptors[typeof(MeleeHitEvent)] = new("Melee", "Hit", arguments => new MeleeHitEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        });
        descriptors[typeof(MeleeLeaveWaterEvent)] = new("Melee", "LeaveWater", arguments => new MeleeLeaveWaterEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
        });
        descriptors[typeof(MeleePickUpEvent)] = new("Melee", "PickUp", arguments => new MeleePickUpEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(MeleePullUseEvent)] = new("Melee", "PullUse", arguments => new MeleePullUseEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(MeleeReleaseUseEvent)] = new("Melee", "ReleaseUse", arguments => new MeleeReleaseUseEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(MeleeSpawnEvent)] = new("Melee", "Spawn", arguments => new MeleeSpawnEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
        });
        descriptors[typeof(MeleeValueChangeEvent)] = new("Melee", "ValueChange", arguments => new MeleeValueChangeEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(ParticleClassRegisterEvent)] = new("Particle", "ClassRegister", arguments => new ParticleClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(ParticleDestroyEvent)] = new("Particle", "Destroy", arguments => new ParticleDestroyEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
        });
        descriptors[typeof(ParticleEnterWaterEvent)] = new("Particle", "EnterWater", arguments => new ParticleEnterWaterEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
        });
        descriptors[typeof(ParticleLeaveWaterEvent)] = new("Particle", "LeaveWater", arguments => new ParticleLeaveWaterEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
        });
        descriptors[typeof(ParticleSpawnEvent)] = new("Particle", "Spawn", arguments => new ParticleSpawnEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
        });
        descriptors[typeof(ParticleValueChangeEvent)] = new("Particle", "ValueChange", arguments => new ParticleValueChangeEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(PlayerClassRegisterEvent)] = new("Player", "ClassRegister", arguments => new PlayerClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(PlayerDestroyEvent)] = new("Player", "Destroy", arguments => new PlayerDestroyEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
        });
        descriptors[typeof(PlayerPossessEvent)] = new("Player", "Possess", arguments => new PlayerPossessEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            Pawn = ScriptEventArguments.Get<Entity?>(arguments, 1),
        });
        descriptors[typeof(PlayerSpawnEvent)] = new("Player", "Spawn", arguments => new PlayerSpawnEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
        });
        descriptors[typeof(PlayerUnPossessEvent)] = new("Player", "UnPossess", arguments => new PlayerUnPossessEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            Pawn = ScriptEventArguments.Get<Entity?>(arguments, 1),
        });
        descriptors[typeof(PlayerVOIPGlobalChannelSettingChangeEvent)] = new("Player", "VOIPGlobalChannelSettingChange", arguments => new PlayerVOIPGlobalChannelSettingChangeEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            Channel = ScriptEventArguments.Get<int>(arguments, 1)!,
            OldSetting = ScriptEventArguments.Get<VOIPSetting>(arguments, 2)!,
            NewSetting = ScriptEventArguments.Get<VOIPSetting>(arguments, 3)!,
        });
        descriptors[typeof(PlayerVOIPLocalSettingChangeEvent)] = new("Player", "VOIPLocalSettingChange", arguments => new PlayerVOIPLocalSettingChangeEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            OldSetting = ScriptEventArguments.Get<VOIPSetting>(arguments, 1)!,
            NewSetting = ScriptEventArguments.Get<VOIPSetting>(arguments, 2)!,
        });
        descriptors[typeof(PlayerValueChangeEvent)] = new("Player", "ValueChange", arguments => new PlayerValueChangeEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(PropClassRegisterEvent)] = new("Prop", "ClassRegister", arguments => new PropClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(PropDestroyEvent)] = new("Prop", "Destroy", arguments => new PropDestroyEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
        });
        descriptors[typeof(PropEnterWaterEvent)] = new("Prop", "EnterWater", arguments => new PropEnterWaterEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
        });
        descriptors[typeof(PropGrabEvent)] = new("Prop", "Grab", arguments => new PropGrabEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(PropHitEvent)] = new("Prop", "Hit", arguments => new PropHitEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        });
        descriptors[typeof(PropLeaveWaterEvent)] = new("Prop", "LeaveWater", arguments => new PropLeaveWaterEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
        });
        descriptors[typeof(PropSpawnEvent)] = new("Prop", "Spawn", arguments => new PropSpawnEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
        });
        descriptors[typeof(PropUnGrabEvent)] = new("Prop", "UnGrab", arguments => new PropUnGrabEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(PropValueChangeEvent)] = new("Prop", "ValueChange", arguments => new PropValueChangeEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(ScriptConsoleLogEntryEvent)] = new("Console_I", "LogEntry", arguments => new ScriptConsoleLogEntryEvent
        {
            Text = ScriptEventArguments.Get<string?>(arguments, 0),
            Type = ScriptEventArguments.Get<LogType>(arguments, 1)!,
        });
        descriptors[typeof(ScriptConsolePlayerSubmitEvent)] = new("Console_I", "PlayerSubmit", arguments => new ScriptConsolePlayerSubmitEvent
        {
            Text = ScriptEventArguments.Get<string?>(arguments, 0),
        });
        descriptors[typeof(StaticMeshClassRegisterEvent)] = new("StaticMesh", "ClassRegister", arguments => new StaticMeshClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(StaticMeshDestroyEvent)] = new("StaticMesh", "Destroy", arguments => new StaticMeshDestroyEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
        });
        descriptors[typeof(StaticMeshEnterWaterEvent)] = new("StaticMesh", "EnterWater", arguments => new StaticMeshEnterWaterEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
        });
        descriptors[typeof(StaticMeshLeaveWaterEvent)] = new("StaticMesh", "LeaveWater", arguments => new StaticMeshLeaveWaterEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
        });
        descriptors[typeof(StaticMeshSpawnEvent)] = new("StaticMesh", "Spawn", arguments => new StaticMeshSpawnEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
        });
        descriptors[typeof(StaticMeshValueChangeEvent)] = new("StaticMesh", "ValueChange", arguments => new StaticMeshValueChangeEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(Text3DClassRegisterEvent)] = new("Text3D", "ClassRegister", arguments => new Text3DClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(Text3DDestroyEvent)] = new("Text3D", "Destroy", arguments => new Text3DDestroyEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
        });
        descriptors[typeof(Text3DEnterWaterEvent)] = new("Text3D", "EnterWater", arguments => new Text3DEnterWaterEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
        });
        descriptors[typeof(Text3DLeaveWaterEvent)] = new("Text3D", "LeaveWater", arguments => new Text3DLeaveWaterEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
        });
        descriptors[typeof(Text3DSpawnEvent)] = new("Text3D", "Spawn", arguments => new Text3DSpawnEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
        });
        descriptors[typeof(Text3DValueChangeEvent)] = new("Text3D", "ValueChange", arguments => new Text3DValueChangeEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(TextRenderClassRegisterEvent)] = new("TextRender", "ClassRegister", arguments => new TextRenderClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(TextRenderDestroyEvent)] = new("TextRender", "Destroy", arguments => new TextRenderDestroyEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
        });
        descriptors[typeof(TextRenderEnterWaterEvent)] = new("TextRender", "EnterWater", arguments => new TextRenderEnterWaterEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
        });
        descriptors[typeof(TextRenderLeaveWaterEvent)] = new("TextRender", "LeaveWater", arguments => new TextRenderLeaveWaterEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
        });
        descriptors[typeof(TextRenderSpawnEvent)] = new("TextRender", "Spawn", arguments => new TextRenderSpawnEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
        });
        descriptors[typeof(TextRenderValueChangeEvent)] = new("TextRender", "ValueChange", arguments => new TextRenderValueChangeEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(TriggerBeginOverlapEvent)] = new("Trigger", "BeginOverlap", arguments => new TriggerBeginOverlapEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            Entity = ScriptEventArguments.Get<Actor?>(arguments, 1),
        });
        descriptors[typeof(TriggerClassRegisterEvent)] = new("Trigger", "ClassRegister", arguments => new TriggerClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(TriggerDestroyEvent)] = new("Trigger", "Destroy", arguments => new TriggerDestroyEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
        });
        descriptors[typeof(TriggerEndOverlapEvent)] = new("Trigger", "EndOverlap", arguments => new TriggerEndOverlapEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            Entity = ScriptEventArguments.Get<Actor?>(arguments, 1),
        });
        descriptors[typeof(TriggerEnterWaterEvent)] = new("Trigger", "EnterWater", arguments => new TriggerEnterWaterEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
        });
        descriptors[typeof(TriggerLeaveWaterEvent)] = new("Trigger", "LeaveWater", arguments => new TriggerLeaveWaterEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
        });
        descriptors[typeof(TriggerSpawnEvent)] = new("Trigger", "Spawn", arguments => new TriggerSpawnEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
        });
        descriptors[typeof(TriggerValueChangeEvent)] = new("Trigger", "ValueChange", arguments => new TriggerValueChangeEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(VehicleWaterCharacterEnterEvent)] = new("VehicleWater", "CharacterEnter", arguments => new VehicleWaterCharacterEnterEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(VehicleWaterCharacterLeaveEvent)] = new("VehicleWater", "CharacterLeave", arguments => new VehicleWaterCharacterLeaveEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(VehicleWaterClassRegisterEvent)] = new("VehicleWater", "ClassRegister", arguments => new VehicleWaterClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(VehicleWaterDeathEvent)] = new("VehicleWater", "Death", arguments => new VehicleWaterDeathEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            LastDamageTaken = ScriptEventArguments.Get<int>(arguments, 1)!,
            LastBoneDamaged = ScriptEventArguments.Get<string?>(arguments, 2),
            DamageTypeReason = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            HitFromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        });
        descriptors[typeof(VehicleWaterDestroyEvent)] = new("VehicleWater", "Destroy", arguments => new VehicleWaterDestroyEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWaterEngineStartEvent)] = new("VehicleWater", "EngineStart", arguments => new VehicleWaterEngineStartEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWaterEngineStopEvent)] = new("VehicleWater", "EngineStop", arguments => new VehicleWaterEngineStopEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWaterEnterWaterEvent)] = new("VehicleWater", "EnterWater", arguments => new VehicleWaterEnterWaterEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWaterHealthChangeEvent)] = new("VehicleWater", "HealthChange", arguments => new VehicleWaterHealthChangeEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            OldHealth = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewHealth = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(VehicleWaterHitEvent)] = new("VehicleWater", "Hit", arguments => new VehicleWaterHitEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        });
        descriptors[typeof(VehicleWaterLeaveWaterEvent)] = new("VehicleWater", "LeaveWater", arguments => new VehicleWaterLeaveWaterEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWaterRespawnEvent)] = new("VehicleWater", "Respawn", arguments => new VehicleWaterRespawnEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWaterSpawnEvent)] = new("VehicleWater", "Spawn", arguments => new VehicleWaterSpawnEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWaterTakeDamageEvent)] = new("VehicleWater", "TakeDamage", arguments => new VehicleWaterTakeDamageEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<object?>(arguments, 6),
        });
        descriptors[typeof(VehicleWaterValueChangeEvent)] = new("VehicleWater", "ValueChange", arguments => new VehicleWaterValueChangeEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(VehicleWheeledCharacterEnterEvent)] = new("VehicleWheeled", "CharacterEnter", arguments => new VehicleWheeledCharacterEnterEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(VehicleWheeledCharacterLeaveEvent)] = new("VehicleWheeled", "CharacterLeave", arguments => new VehicleWheeledCharacterLeaveEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(VehicleWheeledClassRegisterEvent)] = new("VehicleWheeled", "ClassRegister", arguments => new VehicleWheeledClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(VehicleWheeledDeathEvent)] = new("VehicleWheeled", "Death", arguments => new VehicleWheeledDeathEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            LastDamageTaken = ScriptEventArguments.Get<int>(arguments, 1)!,
            LastBoneDamaged = ScriptEventArguments.Get<string?>(arguments, 2),
            DamageTypeReason = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            HitFromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        });
        descriptors[typeof(VehicleWheeledDestroyEvent)] = new("VehicleWheeled", "Destroy", arguments => new VehicleWheeledDestroyEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWheeledEngineStartEvent)] = new("VehicleWheeled", "EngineStart", arguments => new VehicleWheeledEngineStartEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWheeledEngineStopEvent)] = new("VehicleWheeled", "EngineStop", arguments => new VehicleWheeledEngineStopEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWheeledEnterWaterEvent)] = new("VehicleWheeled", "EnterWater", arguments => new VehicleWheeledEnterWaterEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWheeledHealthChangeEvent)] = new("VehicleWheeled", "HealthChange", arguments => new VehicleWheeledHealthChangeEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            OldHealth = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewHealth = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(VehicleWheeledHitEvent)] = new("VehicleWheeled", "Hit", arguments => new VehicleWheeledHitEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        });
        descriptors[typeof(VehicleWheeledHornEvent)] = new("VehicleWheeled", "Horn", arguments => new VehicleWheeledHornEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            IsHonking = ScriptEventArguments.Get<bool>(arguments, 1)!,
        });
        descriptors[typeof(VehicleWheeledLeaveWaterEvent)] = new("VehicleWheeled", "LeaveWater", arguments => new VehicleWheeledLeaveWaterEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWheeledRespawnEvent)] = new("VehicleWheeled", "Respawn", arguments => new VehicleWheeledRespawnEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWheeledSpawnEvent)] = new("VehicleWheeled", "Spawn", arguments => new VehicleWheeledSpawnEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        });
        descriptors[typeof(VehicleWheeledTakeDamageEvent)] = new("VehicleWheeled", "TakeDamage", arguments => new VehicleWheeledTakeDamageEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<object?>(arguments, 6),
        });
        descriptors[typeof(VehicleWheeledValueChangeEvent)] = new("VehicleWheeled", "ValueChange", arguments => new VehicleWheeledValueChangeEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
        descriptors[typeof(WeaponAmmoBagChangeEvent)] = new("Weapon", "AmmoBagChange", arguments => new WeaponAmmoBagChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            OldAmmoBag = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewAmmoBag = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(WeaponAmmoClipChangeEvent)] = new("Weapon", "AmmoClipChange", arguments => new WeaponAmmoClipChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            OldAmmoClip = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewAmmoClip = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(WeaponClassRegisterEvent)] = new("Weapon", "ClassRegister", arguments => new WeaponClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        });
        descriptors[typeof(WeaponDestroyEvent)] = new("Weapon", "Destroy", arguments => new WeaponDestroyEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
        });
        descriptors[typeof(WeaponDropEvent)] = new("Weapon", "Drop", arguments => new WeaponDropEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            WasTriggeredByPlayer = ScriptEventArguments.Get<bool>(arguments, 2)!,
        });
        descriptors[typeof(WeaponEnterWaterEvent)] = new("Weapon", "EnterWater", arguments => new WeaponEnterWaterEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
        });
        descriptors[typeof(WeaponHitEvent)] = new("Weapon", "Hit", arguments => new WeaponHitEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        });
        descriptors[typeof(WeaponLeaveWaterEvent)] = new("Weapon", "LeaveWater", arguments => new WeaponLeaveWaterEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
        });
        descriptors[typeof(WeaponPickUpEvent)] = new("Weapon", "PickUp", arguments => new WeaponPickUpEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(WeaponPullUseEvent)] = new("Weapon", "PullUse", arguments => new WeaponPullUseEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(WeaponReleaseUseEvent)] = new("Weapon", "ReleaseUse", arguments => new WeaponReleaseUseEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(WeaponReloadEvent)] = new("Weapon", "Reload", arguments => new WeaponReloadEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            AmmoToReload = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(WeaponSpawnEvent)] = new("Weapon", "Spawn", arguments => new WeaponSpawnEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
        });
        descriptors[typeof(WeaponValueChangeEvent)] = new("Weapon", "ValueChange", arguments => new WeaponValueChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        });
    }
}
