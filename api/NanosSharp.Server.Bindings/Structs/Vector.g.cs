// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Vector
{
    public static bool Equals(ILuaVM vm, LuaRef selfRef, LuaRef other, double? tolerance = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
        vm.GetField(-1, "Equals");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, other);
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

    public static double Distance(ILuaVM vm, LuaRef selfRef, LuaRef other)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
        vm.GetField(-1, "Distance");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, other);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double DistanceSquared(ILuaVM vm, LuaRef selfRef, LuaRef other)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
        vm.GetField(-1, "DistanceSquared");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, other);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetUnsafeNormal(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
        vm.GetField(-1, "GetUnsafeNormal");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetSafeNormal(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
        vm.GetField(-1, "GetSafeNormal");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static bool IsNearlyZero(ILuaVM vm, LuaRef selfRef, double? tolerance = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
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

    public static bool IsZero(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
        vm.GetField(-1, "IsZero");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool Normalize(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
        vm.GetField(-1, "Normalize");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double Size(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
        vm.GetField(-1, "Size");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double SizeSquared(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
        vm.GetField(-1, "SizeSquared");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static LuaRef Rotation(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Vector");
        vm.GetField(-1, "Rotation");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

}

