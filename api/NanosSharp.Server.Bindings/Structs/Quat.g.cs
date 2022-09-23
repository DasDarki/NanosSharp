// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Quat
{
    public static void Normalize(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Quat");
        vm.GetField(-1, "Normalize");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static int Rotator(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Quat");
        vm.GetField(-1, "Rotator");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static int Inverse(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Quat");
        vm.GetField(-1, "Inverse");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

}

