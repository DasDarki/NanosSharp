// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public static class JSON
{
    public static string stringify(ILuaVM vm, Dictionary<string, object> value)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "JSON");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "stringify");
        pc++;
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static object parse(ILuaVM vm, string value)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "JSON");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "parse");
        pc++;
        vm.PushString(value);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

}

