// Autogenerated by the NanosSharp Server Bindings Generator (c) 2025 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class BDatabase
{
    public static void Close(ILuaVM vm, LuaRef selfRef)
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

    public static void ExecuteAsync(ILuaVM vm, LuaRef selfRef, string query, ILuaVM.CFunction? callback = null, params object[] parameters)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Database");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "ExecuteAsync");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(query);
        if (callback != null)
        {
             pc++;
             vm.PushManagedFunction(callback);
        }
        foreach (var a in parameters) {
            pc++;
            vm.PushObject(a);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Execute(ILuaVM vm, LuaRef selfRef, string query, out long r0, out string r1, params object[] parameters)
    {
        r0 = default;
        r1 = default;
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Database");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Execute");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(query);
        foreach (var a in parameters) {
            pc++;
            vm.PushObject(a);
        }
        vm.MCall(pc, 2);
        r1 = vm.ToString(-1);
        vm.Pop();
        r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
    }

    public static void SelectAsync(ILuaVM vm, LuaRef selfRef, string query, ILuaVM.CFunction? callback = null, params object[] parameters)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Database");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SelectAsync");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(query);
        if (callback != null)
        {
             pc++;
             vm.PushManagedFunction(callback);
        }
        foreach (var a in parameters) {
            pc++;
            vm.PushObject(a);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Select(ILuaVM vm, LuaRef selfRef, string query, out Dictionary<string, object>[] r0, out string r1, params object[] parameters)
    {
        r0 = default;
        r1 = default;
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Database");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Select");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(query);
        foreach (var a in parameters) {
            pc++;
            vm.PushObject(a);
        }
        vm.MCall(pc, 2);
        r1 = vm.ToString(-1);
        vm.Pop();
        r0 = vm.ToArray<Dictionary<string, object>>(-1);
        vm.ClearStack();
    }

}

