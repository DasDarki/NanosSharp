// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Player
{
    public static void Ban(ILuaVM vm, int selfRef, string reason)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Ban");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(reason);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Connect(ILuaVM vm, int selfRef, string IP, string? password = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Connect");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(IP);
        if (password != null)
        {
             pc++;
             vm.PushString(password);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Kick(ILuaVM vm, int selfRef, string reason)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Kick");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(reason);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Possess(ILuaVM vm, int selfRef, LuaRef new_character, double? blend_time = null, double? exp = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Possess");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, new_character);
        if (blend_time != null)
        {
             pc++;
             vm.PushNumber(blend_time.Value);
        }
        if (exp != null)
        {
             pc++;
             vm.PushNumber(exp.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetCameraLocation(ILuaVM vm, int selfRef, LuaRef location)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetCameraLocation");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, location);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetCameraRotation(ILuaVM vm, int selfRef, LuaRef rotation)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetCameraRotation");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, rotation);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void TranslateCameraTo(ILuaVM vm, int selfRef, LuaRef location, double time, double? exp = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "TranslateCameraTo");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, location);
        pc++;
        vm.PushNumber(time);
        if (exp != null)
        {
             pc++;
             vm.PushNumber(exp.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void RotateCameraTo(ILuaVM vm, int selfRef, LuaRef rotation, double time, double? exp = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "RotateCameraTo");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, rotation);
        pc++;
        vm.PushNumber(time);
        if (exp != null)
        {
             pc++;
             vm.PushNumber(exp.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetCameraSocketOffset(ILuaVM vm, int selfRef, LuaRef socket_offset)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetCameraSocketOffset");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, socket_offset);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetCameraArmLength(ILuaVM vm, int selfRef, double length)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetCameraArmLength");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(length);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void AttachCameraTo(ILuaVM vm, int selfRef, LuaRef actor, LuaRef socket_offset, double blend_speed)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "AttachCameraTo");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, actor);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, socket_offset);
        pc++;
        vm.PushNumber(blend_speed);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void ResetCamera(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "ResetCamera");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Spectate(ILuaVM vm, int selfRef, LuaRef player, double? blend_speed = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Spectate");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, player);
        if (blend_speed != null)
        {
             pc++;
             vm.PushNumber(blend_speed.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetName(ILuaVM vm, int selfRef, string new_name)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetName");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(new_name);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetValue(ILuaVM vm, int selfRef, string key, object value, bool? sync_on_clients = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetValue");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(key);
        pc++;
        vm.PushObject(value);
        if (sync_on_clients != null)
        {
             pc++;
             vm.PushBoolean(sync_on_clients.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetVOIPChannel(ILuaVM vm, int selfRef, double channel)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetVOIPChannel");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(channel);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetVOIPSetting(ILuaVM vm, int selfRef, LuaRef setting)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetVOIPSetting");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, setting);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetVOIPVolume(ILuaVM vm, int selfRef, double volume)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetVOIPVolume");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(volume);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void UnPossess(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "UnPossess");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static string GetSteamID(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSteamID");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetAccountID(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAccountID");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetAccountName(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAccountName");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static LuaRef? GetControlledCharacter(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetControlledCharacter");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static double GetID(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetID");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetIP(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetIP");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetName(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetName");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetPing(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetPing");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetType(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetType");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetVOIPChannel(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetVOIPChannel");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static object GetValue(ILuaVM vm, int selfRef, string key, object fallback)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetValue");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(key);
        pc++;
        vm.PushObject(fallback);
        vm.MCall(pc, 1);
        var r0 = vm.ToObject(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool IsValid(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "IsValid");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetVOIPSetting(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetVOIPSetting");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static ILuaVM.CFunction Subscribe(ILuaVM vm, int selfRef, string event_name, ILuaVM.CFunction function)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Subscribe");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(event_name);
        pc++;
        vm.PushManagedFunction(function);
        vm.MCall(pc, 1);
        var r0 = vm.ToCFunction(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static void Unsubscribe(ILuaVM vm, int selfRef, string event_name, ILuaVM.CFunction function)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Player");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Unsubscribe");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(event_name);
        pc++;
        vm.PushManagedFunction(function);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

}

