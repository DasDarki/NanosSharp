// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Database
{
    public static void Close(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Database");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Close");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Execute(ILuaVM vm, int selfRef, string query, ILuaVM.CFunction? callback = null, params object[] parameters)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Database");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Execute");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(query);
        if (callback != null)
        {
             pc++;
             vm.PushManagedFunction(callback);
        }
        pc++;
        foreach (var a in parameters) {
            vm.PushObject(a);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void ExecuteSync(ILuaVM vm, int selfRef, string query, out double r0, out string r1, params object[] parameters)
    {
        r0 = default;
        r1 = default;
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Database");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "ExecuteSync");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(query);
        pc++;
        foreach (var a in parameters) {
            vm.PushObject(a);
        }
        vm.MCall(pc, 2);
        r1 = vm.ToString(-1);
        vm.Pop();
        r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
    }

    public static void Select(ILuaVM vm, int selfRef, string query, ILuaVM.CFunction? callback = null, params object[] parameters)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Database");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Select");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(query);
        if (callback != null)
        {
             pc++;
             vm.PushManagedFunction(callback);
        }
        pc++;
        foreach (var a in parameters) {
            vm.PushObject(a);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SelectSync(ILuaVM vm, int selfRef, string query, out Dictionary<string, object>[] r0, out string r1, params object[] parameters)
    {
        r0 = default;
        r1 = default;
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Database");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SelectSync");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(query);
        pc++;
        foreach (var a in parameters) {
            vm.PushObject(a);
        }
        vm.MCall(pc, 2);
        r1 = vm.ToString(-1);
        vm.Pop();
        r0 = vm.ToArray<Dictionary<string, object>>(-1);
        vm.ClearStack();
    }

}

