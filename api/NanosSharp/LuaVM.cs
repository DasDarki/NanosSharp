﻿using System.Runtime.InteropServices;
using System.Text;
using NanosSharp.API;

namespace NanosSharp;

/// <summary>
/// The implementation of <see cref="ILuaVM"/>.
/// </summary>
internal class LuaVM : ILuaVM
{
    private static readonly Encoding Encoding = Encoding.UTF8;
    private delegate int LuaCFunction(IntPtr luaState);
    
    /// <summary>
    /// The pointer to the native luaVM.
    /// </summary>
    internal IntPtr Handle { get; }
    
    /// <summary>
    /// Nanos World uses a scripting lua enviroment to identify packages. This is the pointer to that lua environment.
    /// We need to inject it at some points to ensure that the lua environment is correctly set up for the package.
    /// Thanks Syed for helping &lt;3
    /// </summary>
    internal IntPtr LuaEnv { get; set; }

    internal LuaVM(IntPtr handle)
    {
        Handle = handle;
    }

    public void InjectEnvironment()
    {
        if (LuaEnv == IntPtr.Zero)
            return;
        
        // Access the registry table
        PushString("environments"); // Push key
        unsafe
        {
            Natives.Lua_RawGet(Handle, ILuaVM.RegistryIndex); // Get registry["environments"]
        }

        // Push the Lua environment pointer onto the stack
        PushLightUserData(LuaEnv);

        // Get the environment table for the Lua environment (this is the table that contains the Lua environment)
        RawGet(-2); // environments[LuaEnv]

        // The stack now looks like this:
        // -1: Environment
        // -2: environments table
        // -> We can now remove the environments table from the stack
        Remove(-2);
    }

    public int AbsIndex(int idx)
    {
        unsafe
        {
            return Natives.Lua_AbsIndex(Handle, idx);
        }
    }

    public int GetTop()
    {
        unsafe
        {
            return Natives.Lua_GetTop(Handle);
        }
    }

    public void SetTop(int idx)
    {
        unsafe
        {
            Natives.Lua_SetTop(Handle, idx);
        }
    }

    public void PushValue(int idx)
    {
        unsafe
        {
            Natives.Lua_PushValue(Handle, idx);
        }
    }

    public void Rotate(int idx, int n)
    {
        unsafe
        {
            Natives.Lua_Rotate(Handle, idx, n);
        }
    }

    public void Copy(int fromIdx, int toIdx)
    {
        unsafe
        {
            Natives.Lua_Copy(Handle, fromIdx, toIdx);
        }
    }

    public int CheckStack(int n)
    {
        unsafe
        {
            return Natives.Lua_CheckStack(Handle, n);
        }
    }

    public void XMove(ILuaVM to, int n)
    {
        unsafe
        {
            Natives.Lua_XMove(Handle, ((LuaVM)to).Handle, n);
        }
    }

    public bool IsNumber(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsNumber(Handle, idx) != 0;
        }
    }

    public bool IsString(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsString(Handle, idx) != 0;
        }
    }

    public bool IsCFunction(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsCFunction(Handle, idx) != 0;
        }
    }

    public bool IsInteger(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsInteger(Handle, idx) != 0;
        }
    }

    public bool IsUserData(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsUserData(Handle, idx) != 0;
        }
    }

    public LuaType Type(int idx)
    {
        unsafe
        {
            return Natives.Lua_Type(Handle, idx);
        }
    }

    public string TypeName(LuaType tp)
    {
        uint len = 0;
            
        unsafe
        {
            uint *ptr = &len;
            IntPtr str = Natives.Lua_TypeName(Handle, (int) tp, (IntPtr) ptr);
            if (str == IntPtr.Zero)
                return string.Empty;

            return Encoding.UTF8.GetString((byte*) str, (int) len);
        }
    }

    public double ToNumberX(int idx, out bool isNum)
    {
        unsafe
        {
            int isNumInt;
            double result = Natives.Lua_ToNumberX(Handle, idx, &isNumInt);
            isNum = isNumInt != 0;
            return result;
        }
    }

    public long ToIntegerX(int idx, out bool isNum)
    {
        unsafe
        {
            int isNumInt;
            long result = Natives.Lua_ToIntegerX(Handle, idx, &isNumInt);
            isNum = isNumInt != 0;
            return result;
        }
    }

    public bool ToBoolean(int idx)
    {
        unsafe
        {
            return Natives.Lua_ToBoolean(Handle, idx) != 0;
        }
    }

    public ulong RawLen(int idx)
    {
        unsafe
        {
            return Natives.Lua_RawLen(Handle, idx);
        }
    }

    public ILuaVM.CFunction? ToCFunction(int idx)
    {
        unsafe
        {
            var funcPtr = Natives.Lua_ToCFunction(Handle, idx);
            if (funcPtr == IntPtr.Zero)
                return null;
            
            var nativeDelegate = (delegate* unmanaged[Cdecl]<IntPtr, int>) funcPtr;
            return state => nativeDelegate(((LuaVM) state).Handle);
        }
    }

    public IntPtr ToUserData(int idx)
    {
        unsafe
        {
            return Natives.Lua_ToUserData(Handle, idx);
        }
    }

    public IntPtr ToPointer(int idx)
    {
        unsafe
        {
            return Natives.Lua_ToPointer(Handle, idx);
        }
    }

    public void Arith(LuaOperator op)
    {
        unsafe
        {
            Natives.Lua_Arith(Handle, op);
        }
    }

    public bool RawEqual(int idx1, int idx2)
    {
        unsafe
        {
            return Natives.Lua_RawEqual(Handle, idx1, idx2) != 0;
        }
    }

    public bool Compare(int idx1, int idx2, LuaComparator op)
    {
        unsafe
        {
            return Natives.Lua_Compare(Handle, idx1, idx2, op) != 0;
        }
    }

    public void PushNil()
    {
        unsafe
        {
            Natives.Lua_PushNil(Handle);
        }
    }

    public void PushNumber(double n)
    {
        unsafe
        {
            Natives.Lua_PushNumber(Handle, n);
        }
    }

    public void PushInteger(int n)
    {
        unsafe
        {
            Natives.Lua_PushInteger(Handle, n);
        }
    }

    public void PushLString(string s, uint len)
    {
        byte[] buff = Encoding.GetBytes(s);
        
        unsafe
        {
            fixed (byte* ptr = buff)
            {
                Natives.Lua_PushLString(Handle, (IntPtr) ptr, len);
            }
        }
    }

    public void PushString(string s)
    {
        byte[] buff = Encoding.GetBytes(s);
        
        unsafe
        {
            fixed (byte* ptr = buff)
            {
                Natives.Lua_PushString(Handle, (IntPtr) ptr);
            }
        }
    }

    public void PushCClosure(IntPtr fnPtr, int n)
    {
        unsafe
        {
            Natives.Lua_PushCClosure(Handle, fnPtr, n);
        }
    }

    public void PushBoolean(bool b)
    {
        unsafe
        {
            Natives.Lua_PushBoolean(Handle, b ? 1 : 0);
        }
    }

    public void PushObject(object? o)
    {
        if (o == null)
        {
            PushNil();
            return;
        }
        
        if (o is Enum e)
        {
            PushEnum(e);
            return;
        }
        
        switch (o)
        {
            case LuaRef r:
                RawGetI(ILuaVM.RegistryIndex, r);
                break;
            case ILuaUnit u:
                RawGetI(ILuaVM.RegistryIndex, u.Handle);
                break;
            case bool b:
                PushBoolean(b);
                break;
            case float f:
                PushNumber(f);
                break;
            case double d:
                PushNumber(d);
                break;
            case long i:
                PushNumber((int)i);
                break;
            case int i:
                PushNumber(i);
                break;
            case string s:
                PushString(s);
                break;
            case ILuaVM.CFunction c:
                PushManagedFunction(c);
                break;
        }
    }

    public void PushEnum(Enum e)
    {
        PushInteger(Convert.ToInt32(e));
    }

    public void PushTable(Dictionary<string, object?> table)
    {
        CreateTable(table.Count, 0);
        
        foreach (var (key, value) in table)
        {
            PushString(key);
            PushObject(value);
            SetTable(-3);
        }
    }

    public object? ToObject(int idx)
    {
        var type = Type(idx);
        switch (type)
        {
            case LuaType.Nil:
                // nothing needs to be done, nil is cleaned up from the stack automatically
                return null;
            case LuaType.Boolean:
                return ToBoolean(idx);
            case LuaType.Number:
                return ToNumber(idx);
            case LuaType.String:
                return ToString(idx);
            case LuaType.Function:
                return ToCFunction(idx);
            case LuaType.UserData:
                return ToUserData(idx);
            case LuaType.Table:
                return ToTable(idx);
            case LuaType.LightUserData:
                return ToInteger(idx);
            default:
                throw new NotSupportedException("The type " + type + " is not supported for ToObject.");
        }
    }

    public void PushArray(Array arr)
    {
        CreateTable(arr.Length, 0);
        for (int i = 0; i < arr.Length; i++)
        {
            var val = arr.GetValue(i);
            if (val != null)
            {
                PushObject(val);
            }
            else
            {
                PushNil();
            }
            
            RawSetI(-2, i + 1);
        }
    }

    public object?[] ToArray(int idx)
    {
        List<object?> list = new();
        
        PushNil();
        while (Next(idx) != 0)
        {
            list.Add(ToObject(-1));
            Pop();
        }
        
        return list.ToArray();
    }

    public T?[] ToArray<T>(int idx)
    {
        var arr = ToArray(idx);
        return arr.Select(o =>
        {
            try
            {
                if (typeof(T).IsEnum)
                {
                    return (T) Enum.ToObject(typeof(T), Convert.ToInt32(o));
                }
                
                return (T) o!;
            }
            catch
            {
                return default;
            }
        }).ToArray();
    }

    public T ToEnum<T>(int idx) where T : Enum
    {
        return (T) Enum.ToObject(typeof(T), ToInteger(idx));
    }

    public LuaRef[] ToRefArray(int idx)
    {
        List<LuaRef> list = new();
        
        PushNil();
        while (Next(idx) != 0)
        {
            list.Add(Ref(ILuaVM.RegistryIndex));
        }
        
        return list.ToArray();
    }

    public Dictionary<string, object?> ToTable(int idx)
    {
        var table = new Dictionary<string, object?>();

        PushNil();
        while (Next(idx) != 0)
        {
            var type = Type(-2);
            var key = type == LuaType.String ? ToString(-2) : ToInteger(-2).ToString();
            var value = ToObject(-1);
            table.Add(key, value);
            Pop();
        }
        
        return table;
    }

    public void PushLightUserData(IntPtr p)
    {
        unsafe
        {
            Natives.Lua_PushLightUserData(Handle, p);
        }
    }

    public int GetGlobal(string name)
    {
        byte[] buff = Encoding.GetBytes(name);
        
        unsafe
        {
            fixed (byte* ptr = buff)
            {
                return Natives.Lua_GetGlobal(Handle, (IntPtr) ptr);
            }
        }
    }

    public int GetTable(int idx)
    {
        unsafe
        {
            return Natives.Lua_GetTable(Handle, idx);
        }
    }

    public int GetField(int idx, string k)
    {
        byte[] buff = Encoding.GetBytes(k);
        
        unsafe
        {
            fixed (byte* ptr = buff)
            {
                return Natives.Lua_GetField(Handle, idx, (IntPtr) ptr);
            }
        }
    }

    public int GetI(int idx, long n)
    {
        unsafe
        {
            return Natives.Lua_GetI(Handle, idx, n);
        }
    }

    public int RawGet(int idx)
    {
        unsafe
        {
            return Natives.Lua_RawGet(Handle, idx);
        }
    }

    public int RawGetI(int idx, long n)
    {
        unsafe
        {
            return Natives.Lua_RawGetI(Handle, idx, n);
        }
    }

    public void RawSetI(int idx, long n)
    {
        unsafe
        {
            Natives.Lua_RawSetI(Handle, idx, n);
        }
    }

    public int RawGetP(int idx, IntPtr p)
    {
        unsafe
        {
            return Natives.Lua_RawGetP(Handle, idx, p);
        }
    }

    public void CreateTable(int narr, int nrec)
    {
        unsafe
        {
            Natives.Lua_CreateTable(Handle, narr, nrec);
        }
    }

    public IntPtr NewUserData(uint sz)
    {
        unsafe
        {
            return Natives.Lua_NewUserData(Handle, sz);
        }
    }

    public int GetMetaTable(int objIndex)
    {
        unsafe
        {
            return Natives.Lua_GetMetaTable(Handle, objIndex);
        }
    }

    public void SetGlobal(string name)
    {
        byte[] buff = Encoding.GetBytes(name);
        
        unsafe
        {
            fixed (byte *ptr = &buff[0])
            {
                Natives.Lua_SetGlobal(Handle, (IntPtr) ptr);
            }
        }
    }

    public void SetTable(int idx)
    {
        unsafe
        {
            Natives.Lua_SetTable(Handle, idx);
        }
    }

    public void SetField(int idx, string k)
    {
        byte[] buff = Encoding.GetBytes(k);
        
        unsafe
        {
            fixed (byte *ptr = &buff[0])
            {
                Natives.Lua_SetField(Handle, idx, (IntPtr) ptr);
            }
        }
    }

    public void RawSet(int idx)
    {
        unsafe
        {
            Natives.Lua_RawSet(Handle, idx);
        }
    }

    public int SetMetaTable(int objIndex)
    {
        unsafe
        {
            return Natives.Lua_SetMetaTable(Handle, objIndex);
        }
    }

    public void Call(int nargs, int nresults)
    {
        unsafe
        {
            Natives.Lua_Call(Handle, nargs, nresults);
        }
    }

    public int PCall(int nargs, int nresults, int errfunc)
    {
        unsafe
        {
            return Natives.Lua_PCall(Handle, nargs, nresults, errfunc);
        }
    }

    public int Yield(int nresults)
    {
        unsafe
        {
            return Natives.Lua_Yield(Handle, nresults);
        }
    }

    public double ToNumber(int idx)
    {
        unsafe
        {
            return Natives.Lua_ToNumber(Handle, idx);
        }
    }

    public long ToInteger(int idx)
    {
        unsafe
        {
            return Natives.Lua_ToInteger(Handle, idx);
        }
    }

    public void Pop(int n = 1)
    {
        unsafe
        {
            Natives.Lua_Pop(Handle, n);
        }
    }

    public void NewTable()
    {
        unsafe
        {
            Natives.Lua_NewTable(Handle);
        }
    }

    public unsafe void PushCFunction(delegate* unmanaged[Cdecl]<IntPtr, int> fnPtr)
    {
        IntPtr ptr = (IntPtr) fnPtr;
        if (ptr == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(fnPtr));
        }
        
        Natives.Lua_PushCFunction(Handle, ptr);
    }

    public bool IsFunction(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsFunction(Handle, idx) != 0;
        }
    }

    public bool IsTable(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsTable(Handle, idx) != 0;
        }
    }

    public bool IsLightUserData(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsLightUserData(Handle, idx) != 0;
        }
    }

    public bool IsNil(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsNil(Handle, idx) != 0;
        }
    }

    public bool IsBoolean(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsBoolean(Handle, idx) != 0;
        }
    }

    public bool IsNone(int idx)
    {
        unsafe
        {
            return Natives.Lua_IsNone(Handle, idx) != 0;
        }
    }

    public void PushGlobalTable()
    {
        unsafe
        {
            Natives.Lua_PushGlobalTable(Handle);
        }
    }

    public void GetUserValue(int idx)
    {
        unsafe
        {
            Natives.Lua_GetUserValue(Handle, idx);
        }
    }

    public void SetUserValue(int idx)
    {
        unsafe
        {
            Natives.Lua_SetUserValue(Handle, idx);
        }
    }

    public string ToString(int idx)
    {
        uint len = 0;
        
        unsafe
        {
            uint *ptr = &len;
            IntPtr str = Natives.Lua_ToLString(Handle, idx, (IntPtr) ptr);
            if (str == IntPtr.Zero)
                return string.Empty;
            
            return Encoding.UTF8.GetString((byte*) str, (int) len);
        }
    }

    public void MCall(int args, int results)
    {
        int errorCode = PCall(args, results, 0);
        if (errorCode != 0)
        {
            string error = ToString(-1);
            Pop();
            
            throw new Exception("Lua Error: " + error);
        }
    }

    public void Insert(int idx)
    {
        unsafe
        {
            Natives.Lua_Insert(Handle, idx);
        }
    }

    public void Remove(int idx)
    {
        unsafe
        {
            Natives.Lua_Remove(Handle, idx);
        }
    }

    public void Replace(int idx)
    {
        unsafe
        {
            Natives.Lua_Replace(Handle, idx);
        }
    }

    public int NewMetaTable(string name)
    {
        byte[] buff = Encoding.GetBytes(name);
        
        unsafe
        {
            fixed (byte *ptr = &buff[0])
            {
                return Natives.Lua_NewMetaTable(Handle, (IntPtr) ptr);
            }
        }
    }

    public int Next(int idx)
    {
        unsafe
        {
            return Natives.Lua_Next(Handle, idx);
        }
    }

    public void Concat(int n)
    {
        unsafe
        {
            Natives.Lua_Concat(Handle, n);
        }
    }

    public void Len(int idx)
    {
        unsafe
        {
            Natives.Lua_Len(Handle, idx);
        }
    }

    public void PushManagedFunction(ILuaVM.CFunction fn)
    {
        PushManagedClosure(fn, 0);
    }

    public void PushManagedClosure(ILuaVM.CFunction fn, byte n)
    {
        if (n == byte.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "n must be less than 255");
        }
        
        GCHandle handle = GCHandle.Alloc(fn, GCHandleType.Normal);
        PushLightUserData(GCHandle.ToIntPtr(handle));
        PushCClosure(Natives.ManagedFunctionWrapper, n + 1);
    }

    public LuaRef Ref(int t)
    {
        unsafe
        {
            return Natives.LuaL_Ref(Handle, t);
        }
    }

    public void Unref(int t, LuaRef @ref)
    {
        unsafe
        {
            Natives.LuaL_Unref(Handle, t, @ref);
        }
    }
}