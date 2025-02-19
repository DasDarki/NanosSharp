// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public static class NanosTable
{
    public static string Dump(ILuaVM vm, Dictionary<string, object> table)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "NanosTable");
        vm.GetField(-1, "Dump");
        pc++;
        vm.PushTable(table);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static Dictionary<string, object> ShallowCopy(ILuaVM vm, Dictionary<string, object> table)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "NanosTable");
        vm.GetField(-1, "ShallowCopy");
        pc++;
        vm.PushTable(table);
        vm.MCall(pc, 1);
        var r0 = vm.ToTable(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

}

