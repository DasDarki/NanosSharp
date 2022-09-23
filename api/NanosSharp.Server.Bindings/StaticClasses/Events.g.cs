// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public static class Events
{
    public static void Call(ILuaVM vm, string event_name, params object[] args)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Events");
        vm.GetField(-1, "Call");
        pc++;
        vm.PushString(event_name);
        pc++;
        foreach (var a in args) {
            vm.PushObject(a);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void CallRemote(ILuaVM vm, string event_name, LuaRef player, params object[] args)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Events");
        vm.GetField(-1, "CallRemote");
        pc++;
        vm.PushString(event_name);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, player);
        pc++;
        foreach (var a in args) {
            vm.PushObject(a);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void BroadcastRemote(ILuaVM vm, string event_name, params object[] args)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Events");
        vm.GetField(-1, "BroadcastRemote");
        pc++;
        vm.PushString(event_name);
        pc++;
        foreach (var a in args) {
            vm.PushObject(a);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static ILuaVM.CFunction Subscribe(ILuaVM vm, string event_name, ILuaVM.CFunction callback)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Events");
        vm.GetField(-1, "Subscribe");
        pc++;
        vm.PushString(event_name);
        pc++;
        vm.PushManagedFunction(callback);
        vm.MCall(pc, 1);
        var r0 = vm.ToCFunction(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static void Unsubscribe(ILuaVM vm, string event_name, ILuaVM.CFunction? callback = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Events");
        vm.GetField(-1, "Unsubscribe");
        pc++;
        vm.PushString(event_name);
        if (callback != null)
        {
             pc++;
             vm.PushManagedFunction(callback);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

}
