using NanosSharp.Interop.Arguments;
using NanosSharp.Interop.Reflection;
using NanosSharp.Packages;
using NanosSharp.Threading;

namespace NanosSharp.Entities;

public abstract partial class Entity
{
    private protected void Call(MethodSlot method, int requiredCount)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[1];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.TrimTrailingNil(requiredCount + 1);
            NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected TResult? CallResult<TResult>(MethodSlot method, int requiredCount)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[1];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.TrimTrailingNil(requiredCount + 1);
            var result = NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
            return ResultReader.Read<TResult>(ref result);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected void Call<T1>(MethodSlot method, int requiredCount, T1 argument1)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[2];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.TrimTrailingNil(requiredCount + 1);
            NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected TResult? CallResult<TResult, T1>(MethodSlot method, int requiredCount, T1 argument1)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[2];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.TrimTrailingNil(requiredCount + 1);
            var result = NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
            return ResultReader.Read<TResult>(ref result);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected void Call<T1, T2>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[3];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.TrimTrailingNil(requiredCount + 1);
            NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected TResult? CallResult<TResult, T1, T2>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[3];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.TrimTrailingNil(requiredCount + 1);
            var result = NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
            return ResultReader.Read<TResult>(ref result);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected void Call<T1, T2, T3>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[4];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.TrimTrailingNil(requiredCount + 1);
            NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected TResult? CallResult<TResult, T1, T2, T3>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[4];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.TrimTrailingNil(requiredCount + 1);
            var result = NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
            return ResultReader.Read<TResult>(ref result);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected void Call<T1, T2, T3, T4>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3, T4 argument4)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[5];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.Add(argument4);
            packer.TrimTrailingNil(requiredCount + 1);
            NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected TResult? CallResult<TResult, T1, T2, T3, T4>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3, T4 argument4)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[5];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.Add(argument4);
            packer.TrimTrailingNil(requiredCount + 1);
            var result = NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
            return ResultReader.Read<TResult>(ref result);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected void Call<T1, T2, T3, T4, T5>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[6];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.Add(argument4);
            packer.Add(argument5);
            packer.TrimTrailingNil(requiredCount + 1);
            NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected TResult? CallResult<TResult, T1, T2, T3, T4, T5>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[6];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.Add(argument4);
            packer.Add(argument5);
            packer.TrimTrailingNil(requiredCount + 1);
            var result = NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
            return ResultReader.Read<TResult>(ref result);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected void Call<T1, T2, T3, T4, T5, T6>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5, T6 argument6)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[7];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.Add(argument4);
            packer.Add(argument5);
            packer.Add(argument6);
            packer.TrimTrailingNil(requiredCount + 1);
            NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected TResult? CallResult<TResult, T1, T2, T3, T4, T5, T6>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5, T6 argument6)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[7];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.Add(argument4);
            packer.Add(argument5);
            packer.Add(argument6);
            packer.TrimTrailingNil(requiredCount + 1);
            var result = NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
            return ResultReader.Read<TResult>(ref result);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected void Call<T1, T2, T3, T4, T5, T6, T7>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5, T6 argument6, T7 argument7)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[8];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.Add(argument4);
            packer.Add(argument5);
            packer.Add(argument6);
            packer.Add(argument7);
            packer.TrimTrailingNil(requiredCount + 1);
            NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected TResult? CallResult<TResult, T1, T2, T3, T4, T5, T6, T7>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5, T6 argument6, T7 argument7)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[8];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.Add(argument4);
            packer.Add(argument5);
            packer.Add(argument6);
            packer.Add(argument7);
            packer.TrimTrailingNil(requiredCount + 1);
            var result = NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
            return ResultReader.Read<TResult>(ref result);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected void Call<T1, T2, T3, T4, T5, T6, T7, T8>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5, T6 argument6, T7 argument7, T8 argument8)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[9];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.Add(argument4);
            packer.Add(argument5);
            packer.Add(argument6);
            packer.Add(argument7);
            packer.Add(argument8);
            packer.TrimTrailingNil(requiredCount + 1);
            NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private protected TResult? CallResult<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(MethodSlot method, int requiredCount, T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5, T6 argument6, T7 argument7, T8 argument8)
    {
        MainThread.Ensure();
        Span<NativeArgument> buffer = stackalloc NativeArgument[9];
        var packer = new ArgumentPacker(buffer, stackalloc byte[NativeMethod.StackStringBytes]);
        try
        {
            packer.AddSelf(this);
            packer.Add(argument1);
            packer.Add(argument2);
            packer.Add(argument3);
            packer.Add(argument4);
            packer.Add(argument5);
            packer.Add(argument6);
            packer.Add(argument7);
            packer.Add(argument8);
            packer.TrimTrailingNil(requiredCount + 1);
            var result = NativeClass.GetMethod(method).Execute(PackageContext.Environment, ref packer, buffer);
            return ResultReader.Read<TResult>(ref result);
        }
        finally
        {
            packer.Dispose();
        }
    }
}
