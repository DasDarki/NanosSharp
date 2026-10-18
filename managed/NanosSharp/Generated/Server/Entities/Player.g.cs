#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Player
{
    /// <summary>Triggered when a Player changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
    }

    /// <summary>Triggered when Player is ready (the client fully joined, loaded the map and all entities and is ready to play)</summary>
    public static event ReadyHandler? OnReady
    {
        add => ReadyEvent.Add(value);
        remove => ReadyEvent.Remove(value);
    }

    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Player", "DimensionChange");
    private static readonly ClassEvent<ReadyHandler> ReadyEvent = new("Player", "Ready");
    private static readonly MethodSlot BanSlot = new("Ban");
    private static readonly MethodSlot ConnectSlot = new("Connect");
    private static readonly MethodSlot GetIPSlot = new("GetIP");
    private static readonly MethodSlot IsHostSlot = new("IsHost");
    private static readonly MethodSlot KickSlot = new("Kick");
    private static readonly MethodSlot PossessSlot = new("Possess");
    private static readonly MethodSlot SetDimensionSlot = new("SetDimension");
    private static readonly MethodSlot SetDistanceOptimizationMultiplierSlot = new("SetDistanceOptimizationMultiplier");
    private static readonly MethodSlot SetNameSlot = new("SetName");
    private static readonly MethodSlot SetVOIPChannelSlot = new("SetVOIPChannel");
    private static readonly MethodSlot SetVOIPGlobalAllChannelsSettingSlot = new("SetVOIPGlobalAllChannelsSetting");
    private static readonly MethodSlot SetVOIPGlobalChannelSettingSlot = new("SetVOIPGlobalChannelSetting");
    private static readonly MethodSlot SetVOIPLocalSettingSlot = new("SetVOIPLocalSetting");
    private static readonly MethodSlot UnPossessSlot = new("UnPossess");

    /// <summary>Gets this Player's dimension</summary>
    public uint Dimension
    {
        get => GetDimension();
        set => SetDimension(value);
    }

    /// <summary>Gets the Player's name</summary>
    public string Name
    {
        get => GetName();
        set => SetName(value);
    }

    /// <summary>Gets the voice channel of the Player</summary>
    public uint VOIPChannel
    {
        get => GetVOIPChannel();
        set => SetVOIPChannel(value);
    }

    /// <summary>Gets the Player's IP address</summary>
    public string IP => GetIP();

    /// <summary>Gets if this Player is the host of a P2P (listen) server</summary>
    public bool Host => IsHost();

    /// <summary>Bans the player from the server</summary>
    public void Ban(string reason) => Call(BanSlot, 1, reason);

    /// <summary>Redirects the player to another server</summary>
    public void Connect(string ip, string? password = null) => Call(ConnectSlot, 1, ip, password);

    /// <summary>Gets the Player's IP address</summary>
    public string GetIP() => CallResult<string>(GetIPSlot, 0)!;

    /// <summary>Gets if this Player is the host of a P2P (listen) server</summary>
    public bool IsHost() => CallResult<bool>(IsHostSlot, 0)!;

    /// <summary>Kicks the player from the server</summary>
    public void Kick(string reason) => Call(KickSlot, 1, reason);

    /// <summary>Makes this Player possess and control a Pawn (Character)</summary>
    public void Possess(Entity newPawn, float? blendTime = null, float? exp = null) =>
        Call(PossessSlot, 1, newPawn, blendTime, exp);

    /// <summary>Sets this Player's dimension</summary>
    public void SetDimension(uint dimension) => Call(SetDimensionSlot, 1, dimension);

    /// <summary>Sets the multiplier for how much this Player's is affected by distance optimization from other actors</summary>
    public void SetDistanceOptimizationMultiplier(float multiplier) =>
        Call(SetDistanceOptimizationMultiplierSlot, 1, multiplier);

    /// <summary>Sets the player's name</summary>
    public void SetName(string playerName) => Call(SetNameSlot, 1, playerName);

    /// <summary>Sets the voice channel of the Player</summary>
    public void SetVOIPChannel(uint channel) => Call(SetVOIPChannelSlot, 1, channel);

    /// <summary>Sets the VOIP setting for all global channels</summary>
    public void SetVOIPGlobalAllChannelsSetting(VOIPSetting setting) =>
        Call(SetVOIPGlobalAllChannelsSettingSlot, 1, setting);

    /// <summary>Sets the VOIP setting for a specific global channel</summary>
    public void SetVOIPGlobalChannelSetting(byte channel, VOIPSetting setting) =>
        Call(SetVOIPGlobalChannelSettingSlot, 2, channel, setting);

    /// <summary>Sets the VOIP setting for the local/proximity</summary>
    public void SetVOIPLocalSetting(VOIPSetting setting) => Call(SetVOIPLocalSettingSlot, 1, setting);

    /// <summary>Release the Player from the Character</summary>
    public void UnPossess() => Call(UnPossessSlot, 0);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the Ready event of this entity only.</summary>
    public ScriptSubscription SubscribeReady(ReadyHandler handler) => Subscribe("Ready", handler);

    /// <summary>Called for the DimensionChange event of this entity when a subclass overrides it.</summary>
    [EventHook("DimensionChange")]
    protected virtual void HandleDimensionChange(int oldDimension, int newDimension)
    {
    }

    /// <summary>Called for the Ready event of this entity when a subclass overrides it.</summary>
    [EventHook("Ready")]
    protected virtual void HandleReady()
    {
    }

    /// <summary>Triggered when a Player changes its dimension</summary>
    public delegate void DimensionChangeHandler(Player self, int oldDimension, int newDimension);

    /// <summary>Triggered when Player is ready (the client fully joined, loaded the map and all entities and is ready to play)</summary>
    public delegate void ReadyHandler(Player self);
}
