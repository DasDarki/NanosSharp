// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Prop : Paintable
{
    public static void SetGrabMode(ILuaVM vm, int selfRef, int grab_mode)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Prop");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetGrabMode");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, grab_mode);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetPhysicsDamping(ILuaVM vm, int selfRef, double linear_damping, double angular_damping)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Prop");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetPhysicsDamping");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(linear_damping);
        pc++;
        vm.PushNumber(angular_damping);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static string GetAssetName(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Prop");
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

    public static int? GetHandler(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Prop");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetHandler");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static int GetGrabMode(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Prop");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetGrabMode");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

}

