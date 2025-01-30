// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Entity
{
    public static LuaRef GetID(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetID");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static Dictionary<string, object> GetClass(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetClass");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToTable(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool IsA(ILuaVM vm, LuaRef selfRef, Dictionary<string, object> @class)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "IsA");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushTable(@class);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static ILuaVM.CFunction Subscribe(ILuaVM vm, LuaRef selfRef, string event_name, ILuaVM.CFunction callback)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Subscribe");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
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

    public static ILuaVM.CFunction SubscribeRemote(ILuaVM vm, LuaRef selfRef, string event_name, ILuaVM.CFunction callback)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SubscribeRemote");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
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

    public static void Unsubscribe(ILuaVM vm, LuaRef selfRef, string event_name, ILuaVM.CFunction? callback = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Unsubscribe");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
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

    public static void SetValue(ILuaVM vm, LuaRef selfRef, string key, object value, bool? sync_on_clients = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetValue");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(key);
        pc++;
        vm.PushObject(value);
        if (sync_on_clients != null)
        {
             pc++;
             vm.PushBoolean(sync_on_clients.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static object GetValue(ILuaVM vm, LuaRef selfRef, string key, object fallback)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetValue");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(key);
        pc++;
        vm.PushObject(fallback);
        vm.MCall(pc, 1);
        var r0 = vm.ToObject(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string[] GetAllValuesKeys(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAllValuesKeys");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToArray<string>(-1);
        vm.ClearStack();
        return r0;
    }

    public static void Destroy(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Destroy");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static bool IsValid(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "IsValid");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static void CallRemoteEvent(ILuaVM vm, LuaRef selfRef, string event_name, LuaRef player, params object[] args)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "CallRemoteEvent");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
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

    public static void BroadcastRemoteEvent(ILuaVM vm, LuaRef selfRef, string event_name, params object[] args)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "BroadcastRemoteEvent");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(event_name);
        pc++;
        foreach (var a in args) {
            vm.PushObject(a);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static LuaRef[] GetAll(ILuaVM vm)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "GetAll");
        vm.MCall(pc, 1);
        var r0 = vm.ToRefArray(-1);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetByIndex(ILuaVM vm, LuaRef index)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "GetByIndex");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, index);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetCount(ILuaVM vm)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "GetCount");
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static Dictionary<string, object> Inherit(ILuaVM vm, string name, Dictionary<string, object>? custom_values = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "Inherit");
        pc++;
        vm.PushString(name);
        if (custom_values != null)
        {
             pc++;
             vm.PushTable(custom_values);
        }
        vm.MCall(pc, 1);
        var r0 = vm.ToTable(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static Dictionary<string, object>[] GetInheritedClasses(ILuaVM vm, bool? recursively = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "GetInheritedClasses");
        if (recursively != null)
        {
             pc++;
             vm.PushBoolean(recursively.Value);
        }
        vm.MCall(pc, 1);
        var r0 = vm.ToArray<Dictionary<string, object>>(-1);
        vm.ClearStack();
        return r0;
    }

    public static Dictionary<string, object>? GetParentClass(ILuaVM vm)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "GetParentClass");
        vm.MCall(pc, 1);
        var r0 = vm.ToTable(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool IsChildOf(ILuaVM vm, Dictionary<string, object> @class)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "IsChildOf");
        pc++;
        vm.PushTable(@class);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static ILuaVM.CFunction Subscribe(ILuaVM vm, string event_name, ILuaVM.CFunction callback)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
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

    public static ILuaVM.CFunction SubscribeRemote(ILuaVM vm, string event_name, ILuaVM.CFunction callback)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Entity");
        vm.GetField(-1, "SubscribeRemote");
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
        vm.GetField(-1, "Entity");
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

