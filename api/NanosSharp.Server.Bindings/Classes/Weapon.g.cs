// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Weapon : Pickable
{
    public static void Reload(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Reload");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetAmmoBag(ILuaVM vm, int selfRef, double new_ammo)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetAmmoBag");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(new_ammo);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetAmmoClip(ILuaVM vm, int selfRef, double new_ammo)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetAmmoClip");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(new_ammo);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetAmmoSettings(ILuaVM vm, int selfRef, double ammo_clip, double ammo_bag, double? ammo_to_reload = null, double? clip_capacity = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetAmmoSettings");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(ammo_clip);
        pc++;
        vm.PushNumber(ammo_bag);
        if (ammo_to_reload != null)
        {
             pc++;
             vm.PushNumber(ammo_to_reload.Value);
        }
        if (clip_capacity != null)
        {
             pc++;
             vm.PushNumber(clip_capacity.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetAnimationFire(ILuaVM vm, int selfRef, string animation_asset_path, double? play_rate = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetAnimationFire");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(animation_asset_path);
        if (play_rate != null)
        {
             pc++;
             vm.PushNumber(play_rate.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetAnimationCharacterFire(ILuaVM vm, int selfRef, string animation_path, double? play_rate = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetAnimationCharacterFire");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(animation_path);
        if (play_rate != null)
        {
             pc++;
             vm.PushNumber(play_rate.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetAutoReload(ILuaVM vm, int selfRef, bool auto_reload)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetAutoReload");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushBoolean(auto_reload);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetBulletColor(ILuaVM vm, int selfRef, LuaRef color)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetBulletColor");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, color);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetBulletSettings(ILuaVM vm, int selfRef, double bullet_count, double bullet_max_distance, double bullet_velocity, LuaRef bullet_color)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetBulletSettings");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(bullet_count);
        pc++;
        vm.PushNumber(bullet_max_distance);
        pc++;
        vm.PushNumber(bullet_velocity);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, bullet_color);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetCadence(ILuaVM vm, int selfRef, double cadence)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetCadence");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(cadence);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetClipCapacity(ILuaVM vm, int selfRef, double clip)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetClipCapacity");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(clip);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetDamage(ILuaVM vm, int selfRef, double damage)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetDamage");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(damage);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetHandlingMode(ILuaVM vm, int selfRef, LuaRef mode)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetHandlingMode");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, mode);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetLeftHandTransform(ILuaVM vm, int selfRef, LuaRef location, LuaRef rotation)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetLeftHandTransform");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, location);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, rotation);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetMagazineMesh(ILuaVM vm, int selfRef, string static_mesh_asset_path, string? magazine_mesh_hide_bone = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetMagazineMesh");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(static_mesh_asset_path);
        if (magazine_mesh_hide_bone != null)
        {
             pc++;
             vm.PushString(magazine_mesh_hide_bone);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetParticlesBulletTrail(ILuaVM vm, int selfRef, string particle_asset_path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetParticlesBulletTrail");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(particle_asset_path);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetParticlesBarrel(ILuaVM vm, int selfRef, string particle_asset_path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetParticlesBarrel");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(particle_asset_path);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetParticlesShells(ILuaVM vm, int selfRef, string particle_asset_path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetParticlesShells");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(particle_asset_path);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetRightHandOffset(ILuaVM vm, int selfRef, LuaRef offset)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetRightHandOffset");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, offset);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSightFOVMultiplier(ILuaVM vm, int selfRef, double multiplier)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSightFOVMultiplier");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(multiplier);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSightTransform(ILuaVM vm, int selfRef, LuaRef location, LuaRef rotation)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSightTransform");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, location);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, rotation);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSoundDry(ILuaVM vm, int selfRef, string sound_asset_path, double? volume = null, double? pitch = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSoundDry");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(sound_asset_path);
        if (volume != null)
        {
             pc++;
             vm.PushNumber(volume.Value);
        }
        if (pitch != null)
        {
             pc++;
             vm.PushNumber(pitch.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSoundLoad(ILuaVM vm, int selfRef, string sound_asset_path, double? volume = null, double? pitch = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSoundLoad");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(sound_asset_path);
        if (volume != null)
        {
             pc++;
             vm.PushNumber(volume.Value);
        }
        if (pitch != null)
        {
             pc++;
             vm.PushNumber(pitch.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSoundUnload(ILuaVM vm, int selfRef, string sound_asset_path, double? volume = null, double? pitch = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSoundUnload");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(sound_asset_path);
        if (volume != null)
        {
             pc++;
             vm.PushNumber(volume.Value);
        }
        if (pitch != null)
        {
             pc++;
             vm.PushNumber(pitch.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSoundZooming(ILuaVM vm, int selfRef, string sound_asset_path, double? volume = null, double? pitch = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSoundZooming");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(sound_asset_path);
        if (volume != null)
        {
             pc++;
             vm.PushNumber(volume.Value);
        }
        if (pitch != null)
        {
             pc++;
             vm.PushNumber(pitch.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSoundFire(ILuaVM vm, int selfRef, string sound_asset_path, double? volume = null, double? pitch = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSoundFire");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(sound_asset_path);
        if (volume != null)
        {
             pc++;
             vm.PushNumber(volume.Value);
        }
        if (pitch != null)
        {
             pc++;
             vm.PushNumber(pitch.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSoundAim(ILuaVM vm, int selfRef, string sound_asset_path, double? volume = null, double? pitch = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSoundAim");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(sound_asset_path);
        if (volume != null)
        {
             pc++;
             vm.PushNumber(volume.Value);
        }
        if (pitch != null)
        {
             pc++;
             vm.PushNumber(pitch.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSoundFireLastBullets(ILuaVM vm, int selfRef, string sound_asset_path, double? remaining_bullets_count = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSoundFireLastBullets");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(sound_asset_path);
        if (remaining_bullets_count != null)
        {
             pc++;
             vm.PushNumber(remaining_bullets_count.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetSpread(ILuaVM vm, int selfRef, double spread)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetSpread");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(spread);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetRecoil(ILuaVM vm, int selfRef, double recoil)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetRecoil");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(recoil);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetUsageSettings(ILuaVM vm, int selfRef, bool can_hold_use, bool hold_release_use)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetUsageSettings");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushBoolean(can_hold_use);
        pc++;
        vm.PushBoolean(hold_release_use);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetWallbangSettings(ILuaVM vm, int selfRef, double max_distance, double damage_multiplier)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetWallbangSettings");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(max_distance);
        pc++;
        vm.PushNumber(damage_multiplier);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static double GetAmmoBag(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAmmoBag");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetAmmoClip(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAmmoClip");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetAmmoToReload(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAmmoToReload");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetHandlingMode(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetHandlingMode");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static string GetAnimationCharacterFire(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAnimationCharacterFire");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetAnimationFire(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAnimationFire");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetMagazineMesh(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetMagazineMesh");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetParticlesBulletTrail(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetParticlesBulletTrail");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetParticlesShells(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetParticlesShells");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetSoundDry(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSoundDry");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetSoundLoad(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSoundLoad");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetSoundUnload(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSoundUnload");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetSoundZooming(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSoundZooming");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetSoundAim(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSoundAim");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string GetSoundFire(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSoundFire");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool GetCanHoldUse(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetCanHoldUse");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool GetHoldReleaseUse(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetHoldReleaseUse");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetBulletCount(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetBulletCount");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetBulletColor(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetBulletColor");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static double GetCadence(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetCadence");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetClipCapacity(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetClipCapacity");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetDamage(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetDamage");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetRightHandOffset(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetRightHandOffset");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetLeftHandLocation(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetLeftHandLocation");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetLeftHandRotation(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetLeftHandRotation");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetSightLocation(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSightLocation");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetSightRotation(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSightRotation");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static double GetSightFOVMultiplier(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSightFOVMultiplier");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetSpread(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetSpread");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetRecoil(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Weapon");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetRecoil");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

}

