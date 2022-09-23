// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public static class Timer
{
    public static double SetTimeout(ILuaVM vm, ILuaVM.CFunction callback, double? milliseconds = null, params object[] parameters)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "SetTimeout");
        pc++;
        vm.PushManagedFunction(callback);
        if (milliseconds != null)
        {
             pc++;
             vm.PushNumber(milliseconds.Value);
        }
        pc++;
        foreach (var a in parameters) {
            vm.PushObject(a);
        }
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double SetInterval(ILuaVM vm, ILuaVM.CFunction callback, double? milliseconds = null, params object[] parameters)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "SetInterval");
        pc++;
        vm.PushManagedFunction(callback);
        if (milliseconds != null)
        {
             pc++;
             vm.PushNumber(milliseconds.Value);
        }
        pc++;
        foreach (var a in parameters) {
            vm.PushObject(a);
        }
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static void ClearTimeout(ILuaVM vm, double timeout_id)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "ClearTimeout");
        pc++;
        vm.PushNumber(timeout_id);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void ClearInterval(ILuaVM vm, double interval_id)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "ClearInterval");
        pc++;
        vm.PushNumber(interval_id);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Bind(ILuaVM vm, double timer_id, LuaRef actor)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "Bind");
        pc++;
        vm.PushNumber(timer_id);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, actor);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static bool IsValid(ILuaVM vm, double timer_id)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "IsValid");
        pc++;
        vm.PushNumber(timer_id);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetElapsedTime(ILuaVM vm, double timer_id)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "GetElapsedTime");
        pc++;
        vm.PushNumber(timer_id);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double GetRemainingTime(ILuaVM vm, double timer_id)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "GetRemainingTime");
        pc++;
        vm.PushNumber(timer_id);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static void Pause(ILuaVM vm, double timer_id)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "Pause");
        pc++;
        vm.PushNumber(timer_id);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Resume(ILuaVM vm, double timer_id)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "Resume");
        pc++;
        vm.PushNumber(timer_id);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void ResetElapsedTime(ILuaVM vm, double timer_id)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Timer");
        vm.GetField(-1, "ResetElapsedTime");
        pc++;
        vm.PushNumber(timer_id);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

}
