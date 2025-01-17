// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Rotator
{
    public static LuaRef GetForwardVector(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "GetForwardVector");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetRightVector(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "GetRightVector");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetUpVector(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "GetUpVector");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef RotateVector(ILuaVM vm, int selfRef, LuaRef vector)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "RotateVector");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, vector);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static void Normalize(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "Normalize");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static LuaRef UnrotateVector(ILuaVM vm, int selfRef, LuaRef vector)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "UnrotateVector");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, vector);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef Quaternion(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "Quaternion");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetNormalized(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "GetNormalized");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static bool IsNearlyZero(ILuaVM vm, int selfRef, double? tolerance = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "IsNearlyZero");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        if (tolerance != null)
        {
             pc++;
             vm.PushNumber(tolerance.Value);
        }
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool IsZero(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "IsZero");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static LuaRef Random(ILuaVM vm, bool? roll = null, LuaRef? min = null, LuaRef? max = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Rotator");
        vm.GetField(-1, "Random");
        if (roll != null)
        {
             pc++;
             vm.PushBoolean(roll.Value);
        }
        if (min != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, min.Value);
        }
        if (max != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, max.Value);
        }
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

}

