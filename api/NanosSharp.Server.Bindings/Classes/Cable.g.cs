// Autogenerated by the NanosSharp Server Bindings Generator (c) 2025 DasDarki / GPLv3

using NanosSharp.API;

namespace NanosSharp.Server.Bindings;

public class BCable : BEntity
{
    public static void AttachStartTo(ILuaVM vm, LuaRef selfRef, LuaRef other, LuaRef? relative_location = null, string? bone_name = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "AttachStartTo");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, other);
        if (relative_location != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, relative_location.Value);
        }
        if (bone_name != null)
        {
             pc++;
             vm.PushString(bone_name);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void AttachEndTo(ILuaVM vm, LuaRef selfRef, LuaRef other, LuaRef? relative_location = null, string? bone_name = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "AttachEndTo");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, other);
        if (relative_location != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, relative_location.Value);
        }
        if (bone_name != null)
        {
             pc++;
             vm.PushString(bone_name);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void DetachEnd(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "DetachEnd");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void DetachStart(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "DetachStart");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetCableSettings(ILuaVM vm, LuaRef selfRef, LuaRef length, long? num_segments = null, long? solver_iterations = null, bool? enable_stiffness = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetCableSettings");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, length);
        if (num_segments != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, num_segments.Value);
        }
        if (solver_iterations != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, solver_iterations.Value);
        }
        if (enable_stiffness != null)
        {
             pc++;
             vm.PushBoolean(enable_stiffness.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetForces(ILuaVM vm, LuaRef selfRef, LuaRef force, LuaRef? gravity_scale = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetForces");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, force);
        if (gravity_scale != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, gravity_scale.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetAngularLimits(ILuaVM vm, LuaRef selfRef, ConstraintMotion swing_1_motion, ConstraintMotion swing_2_motion, ConstraintMotion twist_motion, LuaRef? swing_1_limit = null, LuaRef? swing_2_limit = null, LuaRef? twist_limit = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetAngularLimits");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushEnum(swing_1_motion);
        pc++;
        vm.PushEnum(swing_2_motion);
        pc++;
        vm.PushEnum(twist_motion);
        if (swing_1_limit != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, swing_1_limit.Value);
        }
        if (swing_2_limit != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, swing_2_limit.Value);
        }
        if (twist_limit != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, twist_limit.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetLinearLimits(ILuaVM vm, LuaRef selfRef, ConstraintMotion x_motion, ConstraintMotion y_motion, ConstraintMotion z_motion, LuaRef? limit = null, LuaRef? restitution = null, bool? use_soft_constraint = null, LuaRef? stiffness = null, LuaRef? damping = null)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetLinearLimits");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.PushEnum(x_motion);
        pc++;
        vm.PushEnum(y_motion);
        pc++;
        vm.PushEnum(z_motion);
        if (limit != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, limit.Value);
        }
        if (restitution != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, restitution.Value);
        }
        if (use_soft_constraint != null)
        {
             pc++;
             vm.PushBoolean(use_soft_constraint.Value);
        }
        if (stiffness != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, stiffness.Value);
        }
        if (damping != null)
        {
             pc++;
             vm.RawGetI(ILuaVM.RegistryIndex, damping.Value);
        }
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static void SetRenderingSettings(ILuaVM vm, LuaRef selfRef, LuaRef width, long num_sides, long tile_material)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "SetRenderingSettings");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, width);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, num_sides);
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, tile_material);
        vm.MCall(pc, 0);
        vm.ClearStack();
    }

    public static LuaRef GetAttachedStartTo(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAttachedStartTo");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

    public static LuaRef GetAttachedEndTo(ILuaVM vm, LuaRef selfRef)
    {
        int pc = 0;
        vm.PushGlobalTable();
        vm.GetField(-1, "Cable");
        vm.GetField(-1, "__function");
        vm.GetField(-1, "GetAttachedEndTo");
        pc++;
        vm.RawGetI(ILuaVM.RegistryIndex, selfRef);
        vm.MCall(pc, 1);
        var r0 = vm.Ref(ILuaVM.RegistryIndex);
        vm.ClearStack();
        return r0;
    }

}

