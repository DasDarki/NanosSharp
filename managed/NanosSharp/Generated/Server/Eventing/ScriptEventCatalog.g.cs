#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

internal static partial class ScriptEventCatalog
{
    static partial void AddSideDescriptors(Dictionary<Type, ScriptEventDescriptor> descriptors)
    {
        descriptors[typeof(BlueprintDimensionChangeEvent)] = new("Blueprint", "DimensionChange", arguments => new BlueprintDimensionChangeEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(BlueprintNetworkAuthorityChangeEvent)] = new("Blueprint", "NetworkAuthorityChange", arguments => new BlueprintNetworkAuthorityChangeEvent
        {
            Blueprint = ScriptEventArguments.Get<Blueprint>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(CableDimensionChangeEvent)] = new("Cable", "DimensionChange", arguments => new CableDimensionChangeEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CableNetworkAuthorityChangeEvent)] = new("Cable", "NetworkAuthorityChange", arguments => new CableNetworkAuthorityChangeEvent
        {
            Cable = ScriptEventArguments.Get<Cable>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(CharacterAttemptEnterVehicleEvent)] = new("Character", "AttemptEnterVehicle", arguments => new CharacterAttemptEnterVehicleEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterAttemptLeaveVehicleEvent)] = new("Character", "AttemptLeaveVehicle", arguments => new CharacterAttemptLeaveVehicleEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
        });
        descriptors[typeof(CharacterDimensionChangeEvent)] = new("Character", "DimensionChange", arguments => new CharacterDimensionChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterInteractEvent)] = new("Character", "Interact", arguments => new CharacterInteractEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            Object = ScriptEventArguments.Get<Prop?>(arguments, 1),
        });
        descriptors[typeof(CharacterNetworkAuthorityChangeEvent)] = new("Character", "NetworkAuthorityChange", arguments => new CharacterNetworkAuthorityChangeEvent
        {
            Character = ScriptEventArguments.Get<Character>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(CharacterGASPAttemptEnterVehicleEvent)] = new("CharacterGASP", "AttemptEnterVehicle", arguments => new CharacterGASPAttemptEnterVehicleEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
            SeatIndex = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterGASPAttemptLeaveVehicleEvent)] = new("CharacterGASP", "AttemptLeaveVehicle", arguments => new CharacterGASPAttemptLeaveVehicleEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Vehicle = ScriptEventArguments.Get<Entity?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPDimensionChangeEvent)] = new("CharacterGASP", "DimensionChange", arguments => new CharacterGASPDimensionChangeEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterGASPInteractEvent)] = new("CharacterGASP", "Interact", arguments => new CharacterGASPInteractEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            Object = ScriptEventArguments.Get<Prop?>(arguments, 1),
        });
        descriptors[typeof(CharacterGASPNetworkAuthorityChangeEvent)] = new("CharacterGASP", "NetworkAuthorityChange", arguments => new CharacterGASPNetworkAuthorityChangeEvent
        {
            CharacterGASP = ScriptEventArguments.Get<CharacterGASP>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(CharacterSimpleDimensionChangeEvent)] = new("CharacterSimple", "DimensionChange", arguments => new CharacterSimpleDimensionChangeEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(CharacterSimpleNetworkAuthorityChangeEvent)] = new("CharacterSimple", "NetworkAuthorityChange", arguments => new CharacterSimpleNetworkAuthorityChangeEvent
        {
            CharacterSimple = ScriptEventArguments.Get<CharacterSimple>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(GrenadeDimensionChangeEvent)] = new("Grenade", "DimensionChange", arguments => new GrenadeDimensionChangeEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(GrenadeInteractEvent)] = new("Grenade", "Interact", arguments => new GrenadeInteractEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(GrenadeNetworkAuthorityChangeEvent)] = new("Grenade", "NetworkAuthorityChange", arguments => new GrenadeNetworkAuthorityChangeEvent
        {
            Grenade = ScriptEventArguments.Get<Grenade>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(InstancedStaticMeshDimensionChangeEvent)] = new("InstancedStaticMesh", "DimensionChange", arguments => new InstancedStaticMeshDimensionChangeEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(InstancedStaticMeshNetworkAuthorityChangeEvent)] = new("InstancedStaticMesh", "NetworkAuthorityChange", arguments => new InstancedStaticMeshNetworkAuthorityChangeEvent
        {
            InstancedStaticMesh = ScriptEventArguments.Get<InstancedStaticMesh>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(LightDimensionChangeEvent)] = new("Light", "DimensionChange", arguments => new LightDimensionChangeEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(LightNetworkAuthorityChangeEvent)] = new("Light", "NetworkAuthorityChange", arguments => new LightNetworkAuthorityChangeEvent
        {
            Light = ScriptEventArguments.Get<Light>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(MeleeDimensionChangeEvent)] = new("Melee", "DimensionChange", arguments => new MeleeDimensionChangeEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(MeleeInteractEvent)] = new("Melee", "Interact", arguments => new MeleeInteractEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(MeleeNetworkAuthorityChangeEvent)] = new("Melee", "NetworkAuthorityChange", arguments => new MeleeNetworkAuthorityChangeEvent
        {
            Melee = ScriptEventArguments.Get<Melee>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(ParticleDimensionChangeEvent)] = new("Particle", "DimensionChange", arguments => new ParticleDimensionChangeEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(ParticleNetworkAuthorityChangeEvent)] = new("Particle", "NetworkAuthorityChange", arguments => new ParticleNetworkAuthorityChangeEvent
        {
            Particle = ScriptEventArguments.Get<Particle>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(PlayerDimensionChangeEvent)] = new("Player", "DimensionChange", arguments => new PlayerDimensionChangeEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(PlayerReadyEvent)] = new("Player", "Ready", arguments => new PlayerReadyEvent
        {
            Player = ScriptEventArguments.Get<Player>(arguments, 0)!,
        });
        descriptors[typeof(PropDimensionChangeEvent)] = new("Prop", "DimensionChange", arguments => new PropDimensionChangeEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(PropInteractEvent)] = new("Prop", "Interact", arguments => new PropInteractEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(PropNetworkAuthorityChangeEvent)] = new("Prop", "NetworkAuthorityChange", arguments => new PropNetworkAuthorityChangeEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(PropTakeDamageEvent)] = new("Prop", "TakeDamage", arguments => new PropTakeDamageEvent
        {
            Prop = ScriptEventArguments.Get<Prop>(arguments, 0)!,
            Damage = ScriptEventArguments.Get<int>(arguments, 1)!,
            Bone = ScriptEventArguments.Get<string?>(arguments, 2),
            Type = ScriptEventArguments.Get<DamageType>(arguments, 3)!,
            FromDirection = ScriptEventArguments.Get<Vector>(arguments, 4)!,
            Instigator = ScriptEventArguments.Get<Player?>(arguments, 5),
            Causer = ScriptEventArguments.Get<Actor?>(arguments, 6),
        });
        descriptors[typeof(ServerChangeMapEvent)] = new("Server", "ChangeMap", arguments => new ServerChangeMapEvent
        {
            OldMap = ScriptEventArguments.Get<string?>(arguments, 0),
            NewMap = ScriptEventArguments.Get<string?>(arguments, 1),
        });
        descriptors[typeof(ServerPackageLoadEvent)] = new("Server", "PackageLoad", arguments => new ServerPackageLoadEvent
        {
            PackageName = ScriptEventArguments.Get<string?>(arguments, 0),
        });
        descriptors[typeof(ServerPackageUnloadEvent)] = new("Server", "PackageUnload", arguments => new ServerPackageUnloadEvent
        {
            PackageName = ScriptEventArguments.Get<string?>(arguments, 0),
        });
        descriptors[typeof(ServerPlayerConnectEvent)] = new("Server", "PlayerConnect", arguments => new ServerPlayerConnectEvent
        {
            Ip = ScriptEventArguments.Get<string?>(arguments, 0),
            PlayerAccountId = ScriptEventArguments.Get<string?>(arguments, 1),
            PlayerName = ScriptEventArguments.Get<string?>(arguments, 2),
            PlayerSteamId = ScriptEventArguments.Get<string?>(arguments, 3),
        });
        descriptors[typeof(ServerPlayerDisconnectEvent)] = new("Server", "PlayerDisconnect", arguments => new ServerPlayerDisconnectEvent
        {
            Ip = ScriptEventArguments.Get<string?>(arguments, 0),
            PlayerAccountId = ScriptEventArguments.Get<string?>(arguments, 1),
            PlayerName = ScriptEventArguments.Get<string?>(arguments, 2),
            PlayerSteamId = ScriptEventArguments.Get<string?>(arguments, 3),
            DisconnectReason = ScriptEventArguments.Get<string?>(arguments, 4),
        });
        descriptors[typeof(ServerRestartEvent)] = new("Server", "Restart", arguments => new ServerRestartEvent
        {
        });
        descriptors[typeof(ServerStartEvent)] = new("Server", "Start", arguments => new ServerStartEvent
        {
        });
        descriptors[typeof(ServerStopEvent)] = new("Server", "Stop", arguments => new ServerStopEvent
        {
        });
        descriptors[typeof(ServerTickEvent)] = new("Server", "Tick", arguments => new ServerTickEvent
        {
            DeltaTime = ScriptEventArguments.Get<float>(arguments, 0)!,
        });
        descriptors[typeof(ServerValueChangeEvent)] = new("Server", "ValueChange", arguments => new ServerValueChangeEvent
        {
            Key = ScriptEventArguments.Get<string?>(arguments, 0),
            Value = ScriptEventArguments.Get<object?>(arguments, 1),
        });
        descriptors[typeof(StaticMeshDimensionChangeEvent)] = new("StaticMesh", "DimensionChange", arguments => new StaticMeshDimensionChangeEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(StaticMeshNetworkAuthorityChangeEvent)] = new("StaticMesh", "NetworkAuthorityChange", arguments => new StaticMeshNetworkAuthorityChangeEvent
        {
            StaticMesh = ScriptEventArguments.Get<StaticMesh>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(TebexOfflineCommandEvent)] = new("Tebex", "OfflineCommand", arguments => new TebexOfflineCommandEvent
        {
            CommandId = ScriptEventArguments.Get<long>(arguments, 0)!,
            PaymentId = ScriptEventArguments.Get<long>(arguments, 1)!,
            PackageId = ScriptEventArguments.Get<long>(arguments, 2)!,
            Command = ScriptEventArguments.Get<string?>(arguments, 3),
            PlayerId = ScriptEventArguments.Get<string?>(arguments, 4),
            PlayerName = ScriptEventArguments.Get<string?>(arguments, 5),
            PlayerSteamId = ScriptEventArguments.Get<string?>(arguments, 6),
        });
        descriptors[typeof(TebexOnlineCommandEvent)] = new("Tebex", "OnlineCommand", arguments => new TebexOnlineCommandEvent
        {
            CommandId = ScriptEventArguments.Get<long>(arguments, 0)!,
            PaymentId = ScriptEventArguments.Get<long>(arguments, 1)!,
            PackageId = ScriptEventArguments.Get<long>(arguments, 2)!,
            Command = ScriptEventArguments.Get<string?>(arguments, 3),
            PlayerId = ScriptEventArguments.Get<string?>(arguments, 4),
            PlayerName = ScriptEventArguments.Get<string?>(arguments, 5),
            PlayerSteamId = ScriptEventArguments.Get<string?>(arguments, 6),
        });
        descriptors[typeof(Text3DDimensionChangeEvent)] = new("Text3D", "DimensionChange", arguments => new Text3DDimensionChangeEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(Text3DNetworkAuthorityChangeEvent)] = new("Text3D", "NetworkAuthorityChange", arguments => new Text3DNetworkAuthorityChangeEvent
        {
            Text3D = ScriptEventArguments.Get<Text3D>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(TextRenderDimensionChangeEvent)] = new("TextRender", "DimensionChange", arguments => new TextRenderDimensionChangeEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(TextRenderNetworkAuthorityChangeEvent)] = new("TextRender", "NetworkAuthorityChange", arguments => new TextRenderNetworkAuthorityChangeEvent
        {
            TextRender = ScriptEventArguments.Get<TextRender>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(TriggerDimensionChangeEvent)] = new("Trigger", "DimensionChange", arguments => new TriggerDimensionChangeEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(TriggerNetworkAuthorityChangeEvent)] = new("Trigger", "NetworkAuthorityChange", arguments => new TriggerNetworkAuthorityChangeEvent
        {
            Trigger = ScriptEventArguments.Get<Trigger>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(VehicleWaterCharacterAttemptEnterEvent)] = new("VehicleWater", "CharacterAttemptEnter", arguments => new VehicleWaterCharacterAttemptEnterEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            Seat = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(VehicleWaterCharacterAttemptLeaveEvent)] = new("VehicleWater", "CharacterAttemptLeave", arguments => new VehicleWaterCharacterAttemptLeaveEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(VehicleWaterDimensionChangeEvent)] = new("VehicleWater", "DimensionChange", arguments => new VehicleWaterDimensionChangeEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(VehicleWaterNetworkAuthorityChangeEvent)] = new("VehicleWater", "NetworkAuthorityChange", arguments => new VehicleWaterNetworkAuthorityChangeEvent
        {
            VehicleWater = ScriptEventArguments.Get<VehicleWater>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(VehicleWheeledCharacterAttemptEnterEvent)] = new("VehicleWheeled", "CharacterAttemptEnter", arguments => new VehicleWheeledCharacterAttemptEnterEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
            Seat = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(VehicleWheeledCharacterAttemptLeaveEvent)] = new("VehicleWheeled", "CharacterAttemptLeave", arguments => new VehicleWheeledCharacterAttemptLeaveEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(VehicleWheeledDimensionChangeEvent)] = new("VehicleWheeled", "DimensionChange", arguments => new VehicleWheeledDimensionChangeEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(VehicleWheeledNetworkAuthorityChangeEvent)] = new("VehicleWheeled", "NetworkAuthorityChange", arguments => new VehicleWheeledNetworkAuthorityChangeEvent
        {
            VehicleWheeled = ScriptEventArguments.Get<VehicleWheeled>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(WeaponDimensionChangeEvent)] = new("Weapon", "DimensionChange", arguments => new WeaponDimensionChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            OldDimension = ScriptEventArguments.Get<int>(arguments, 1)!,
            NewDimension = ScriptEventArguments.Get<int>(arguments, 2)!,
        });
        descriptors[typeof(WeaponInteractEvent)] = new("Weapon", "Interact", arguments => new WeaponInteractEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            Character = ScriptEventArguments.Get<Character?>(arguments, 1),
        });
        descriptors[typeof(WeaponNetworkAuthorityChangeEvent)] = new("Weapon", "NetworkAuthorityChange", arguments => new WeaponNetworkAuthorityChangeEvent
        {
            Weapon = ScriptEventArguments.Get<Weapon>(arguments, 0)!,
            OldNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 1),
            NewNetworkAuthority = ScriptEventArguments.Get<Player?>(arguments, 2),
        });
        descriptors[typeof(WebSocketCloseEvent)] = new("WebSocket", "Close", arguments => new WebSocketCloseEvent
        {
            WebSocket = ScriptEventArguments.Get<WebSocket>(arguments, 0)!,
        });
        descriptors[typeof(WebSocketReceiveMessageEvent)] = new("WebSocket", "ReceiveMessage", arguments => new WebSocketReceiveMessageEvent
        {
            WebSocket = ScriptEventArguments.Get<WebSocket>(arguments, 0)!,
            Message = ScriptEventArguments.Get<string?>(arguments, 1),
        });
    }
}
