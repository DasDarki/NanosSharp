// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Melee : Pickable
{
    public static void AddAnimationCharacterUse(ILuaVM vm, LuaRef selfRef, string asset_path, LuaRef play_rate, AnimationSlotType slot_Type)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "AddAnimationCharacterUse");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(asset_path);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, play_rate);
        pc++;
        vm.PushEnum(slot_Type);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void ClearAnimationsCharacterUse(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "ClearAnimationsCharacterUse");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetImpactSound(ILuaVM vm, LuaRef selfRef, SurfaceType surface_type, string asset_path, LuaRef volume, LuaRef pitch)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetImpactSound");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushEnum(surface_type);
        pc++;
        vm.PushString(asset_path);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, volume);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, pitch);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSoundUse(ILuaVM vm, LuaRef selfRef, string asset_path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSoundUse");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(asset_path);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetBaseDamage(ILuaVM vm, LuaRef selfRef, LuaRef? damage = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetBaseDamage");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        if (damage != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, damage.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetDamageSettings(ILuaVM vm, LuaRef selfRef, LuaRef damage_start_time, LuaRef damage_duration_time)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetDamageSettings");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, damage_start_time);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, damage_duration_time);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetCooldown(ILuaVM vm, LuaRef selfRef, LuaRef cooldown)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetCooldown");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, cooldown);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static string[] GetAnimationsCharacterUse(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAnimationsCharacterUse");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToArray<string>(-1);
        vm.ClearStack();
        return r0;
    }

    public static string GetSoundUse(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSoundUse");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetBaseDamage(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetBaseDamage");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetCooldown(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Melee");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetCooldown");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

}

