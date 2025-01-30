// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class table
{
    public static double insert(ILuaVM vm, Dictionary<string, object> tbl, double? position = null, object? value = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "table");
        vm.GetField(-1, "insert");
        pc++;
        vm.PushTable(tbl);
        if (position != null)
        {
             pc++;
             vm.PushNumber(position.Value);
        }
        if (value != null)
        {
             pc++;
             vm.PushObject(value);
        }
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static void sort(ILuaVM vm, Dictionary<string, object> tbl, ILuaVM.CFunction sorter)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "table");
        vm.GetField(-1, "sort");
        pc++;
        vm.PushTable(tbl);
        pc++;
        vm.PushManagedFunction(sorter);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static Dictionary<string, object> move(ILuaVM vm, Dictionary<string, object> source_table, double from, double to, double dest, Dictionary<string, object>? dest_table = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "table");
        vm.GetField(-1, "move");
        pc++;
        vm.PushTable(source_table);
        pc++;
        vm.PushNumber(from);
        pc++;
        vm.PushNumber(to);
        pc++;
        vm.PushNumber(dest);
        if (dest_table != null)
        {
             pc++;
             vm.PushTable(dest_table);
        }
        vm.MCall(pc, 1);
        var r0 = vm.ToTable(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static object remove(ILuaVM vm, Dictionary<string, object> tbl, double index)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "table");
        vm.GetField(-1, "remove");
        pc++;
        vm.PushTable(tbl);
        pc++;
        vm.PushNumber(index);
        vm.MCall(pc, 1);
        var r0 = vm.ToObject(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string concat(ILuaVM vm, Dictionary<string, object> tbl, string? separator = null, double? start_pos = null, double? end_pos = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "table");
        vm.GetField(-1, "concat");
        pc++;
        vm.PushTable(tbl);
        if (separator != null)
        {
             pc++;
             vm.PushString(separator);
        }
        if (start_pos != null)
        {
             pc++;
             vm.PushNumber(start_pos.Value);
        }
        if (end_pos != null)
        {
             pc++;
             vm.PushNumber(end_pos.Value);
        }
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

}

