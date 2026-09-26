#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

internal static partial class ScriptEventCatalog
{
    private static partial Dictionary<Type, ScriptEventDescriptor> CreateDescriptors() => new()
    {
        [typeof(BlueprintClassRegisterEvent)] = new("Blueprint", "ClassRegister", arguments => new BlueprintClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(BlueprintDestroyEvent)] = new("Blueprint", "Destroy", arguments => new BlueprintDestroyEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
        }),
        [typeof(BlueprintDimensionChangeEvent)] = new("Blueprint", "DimensionChange", arguments => new BlueprintDimensionChangeEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(BlueprintEnterWaterEvent)] = new("Blueprint", "EnterWater", arguments => new BlueprintEnterWaterEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
        }),
        [typeof(BlueprintLeaveWaterEvent)] = new("Blueprint", "LeaveWater", arguments => new BlueprintLeaveWaterEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
        }),
        [typeof(BlueprintNetworkAuthorityChangeEvent)] = new("Blueprint", "NetworkAuthorityChange", arguments => new BlueprintNetworkAuthorityChangeEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(BlueprintSpawnEvent)] = new("Blueprint", "Spawn", arguments => new BlueprintSpawnEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
        }),
        [typeof(BlueprintValueChangeEvent)] = new("Blueprint", "ValueChange", arguments => new BlueprintValueChangeEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(CableClassRegisterEvent)] = new("Cable", "ClassRegister", arguments => new CableClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(CableDestroyEvent)] = new("Cable", "Destroy", arguments => new CableDestroyEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
        }),
        [typeof(CableDimensionChangeEvent)] = new("Cable", "DimensionChange", arguments => new CableDimensionChangeEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(CableEnterWaterEvent)] = new("Cable", "EnterWater", arguments => new CableEnterWaterEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
        }),
        [typeof(CableLeaveWaterEvent)] = new("Cable", "LeaveWater", arguments => new CableLeaveWaterEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
        }),
        [typeof(CableNetworkAuthorityChangeEvent)] = new("Cable", "NetworkAuthorityChange", arguments => new CableNetworkAuthorityChangeEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(CableSpawnEvent)] = new("Cable", "Spawn", arguments => new CableSpawnEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
        }),
        [typeof(CableValueChangeEvent)] = new("Cable", "ValueChange", arguments => new CableValueChangeEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(CharacterAttackEvent)] = new("Character", "Attack", arguments => new CharacterAttackEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Melee = ScriptEventArguments.Get<Melee?>(arguments, 1),
        }),
        [typeof(CharacterAttemptEnterVehicleEvent)] = new("Character", "AttemptEnterVehicle", arguments => new CharacterAttemptEnterVehicleEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(CharacterAttemptLeaveVehicleEvent)] = new("Character", "AttemptLeaveVehicle", arguments => new CharacterAttemptLeaveVehicleEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
        }),
        [typeof(CharacterAttemptReloadEvent)] = new("Character", "AttemptReload", arguments => new CharacterAttemptReloadEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Weapon = ScriptEventArguments.Get<Weapon?>(arguments, 1),
        }),
        [typeof(CharacterClassRegisterEvent)] = new("Character", "ClassRegister", arguments => new CharacterClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(CharacterDeathEvent)] = new("Character", "Death", arguments => new CharacterDeathEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            LastDamageTaken = ScriptEventArguments.Get<int>(arguments, 1)!,
            LastBoneDamaged = ScriptEventArguments.Get<string?>(arguments, 2),
            DamageTypeReason = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            HitFromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        }),
        [typeof(CharacterDestroyEvent)] = new("Character", "Destroy", arguments => new CharacterDestroyEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        }),
        [typeof(CharacterDimensionChangeEvent)] = new("Character", "DimensionChange", arguments => new CharacterDimensionChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(CharacterDropEvent)] = new("Character", "Drop", arguments => new CharacterDropEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Object = ScriptEventArguments.Get<Entity?>(arguments, 1),
            TriggeredByPlayer = ScriptEventArguments.Get<bool>(arguments, 2)!,
        }),
        [typeof(CharacterEnterVehicleEvent)] = new("Character", "EnterVehicle", arguments => new CharacterEnterVehicleEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(CharacterEnterWaterEvent)] = new("Character", "EnterWater", arguments => new CharacterEnterWaterEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        }),
        [typeof(CharacterGrabPropEvent)] = new("Character", "GrabProp", arguments => new CharacterGrabPropEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Prop = ScriptEventArguments.Get<Prop?>(arguments, 1),
        }),
        [typeof(CharacterHealthChangeEvent)] = new("Character", "HealthChange", arguments => new CharacterHealthChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            OldHealth = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewHealth = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(CharacterHighlightEvent)] = new("Character", "Highlight", arguments => new CharacterHighlightEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            IsHighlighted = ScriptEventArguments.Get<bool>(arguments, 1)!,
            Object = ScriptEventArguments.Get<Prop?>(arguments, 2),
        }),
        [typeof(CharacterInteractEvent)] = new("Character", "Interact", arguments => new CharacterInteractEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Object = ScriptEventArguments.Get<Prop?>(arguments, 1),
        }),
        [typeof(CharacterLeaveVehicleEvent)] = new("Character", "LeaveVehicle", arguments => new CharacterLeaveVehicleEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(CharacterLeaveWaterEvent)] = new("Character", "LeaveWater", arguments => new CharacterLeaveWaterEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        }),
        [typeof(CharacterMoveCompleteEvent)] = new("Character", "MoveComplete", arguments => new CharacterMoveCompleteEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Succeeded = ScriptEventArguments.Get<bool>(arguments, 1)!,
        }),
        [typeof(CharacterNetworkAuthorityChangeEvent)] = new("Character", "NetworkAuthorityChange", arguments => new CharacterNetworkAuthorityChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(CharacterPickUpEvent)] = new("Character", "PickUp", arguments => new CharacterPickUpEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Object = ScriptEventArguments.Get<Entity?>(arguments, 1),
        }),
        [typeof(CharacterPossessEvent)] = new("Character", "Possess", arguments => new CharacterPossessEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Player = ScriptEventArguments.Get<Player?>(arguments, 1),
        }),
        [typeof(CharacterPullUseEvent)] = new("Character", "PullUse", arguments => new CharacterPullUseEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Pickable = ScriptEventArguments.Get<Entity?>(arguments, 1),
        }),
        [typeof(CharacterPunchEvent)] = new("Character", "Punch", arguments => new CharacterPunchEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        }),
        [typeof(CharacterRagdollModeChangeEvent)] = new("Character", "RagdollModeChange", arguments => new CharacterRagdollModeChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            OldState = ScriptEventArguments.Get<bool>(arguments, 1)!,
            NewState = ScriptEventArguments.Get<bool>(arguments, 2)!,
        }),
        [typeof(CharacterReleaseUseEvent)] = new("Character", "ReleaseUse", arguments => new CharacterReleaseUseEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Pickable = ScriptEventArguments.Get<Entity?>(arguments, 1),
        }),
        [typeof(CharacterReloadEvent)] = new("Character", "Reload", arguments => new CharacterReloadEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Weapon = ScriptEventArguments.Get<Weapon?>(arguments, 1),
            AmmoToReload = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(CharacterRespawnEvent)] = new("Character", "Respawn", arguments => new CharacterRespawnEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        }),
        [typeof(CharacterSpawnEvent)] = new("Character", "Spawn", arguments => new CharacterSpawnEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
        }),
        [typeof(CharacterTakeDamageEvent)] = new("Character", "TakeDamage", arguments => new CharacterTakeDamageEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<object?>(arguments, 6),
        }),
        [typeof(CharacterUnGrabPropEvent)] = new("Character", "UnGrabProp", arguments => new CharacterUnGrabPropEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Prop = ScriptEventArguments.Get<Prop?>(arguments, 1),
        }),
        [typeof(CharacterUnPossessEvent)] = new("Character", "UnPossess", arguments => new CharacterUnPossessEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            OldPlayer = ScriptEventArguments.Get<Player?>(arguments, 1),
        }),
        [typeof(CharacterValueChangeEvent)] = new("Character", "ValueChange", arguments => new CharacterValueChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(CharacterSimpleClassRegisterEvent)] = new("CharacterSimple", "ClassRegister", arguments => new CharacterSimpleClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(CharacterSimpleDeathEvent)] = new("CharacterSimple", "Death", arguments => new CharacterSimpleDeathEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            LastDamageTaken = ScriptEventArguments.Get<int>(arguments, 1)!,
            LastBoneDamaged = ScriptEventArguments.Get<string?>(arguments, 2),
            DamageTypeReason = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            HitFromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        }),
        [typeof(CharacterSimpleDestroyEvent)] = new("CharacterSimple", "Destroy", arguments => new CharacterSimpleDestroyEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        }),
        [typeof(CharacterSimpleDimensionChangeEvent)] = new("CharacterSimple", "DimensionChange", arguments => new CharacterSimpleDimensionChangeEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(CharacterSimpleEnterWaterEvent)] = new("CharacterSimple", "EnterWater", arguments => new CharacterSimpleEnterWaterEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        }),
        [typeof(CharacterSimpleHealthChangeEvent)] = new("CharacterSimple", "HealthChange", arguments => new CharacterSimpleHealthChangeEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            OldHealth = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewHealth = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(CharacterSimpleLeaveWaterEvent)] = new("CharacterSimple", "LeaveWater", arguments => new CharacterSimpleLeaveWaterEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        }),
        [typeof(CharacterSimpleMoveCompleteEvent)] = new("CharacterSimple", "MoveComplete", arguments => new CharacterSimpleMoveCompleteEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            Succeeded = ScriptEventArguments.Get<bool>(arguments, 1)!,
        }),
        [typeof(CharacterSimpleNetworkAuthorityChangeEvent)] = new("CharacterSimple", "NetworkAuthorityChange", arguments => new CharacterSimpleNetworkAuthorityChangeEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(CharacterSimplePossessEvent)] = new("CharacterSimple", "Possess", arguments => new CharacterSimplePossessEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            Player = ScriptEventArguments.Get<Player?>(arguments, 1),
        }),
        [typeof(CharacterSimpleRespawnEvent)] = new("CharacterSimple", "Respawn", arguments => new CharacterSimpleRespawnEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        }),
        [typeof(CharacterSimpleSpawnEvent)] = new("CharacterSimple", "Spawn", arguments => new CharacterSimpleSpawnEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
        }),
        [typeof(CharacterSimpleTakeDamageEvent)] = new("CharacterSimple", "TakeDamage", arguments => new CharacterSimpleTakeDamageEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<object?>(arguments, 6),
        }),
        [typeof(CharacterSimpleUnPossessEvent)] = new("CharacterSimple", "UnPossess", arguments => new CharacterSimpleUnPossessEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            OldPlayer = ScriptEventArguments.Get<Player?>(arguments, 1),
        }),
        [typeof(CharacterSimpleValueChangeEvent)] = new("CharacterSimple", "ValueChange", arguments => new CharacterSimpleValueChangeEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(ChatPlayerSubmitEvent)] = new("Chat", "PlayerSubmit", arguments => new ChatPlayerSubmitEvent
        {
            Message = ScriptEventArguments.Get<string?>(arguments, 0),
            Player = ScriptEventArguments.Get<Player?>(arguments, 1),
        }),
        [typeof(GrenadeClassRegisterEvent)] = new("Grenade", "ClassRegister", arguments => new GrenadeClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(GrenadeDestroyEvent)] = new("Grenade", "Destroy", arguments => new GrenadeDestroyEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
        }),
        [typeof(GrenadeDimensionChangeEvent)] = new("Grenade", "DimensionChange", arguments => new GrenadeDimensionChangeEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(GrenadeDropEvent)] = new("Grenade", "Drop", arguments => new GrenadeDropEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            WasTriggeredByPlayer = ScriptEventArguments.Get<bool>(arguments, 2)!,
        }),
        [typeof(GrenadeEnterWaterEvent)] = new("Grenade", "EnterWater", arguments => new GrenadeEnterWaterEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
        }),
        [typeof(GrenadeExplodeEvent)] = new("Grenade", "Explode", arguments => new GrenadeExplodeEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
        }),
        [typeof(GrenadeHitEvent)] = new("Grenade", "Hit", arguments => new GrenadeHitEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        }),
        [typeof(GrenadeInteractEvent)] = new("Grenade", "Interact", arguments => new GrenadeInteractEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(GrenadeLeaveWaterEvent)] = new("Grenade", "LeaveWater", arguments => new GrenadeLeaveWaterEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
        }),
        [typeof(GrenadeNetworkAuthorityChangeEvent)] = new("Grenade", "NetworkAuthorityChange", arguments => new GrenadeNetworkAuthorityChangeEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(GrenadePickUpEvent)] = new("Grenade", "PickUp", arguments => new GrenadePickUpEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(GrenadePullUseEvent)] = new("Grenade", "PullUse", arguments => new GrenadePullUseEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(GrenadeReleaseUseEvent)] = new("Grenade", "ReleaseUse", arguments => new GrenadeReleaseUseEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(GrenadeSpawnEvent)] = new("Grenade", "Spawn", arguments => new GrenadeSpawnEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
        }),
        [typeof(GrenadeThrowEvent)] = new("Grenade", "Throw", arguments => new GrenadeThrowEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Handler = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(GrenadeValueChangeEvent)] = new("Grenade", "ValueChange", arguments => new GrenadeValueChangeEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(InstancedStaticMeshClassRegisterEvent)] = new("InstancedStaticMesh", "ClassRegister", arguments => new InstancedStaticMeshClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(InstancedStaticMeshDestroyEvent)] = new("InstancedStaticMesh", "Destroy", arguments => new InstancedStaticMeshDestroyEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
        }),
        [typeof(InstancedStaticMeshDimensionChangeEvent)] = new("InstancedStaticMesh", "DimensionChange", arguments => new InstancedStaticMeshDimensionChangeEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(InstancedStaticMeshEnterWaterEvent)] = new("InstancedStaticMesh", "EnterWater", arguments => new InstancedStaticMeshEnterWaterEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
        }),
        [typeof(InstancedStaticMeshLeaveWaterEvent)] = new("InstancedStaticMesh", "LeaveWater", arguments => new InstancedStaticMeshLeaveWaterEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
        }),
        [typeof(InstancedStaticMeshNetworkAuthorityChangeEvent)] = new("InstancedStaticMesh", "NetworkAuthorityChange", arguments => new InstancedStaticMeshNetworkAuthorityChangeEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(InstancedStaticMeshSpawnEvent)] = new("InstancedStaticMesh", "Spawn", arguments => new InstancedStaticMeshSpawnEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
        }),
        [typeof(InstancedStaticMeshValueChangeEvent)] = new("InstancedStaticMesh", "ValueChange", arguments => new InstancedStaticMeshValueChangeEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(LightClassRegisterEvent)] = new("Light", "ClassRegister", arguments => new LightClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(LightDestroyEvent)] = new("Light", "Destroy", arguments => new LightDestroyEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
        }),
        [typeof(LightDimensionChangeEvent)] = new("Light", "DimensionChange", arguments => new LightDimensionChangeEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(LightEnterWaterEvent)] = new("Light", "EnterWater", arguments => new LightEnterWaterEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
        }),
        [typeof(LightLeaveWaterEvent)] = new("Light", "LeaveWater", arguments => new LightLeaveWaterEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
        }),
        [typeof(LightNetworkAuthorityChangeEvent)] = new("Light", "NetworkAuthorityChange", arguments => new LightNetworkAuthorityChangeEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(LightSpawnEvent)] = new("Light", "Spawn", arguments => new LightSpawnEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
        }),
        [typeof(LightValueChangeEvent)] = new("Light", "ValueChange", arguments => new LightValueChangeEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(MeleeAttackEvent)] = new("Melee", "Attack", arguments => new MeleeAttackEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Handler = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(MeleeClassRegisterEvent)] = new("Melee", "ClassRegister", arguments => new MeleeClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(MeleeDestroyEvent)] = new("Melee", "Destroy", arguments => new MeleeDestroyEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
        }),
        [typeof(MeleeDimensionChangeEvent)] = new("Melee", "DimensionChange", arguments => new MeleeDimensionChangeEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(MeleeDropEvent)] = new("Melee", "Drop", arguments => new MeleeDropEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            WasTriggeredByPlayer = ScriptEventArguments.Get<bool>(arguments, 2)!,
        }),
        [typeof(MeleeEnterWaterEvent)] = new("Melee", "EnterWater", arguments => new MeleeEnterWaterEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
        }),
        [typeof(MeleeHitEvent)] = new("Melee", "Hit", arguments => new MeleeHitEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        }),
        [typeof(MeleeInteractEvent)] = new("Melee", "Interact", arguments => new MeleeInteractEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(MeleeLeaveWaterEvent)] = new("Melee", "LeaveWater", arguments => new MeleeLeaveWaterEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
        }),
        [typeof(MeleeNetworkAuthorityChangeEvent)] = new("Melee", "NetworkAuthorityChange", arguments => new MeleeNetworkAuthorityChangeEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(MeleePickUpEvent)] = new("Melee", "PickUp", arguments => new MeleePickUpEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(MeleePullUseEvent)] = new("Melee", "PullUse", arguments => new MeleePullUseEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(MeleeReleaseUseEvent)] = new("Melee", "ReleaseUse", arguments => new MeleeReleaseUseEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(MeleeSpawnEvent)] = new("Melee", "Spawn", arguments => new MeleeSpawnEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
        }),
        [typeof(MeleeValueChangeEvent)] = new("Melee", "ValueChange", arguments => new MeleeValueChangeEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(ParticleClassRegisterEvent)] = new("Particle", "ClassRegister", arguments => new ParticleClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(ParticleDestroyEvent)] = new("Particle", "Destroy", arguments => new ParticleDestroyEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
        }),
        [typeof(ParticleDimensionChangeEvent)] = new("Particle", "DimensionChange", arguments => new ParticleDimensionChangeEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(ParticleEnterWaterEvent)] = new("Particle", "EnterWater", arguments => new ParticleEnterWaterEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
        }),
        [typeof(ParticleLeaveWaterEvent)] = new("Particle", "LeaveWater", arguments => new ParticleLeaveWaterEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
        }),
        [typeof(ParticleNetworkAuthorityChangeEvent)] = new("Particle", "NetworkAuthorityChange", arguments => new ParticleNetworkAuthorityChangeEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(ParticleSpawnEvent)] = new("Particle", "Spawn", arguments => new ParticleSpawnEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
        }),
        [typeof(ParticleValueChangeEvent)] = new("Particle", "ValueChange", arguments => new ParticleValueChangeEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(PlayerClassRegisterEvent)] = new("Player", "ClassRegister", arguments => new PlayerClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(PlayerDestroyEvent)] = new("Player", "Destroy", arguments => new PlayerDestroyEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
        }),
        [typeof(PlayerDimensionChangeEvent)] = new("Player", "DimensionChange", arguments => new PlayerDimensionChangeEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(PlayerPossessEvent)] = new("Player", "Possess", arguments => new PlayerPossessEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            Pawn = ScriptEventArguments.Get<Entity?>(arguments, 1),
        }),
        [typeof(PlayerReadyEvent)] = new("Player", "Ready", arguments => new PlayerReadyEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
        }),
        [typeof(PlayerSpawnEvent)] = new("Player", "Spawn", arguments => new PlayerSpawnEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
        }),
        [typeof(PlayerUnPossessEvent)] = new("Player", "UnPossess", arguments => new PlayerUnPossessEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            Pawn = ScriptEventArguments.Get<Entity?>(arguments, 1),
        }),
        [typeof(PlayerVOIPGlobalChannelSettingChangeEvent)] = new("Player", "VOIPGlobalChannelSettingChange", arguments => new PlayerVOIPGlobalChannelSettingChangeEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            Channel = ScriptEventArguments.Get<int>(arguments, 1)!,
            OldSetting = ScriptEventArguments.Get<VOIPSetting>(arguments, 2)!,
            NewSetting = ScriptEventArguments.Get<VOIPSetting>(arguments, 3)!,
        }),
        [typeof(PlayerVOIPLocalSettingChangeEvent)] = new("Player", "VOIPLocalSettingChange", arguments => new PlayerVOIPLocalSettingChangeEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            OldSetting = ScriptEventArguments.Get<VOIPSetting>(arguments, 1)!,
            NewSetting = ScriptEventArguments.Get<VOIPSetting>(arguments, 2)!,
        }),
        [typeof(PlayerValueChangeEvent)] = new("Player", "ValueChange", arguments => new PlayerValueChangeEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(PropClassRegisterEvent)] = new("Prop", "ClassRegister", arguments => new PropClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(PropDestroyEvent)] = new("Prop", "Destroy", arguments => new PropDestroyEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
        }),
        [typeof(PropDimensionChangeEvent)] = new("Prop", "DimensionChange", arguments => new PropDimensionChangeEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(PropEnterWaterEvent)] = new("Prop", "EnterWater", arguments => new PropEnterWaterEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
        }),
        [typeof(PropGrabEvent)] = new("Prop", "Grab", arguments => new PropGrabEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(PropHitEvent)] = new("Prop", "Hit", arguments => new PropHitEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        }),
        [typeof(PropInteractEvent)] = new("Prop", "Interact", arguments => new PropInteractEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(PropLeaveWaterEvent)] = new("Prop", "LeaveWater", arguments => new PropLeaveWaterEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
        }),
        [typeof(PropNetworkAuthorityChangeEvent)] = new("Prop", "NetworkAuthorityChange", arguments => new PropNetworkAuthorityChangeEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(PropSpawnEvent)] = new("Prop", "Spawn", arguments => new PropSpawnEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
        }),
        [typeof(PropTakeDamageEvent)] = new("Prop", "TakeDamage", arguments => new PropTakeDamageEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        }),
        [typeof(PropUnGrabEvent)] = new("Prop", "UnGrab", arguments => new PropUnGrabEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(PropValueChangeEvent)] = new("Prop", "ValueChange", arguments => new PropValueChangeEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(ScriptConsoleLogEntryEvent)] = new("Console_I", "LogEntry", arguments => new ScriptConsoleLogEntryEvent
        {
            Text = ScriptEventArguments.Get<string?>(arguments, 0),
            Type = ScriptEventArguments.Get<LogType>(arguments, 1)!,
        }),
        [typeof(ScriptConsolePlayerSubmitEvent)] = new("Console_I", "PlayerSubmit", arguments => new ScriptConsolePlayerSubmitEvent
        {
            Text = ScriptEventArguments.Get<string?>(arguments, 0),
        }),
        [typeof(ServerChangeMapEvent)] = new("Server", "ChangeMap", arguments => new ServerChangeMapEvent
        {
            OldMap = ScriptEventArguments.Get<string?>(arguments, 0),
            NewMap = ScriptEventArguments.Get<string?>(arguments, 1),
        }),
        [typeof(ServerPackageLoadEvent)] = new("Server", "PackageLoad", arguments => new ServerPackageLoadEvent
        {
            PackageName = ScriptEventArguments.Get<string?>(arguments, 0),
        }),
        [typeof(ServerPackageUnloadEvent)] = new("Server", "PackageUnload", arguments => new ServerPackageUnloadEvent
        {
            PackageName = ScriptEventArguments.Get<string?>(arguments, 0),
        }),
        [typeof(ServerPlayerConnectEvent)] = new("Server", "PlayerConnect", arguments => new ServerPlayerConnectEvent
        {
            Ip = ScriptEventArguments.Get<string?>(arguments, 0),
            PlayerAccountId = ScriptEventArguments.Get<string?>(arguments, 1),
            PlayerName = ScriptEventArguments.Get<string?>(arguments, 2),
            PlayerSteamId = ScriptEventArguments.Get<string?>(arguments, 3),
        }),
        [typeof(ServerPlayerDisconnectEvent)] = new("Server", "PlayerDisconnect", arguments => new ServerPlayerDisconnectEvent
        {
            Ip = ScriptEventArguments.Get<string?>(arguments, 0),
            PlayerAccountId = ScriptEventArguments.Get<string?>(arguments, 1),
            PlayerName = ScriptEventArguments.Get<string?>(arguments, 2),
            PlayerSteamId = ScriptEventArguments.Get<string?>(arguments, 3),
            DisconnectReason = ScriptEventArguments.Get<string?>(arguments, 4),
        }),
        [typeof(ServerRestartEvent)] = new("Server", "Restart", arguments => new ServerRestartEvent
        {
        }),
        [typeof(ServerStartEvent)] = new("Server", "Start", arguments => new ServerStartEvent
        {
        }),
        [typeof(ServerStopEvent)] = new("Server", "Stop", arguments => new ServerStopEvent
        {
        }),
        [typeof(ServerTickEvent)] = new("Server", "Tick", arguments => new ServerTickEvent
        {
            DeltaTime = ScriptEventArguments.Get<float>(arguments, 0)!,
        }),
        [typeof(ServerValueChangeEvent)] = new("Server", "ValueChange", arguments => new ServerValueChangeEvent
        {
            Key = ScriptEventArguments.Get<string?>(arguments, 0),
            Value = ScriptEventArguments.Get<object?>(arguments, 1),
        }),
        [typeof(StaticMeshClassRegisterEvent)] = new("StaticMesh", "ClassRegister", arguments => new StaticMeshClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(StaticMeshDestroyEvent)] = new("StaticMesh", "Destroy", arguments => new StaticMeshDestroyEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
        }),
        [typeof(StaticMeshDimensionChangeEvent)] = new("StaticMesh", "DimensionChange", arguments => new StaticMeshDimensionChangeEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(StaticMeshEnterWaterEvent)] = new("StaticMesh", "EnterWater", arguments => new StaticMeshEnterWaterEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
        }),
        [typeof(StaticMeshLeaveWaterEvent)] = new("StaticMesh", "LeaveWater", arguments => new StaticMeshLeaveWaterEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
        }),
        [typeof(StaticMeshNetworkAuthorityChangeEvent)] = new("StaticMesh", "NetworkAuthorityChange", arguments => new StaticMeshNetworkAuthorityChangeEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(StaticMeshSpawnEvent)] = new("StaticMesh", "Spawn", arguments => new StaticMeshSpawnEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
        }),
        [typeof(StaticMeshValueChangeEvent)] = new("StaticMesh", "ValueChange", arguments => new StaticMeshValueChangeEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(Text3DClassRegisterEvent)] = new("Text3D", "ClassRegister", arguments => new Text3DClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(Text3DDestroyEvent)] = new("Text3D", "Destroy", arguments => new Text3DDestroyEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
        }),
        [typeof(Text3DDimensionChangeEvent)] = new("Text3D", "DimensionChange", arguments => new Text3DDimensionChangeEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(Text3DEnterWaterEvent)] = new("Text3D", "EnterWater", arguments => new Text3DEnterWaterEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
        }),
        [typeof(Text3DLeaveWaterEvent)] = new("Text3D", "LeaveWater", arguments => new Text3DLeaveWaterEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
        }),
        [typeof(Text3DNetworkAuthorityChangeEvent)] = new("Text3D", "NetworkAuthorityChange", arguments => new Text3DNetworkAuthorityChangeEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(Text3DSpawnEvent)] = new("Text3D", "Spawn", arguments => new Text3DSpawnEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
        }),
        [typeof(Text3DValueChangeEvent)] = new("Text3D", "ValueChange", arguments => new Text3DValueChangeEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(TextRenderClassRegisterEvent)] = new("TextRender", "ClassRegister", arguments => new TextRenderClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(TextRenderDestroyEvent)] = new("TextRender", "Destroy", arguments => new TextRenderDestroyEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
        }),
        [typeof(TextRenderDimensionChangeEvent)] = new("TextRender", "DimensionChange", arguments => new TextRenderDimensionChangeEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(TextRenderEnterWaterEvent)] = new("TextRender", "EnterWater", arguments => new TextRenderEnterWaterEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
        }),
        [typeof(TextRenderLeaveWaterEvent)] = new("TextRender", "LeaveWater", arguments => new TextRenderLeaveWaterEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
        }),
        [typeof(TextRenderNetworkAuthorityChangeEvent)] = new("TextRender", "NetworkAuthorityChange", arguments => new TextRenderNetworkAuthorityChangeEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(TextRenderSpawnEvent)] = new("TextRender", "Spawn", arguments => new TextRenderSpawnEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
        }),
        [typeof(TextRenderValueChangeEvent)] = new("TextRender", "ValueChange", arguments => new TextRenderValueChangeEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(TriggerBeginOverlapEvent)] = new("Trigger", "BeginOverlap", arguments => new TriggerBeginOverlapEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            Entity = ScriptEventArguments.Get<Actor?>(arguments, 1),
        }),
        [typeof(TriggerClassRegisterEvent)] = new("Trigger", "ClassRegister", arguments => new TriggerClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(TriggerDestroyEvent)] = new("Trigger", "Destroy", arguments => new TriggerDestroyEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
        }),
        [typeof(TriggerDimensionChangeEvent)] = new("Trigger", "DimensionChange", arguments => new TriggerDimensionChangeEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(TriggerEndOverlapEvent)] = new("Trigger", "EndOverlap", arguments => new TriggerEndOverlapEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            Entity = ScriptEventArguments.Get<Actor?>(arguments, 1),
        }),
        [typeof(TriggerEnterWaterEvent)] = new("Trigger", "EnterWater", arguments => new TriggerEnterWaterEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
        }),
        [typeof(TriggerLeaveWaterEvent)] = new("Trigger", "LeaveWater", arguments => new TriggerLeaveWaterEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
        }),
        [typeof(TriggerNetworkAuthorityChangeEvent)] = new("Trigger", "NetworkAuthorityChange", arguments => new TriggerNetworkAuthorityChangeEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(TriggerSpawnEvent)] = new("Trigger", "Spawn", arguments => new TriggerSpawnEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
        }),
        [typeof(TriggerValueChangeEvent)] = new("Trigger", "ValueChange", arguments => new TriggerValueChangeEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(VehicleWaterCharacterAttemptEnterEvent)] = new("VehicleWater", "CharacterAttemptEnter", arguments => new VehicleWaterCharacterAttemptEnterEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            Seat = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(VehicleWaterCharacterAttemptLeaveEvent)] = new("VehicleWater", "CharacterAttemptLeave", arguments => new VehicleWaterCharacterAttemptLeaveEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(VehicleWaterCharacterEnterEvent)] = new("VehicleWater", "CharacterEnter", arguments => new VehicleWaterCharacterEnterEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(VehicleWaterCharacterLeaveEvent)] = new("VehicleWater", "CharacterLeave", arguments => new VehicleWaterCharacterLeaveEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(VehicleWaterClassRegisterEvent)] = new("VehicleWater", "ClassRegister", arguments => new VehicleWaterClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(VehicleWaterDeathEvent)] = new("VehicleWater", "Death", arguments => new VehicleWaterDeathEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            LastDamageTaken = ScriptEventArguments.Get<int>(arguments, 1)!,
            LastBoneDamaged = ScriptEventArguments.Get<string?>(arguments, 2),
            DamageTypeReason = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            HitFromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        }),
        [typeof(VehicleWaterDestroyEvent)] = new("VehicleWater", "Destroy", arguments => new VehicleWaterDestroyEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        }),
        [typeof(VehicleWaterDimensionChangeEvent)] = new("VehicleWater", "DimensionChange", arguments => new VehicleWaterDimensionChangeEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(VehicleWaterEngineStartEvent)] = new("VehicleWater", "EngineStart", arguments => new VehicleWaterEngineStartEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        }),
        [typeof(VehicleWaterEngineStopEvent)] = new("VehicleWater", "EngineStop", arguments => new VehicleWaterEngineStopEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        }),
        [typeof(VehicleWaterEnterWaterEvent)] = new("VehicleWater", "EnterWater", arguments => new VehicleWaterEnterWaterEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        }),
        [typeof(VehicleWaterHealthChangeEvent)] = new("VehicleWater", "HealthChange", arguments => new VehicleWaterHealthChangeEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            OldHealth = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewHealth = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(VehicleWaterHitEvent)] = new("VehicleWater", "Hit", arguments => new VehicleWaterHitEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        }),
        [typeof(VehicleWaterLeaveWaterEvent)] = new("VehicleWater", "LeaveWater", arguments => new VehicleWaterLeaveWaterEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        }),
        [typeof(VehicleWaterNetworkAuthorityChangeEvent)] = new("VehicleWater", "NetworkAuthorityChange", arguments => new VehicleWaterNetworkAuthorityChangeEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(VehicleWaterRespawnEvent)] = new("VehicleWater", "Respawn", arguments => new VehicleWaterRespawnEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        }),
        [typeof(VehicleWaterSpawnEvent)] = new("VehicleWater", "Spawn", arguments => new VehicleWaterSpawnEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
        }),
        [typeof(VehicleWaterTakeDamageEvent)] = new("VehicleWater", "TakeDamage", arguments => new VehicleWaterTakeDamageEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<object?>(arguments, 6),
        }),
        [typeof(VehicleWaterValueChangeEvent)] = new("VehicleWater", "ValueChange", arguments => new VehicleWaterValueChangeEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(VehicleWheeledCharacterAttemptEnterEvent)] = new("VehicleWheeled", "CharacterAttemptEnter", arguments => new VehicleWheeledCharacterAttemptEnterEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            Seat = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(VehicleWheeledCharacterAttemptLeaveEvent)] = new("VehicleWheeled", "CharacterAttemptLeave", arguments => new VehicleWheeledCharacterAttemptLeaveEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(VehicleWheeledCharacterEnterEvent)] = new("VehicleWheeled", "CharacterEnter", arguments => new VehicleWheeledCharacterEnterEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(VehicleWheeledCharacterLeaveEvent)] = new("VehicleWheeled", "CharacterLeave", arguments => new VehicleWheeledCharacterLeaveEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(VehicleWheeledClassRegisterEvent)] = new("VehicleWheeled", "ClassRegister", arguments => new VehicleWheeledClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(VehicleWheeledDeathEvent)] = new("VehicleWheeled", "Death", arguments => new VehicleWheeledDeathEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            LastDamageTaken = ScriptEventArguments.Get<int>(arguments, 1)!,
            LastBoneDamaged = ScriptEventArguments.Get<string?>(arguments, 2),
            DamageTypeReason = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            HitFromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        }),
        [typeof(VehicleWheeledDestroyEvent)] = new("VehicleWheeled", "Destroy", arguments => new VehicleWheeledDestroyEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        }),
        [typeof(VehicleWheeledDimensionChangeEvent)] = new("VehicleWheeled", "DimensionChange", arguments => new VehicleWheeledDimensionChangeEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(VehicleWheeledEngineStartEvent)] = new("VehicleWheeled", "EngineStart", arguments => new VehicleWheeledEngineStartEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        }),
        [typeof(VehicleWheeledEngineStopEvent)] = new("VehicleWheeled", "EngineStop", arguments => new VehicleWheeledEngineStopEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        }),
        [typeof(VehicleWheeledEnterWaterEvent)] = new("VehicleWheeled", "EnterWater", arguments => new VehicleWheeledEnterWaterEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        }),
        [typeof(VehicleWheeledHealthChangeEvent)] = new("VehicleWheeled", "HealthChange", arguments => new VehicleWheeledHealthChangeEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            OldHealth = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewHealth = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(VehicleWheeledHitEvent)] = new("VehicleWheeled", "Hit", arguments => new VehicleWheeledHitEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        }),
        [typeof(VehicleWheeledHornEvent)] = new("VehicleWheeled", "Horn", arguments => new VehicleWheeledHornEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            IsHonking = ScriptEventArguments.Get<bool>(arguments, 1)!,
        }),
        [typeof(VehicleWheeledLeaveWaterEvent)] = new("VehicleWheeled", "LeaveWater", arguments => new VehicleWheeledLeaveWaterEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        }),
        [typeof(VehicleWheeledNetworkAuthorityChangeEvent)] = new("VehicleWheeled", "NetworkAuthorityChange", arguments => new VehicleWheeledNetworkAuthorityChangeEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(VehicleWheeledRespawnEvent)] = new("VehicleWheeled", "Respawn", arguments => new VehicleWheeledRespawnEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        }),
        [typeof(VehicleWheeledSpawnEvent)] = new("VehicleWheeled", "Spawn", arguments => new VehicleWheeledSpawnEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
        }),
        [typeof(VehicleWheeledTakeDamageEvent)] = new("VehicleWheeled", "TakeDamage", arguments => new VehicleWheeledTakeDamageEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<object?>(arguments, 6),
        }),
        [typeof(VehicleWheeledValueChangeEvent)] = new("VehicleWheeled", "ValueChange", arguments => new VehicleWheeledValueChangeEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
        [typeof(WeaponAmmoBagChangeEvent)] = new("Weapon", "AmmoBagChange", arguments => new WeaponAmmoBagChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            OldAmmoBag = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewAmmoBag = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(WeaponAmmoClipChangeEvent)] = new("Weapon", "AmmoClipChange", arguments => new WeaponAmmoClipChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            OldAmmoClip = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewAmmoClip = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(WeaponClassRegisterEvent)] = new("Weapon", "ClassRegister", arguments => new WeaponClassRegisterEvent
        {
            Class = ScriptEventArguments.Get<object?>(arguments, 0),
        }),
        [typeof(WeaponDestroyEvent)] = new("Weapon", "Destroy", arguments => new WeaponDestroyEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
        }),
        [typeof(WeaponDimensionChangeEvent)] = new("Weapon", "DimensionChange", arguments => new WeaponDimensionChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(WeaponDropEvent)] = new("Weapon", "Drop", arguments => new WeaponDropEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            WasTriggeredByPlayer = ScriptEventArguments.Get<bool>(arguments, 2)!,
        }),
        [typeof(WeaponEnterWaterEvent)] = new("Weapon", "EnterWater", arguments => new WeaponEnterWaterEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
        }),
        [typeof(WeaponHitEvent)] = new("Weapon", "Hit", arguments => new WeaponHitEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            ImpactForce = ScriptEventArguments.Get<float>(arguments, 1)!,
            NormalImpulse = ScriptEventArguments.Get<Vector>(arguments, 2)!,
            ImpactLocation = ScriptEventArguments.Get<Vector>(arguments, 3)!,
            Velocity = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            OtherActor = ScriptEventArguments.Get<Actor?>(arguments, 5),
        }),
        [typeof(WeaponInteractEvent)] = new("Weapon", "Interact", arguments => new WeaponInteractEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(WeaponLeaveWaterEvent)] = new("Weapon", "LeaveWater", arguments => new WeaponLeaveWaterEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
        }),
        [typeof(WeaponNetworkAuthorityChangeEvent)] = new("Weapon", "NetworkAuthorityChange", arguments => new WeaponNetworkAuthorityChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        }),
        [typeof(WeaponPickUpEvent)] = new("Weapon", "PickUp", arguments => new WeaponPickUpEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(WeaponPullUseEvent)] = new("Weapon", "PullUse", arguments => new WeaponPullUseEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(WeaponReleaseUseEvent)] = new("Weapon", "ReleaseUse", arguments => new WeaponReleaseUseEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        }),
        [typeof(WeaponReloadEvent)] = new("Weapon", "Reload", arguments => new WeaponReloadEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            AmmoToReload = ScriptEventArguments.Get<int>(arguments, 2)!,
        }),
        [typeof(WeaponSpawnEvent)] = new("Weapon", "Spawn", arguments => new WeaponSpawnEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
        }),
        [typeof(WeaponValueChangeEvent)] = new("Weapon", "ValueChange", arguments => new WeaponValueChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Key = ScriptEventArguments.Get<string?>(arguments, 1),
            Value = ScriptEventArguments.Get<object?>(arguments, 2),
        }),
    };
}
