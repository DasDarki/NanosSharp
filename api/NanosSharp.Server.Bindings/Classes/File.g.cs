// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class File
{
    public static void Close(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Close");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Flush(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Flush");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static bool IsEOF(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "IsEOF");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool IsBad(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "IsBad");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool IsGood(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "IsGood");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool HasFailed(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "HasFailed");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static string Read(ILuaVM vm, int selfRef, double? length = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Read");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        if (length != null)
        {
             pc++;
             vm.PushNumber(length.Value);
        }
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static void ReadAsync(ILuaVM vm, int selfRef, double length, int callback)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "ReadAsync");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(length);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, callback);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static string ReadLine(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "ReadLine");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToString(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static Dictionary<string, object> ReadJSON(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "ReadJSON");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        vm.ClearStack();
        return r0;
    }

    public static void ReadJSONAsync(ILuaVM vm, int selfRef, int callback)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "ReadJSONAsync");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, callback);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void Seek(ILuaVM vm, int selfRef, double position)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Seek");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(position);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static double Size(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Size");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static void Skip(ILuaVM vm, int selfRef, double amount)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Skip");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushNumber(amount);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static double Tell(ILuaVM vm, int selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Tell");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static void Write(ILuaVM vm, int selfRef, string data)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Write");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(data);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static double Time(ILuaVM vm, string path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Time");
        pc++;
        vm.PushString(path);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool CreateDirectory(ILuaVM vm, string path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "CreateDirectory");
        pc++;
        vm.PushString(path);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static double Remove(ILuaVM vm, string path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Remove");
        pc++;
        vm.PushString(path);
        vm.MCall(pc, 1);
        var r0 = vm.ToNumber(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool Exists(ILuaVM vm, string path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "Exists");
        pc++;
        vm.PushString(path);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool IsDirectory(ILuaVM vm, string path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "IsDirectory");
        pc++;
        vm.PushString(path);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

    public static bool IsRegularFile(ILuaVM vm, string path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "File");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "IsRegularFile");
        pc++;
        vm.PushString(path);
        vm.MCall(pc, 1);
        var r0 = vm.ToBoolean(-1);
        vm.Pop();
        vm.ClearStack();
        return r0;
    }

}

