// Autogenerated by the NanosSharp Server Bindings Generator (c) 2025 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class BStaticMesh : BEntity
{
    public static void SetMesh(ILuaVM vm, LuaRef selfRef, string static_mesh_asset)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "StaticMesh");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetMesh");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(static_mesh_asset);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static string GetMesh(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "StaticMesh");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetMesh");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

}

