// Autogenerated by the NanosSharp Server Bindings Generator (c) 2022 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class Paintable
{
    public static void SetMaterial(ILuaVM vm, int selfRef, string material_path, double? index = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Paintable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetMaterial");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(material_path);
        if (index != null)
        {
             pc++;
             vm.PushNumber(index.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void ResetMaterial(ILuaVM vm, int selfRef, double? index = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Paintable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "ResetMaterial");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        if (index != null)
        {
             pc++;
             vm.PushNumber(index.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetMaterialColorParameter(ILuaVM vm, int selfRef, string parameter_name, LuaRef color)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Paintable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetMaterialColorParameter");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(parameter_name);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, color);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetMaterialScalarParameter(ILuaVM vm, int selfRef, string parameter_name, double scalar)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Paintable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetMaterialScalarParameter");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(parameter_name);
        pc++;
        vm.PushNumber(scalar);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetMaterialTextureParameter(ILuaVM vm, int selfRef, string parameter_name, string texture_path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Paintable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetMaterialTextureParameter");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(parameter_name);
        pc++;
        vm.PushString(texture_path);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetMaterialVectorParameter(ILuaVM vm, int selfRef, string parameter_name, LuaRef vector)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Paintable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetMaterialVectorParameter");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(parameter_name);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, vector);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetPhysicalMaterial(ILuaVM vm, int selfRef, string physical_material_path)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Paintable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetPhysicalMaterial");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushString(physical_material_path);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

}

