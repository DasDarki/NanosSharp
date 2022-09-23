// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Pickable
{
    public static void AddSkeletalMeshAttached(ILuaVM vm, int selfRef, string id, string skeletal_mesh_path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "AddSkeletalMeshAttached");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(id);
        pc++;
        vm.PushString(skeletal_mesh_path);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void AddStaticMeshAttached(ILuaVM vm, int selfRef, string id, string static_mesh_path, string? socket = null, LuaRef? relative_location = null, LuaRef? relative_rotation = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "AddStaticMeshAttached");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(id);
        pc++;
        vm.PushString(static_mesh_path);
        if (socket != null)
        {
             pc++;
             vm.PushString(socket);
        }
        if (relative_location != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, relative_location.Value);
        }
        if (relative_rotation != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, relative_rotation.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void PullUse(ILuaVM vm, int selfRef, double? release_use_after = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "PullUse");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        if (release_use_after != null)
        {
             pc++;
             vm.PushNumber(release_use_after.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void ReleaseUse(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "ReleaseUse");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void RemoveSkeletalMeshAttached(ILuaVM vm, int selfRef, string id)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "RemoveSkeletalMeshAttached");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(id);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void RemoveStaticMeshAttached(ILuaVM vm, int selfRef, string id)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "RemoveStaticMeshAttached");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(id);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetAttachmentSettings(ILuaVM vm, int selfRef, LuaRef relative_location, LuaRef? relative_rotation = null, string? socket = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetAttachmentSettings");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, relative_location);
        if (relative_rotation != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, relative_rotation.Value);
        }
        if (socket != null)
        {
             pc++;
             vm.PushString(socket);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetCrosshairMaterial(ILuaVM vm, int selfRef, string material_asset)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetCrosshairMaterial");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(material_asset);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetPickable(ILuaVM vm, int selfRef, bool is_pickable)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetPickable");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushBoolean(is_pickable);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static string GetAssetName(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAssetName");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static LuaRef? GetHandler(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Pickable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetHandler");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

}

