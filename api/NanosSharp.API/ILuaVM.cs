﻿namespace NanosSharp.API;

/// <summary>
/// The interface represents a native lua state through which one can interact with the luaVM of a nanos world package.
/// </summary>
public interface ILuaVM
{
    const int MaxStack = 1000000;
    const int RegistryIndex = -MaxStack - 1000;
    
    delegate int CFunction(ILuaVM state);

    /// <summary>
    /// Injects the lua scripting environment needed for nanos world to idenitfy the luaVM as a valid nanos world package
    /// into the current lua state.
    /// </summary>
    void InjectEnvironment();
    
    int AbsIndex(int idx);
    int GetTop();
    void SetTop(int idx);
    void PushValue(int idx);
    void Rotate(int idx, int n);
    void Copy(int fromIdx, int toIdx);
    int CheckStack(int n);
    void XMove(ILuaVM to, int n);
    bool IsNumber(int idx);
    bool IsString(int idx);
    bool IsCFunction(int idx);
    bool IsInteger(int idx);
    bool IsUserData(int idx);
    LuaType Type(int idx);
    string TypeName(LuaType tp);
    double ToNumberX(int idx, out bool isNum);
    long ToIntegerX(int idx, out bool isNum);
    bool ToBoolean(int idx);
    ulong RawLen(int idx);
    CFunction? ToCFunction(int idx);
    IntPtr ToUserData(int idx);
    IntPtr ToPointer(int idx);
    void Arith(LuaOperator op);
    bool RawEqual(int idx1, int idx2);
    bool Compare(int idx1, int idx2, LuaComparator op); 
    void PushNil();
    void PushNumber(double n);
    void PushInteger(int n);
    void PushLString(string s, uint len);
    void PushString(string s);
    void PushCClosure(IntPtr fnPtr, int n);
    void PushBoolean(bool b);
    void PushLightUserData(IntPtr p);
    int GetGlobal(string name);
    int GetTable(int idx);
    int GetField(int idx, string k);
    int GetI(int idx, long n);
    int RawGet(int idx);
    int RawGetI(int idx, long n);
    void RawSetI(int idx, long n);
    int RawGetP(int idx, IntPtr p);
    void CreateTable(int narr, int nrec);
    IntPtr NewUserData(uint sz);
    int GetMetaTable(int objIndex);
    void SetGlobal(string name);
    void SetTable(int idx);
    void SetField(int idx, string k);
    void RawSet(int idx);
    int SetMetaTable(int objIndex);
    void Call(int nargs, int nresults);
    int PCall(int nargs, int nresults, int errfunc);
    int Yield(int nresults);
    double ToNumber(int idx);
    long ToInteger(int idx);
    void Pop(int n = 1);
    void NewTable();
    unsafe void PushCFunction(delegate* unmanaged[Cdecl]<IntPtr, int> fnPtr);
    bool IsFunction(int idx);
    bool IsTable(int idx);
    bool IsLightUserData(int idx);
    bool IsNil(int idx);
    bool IsBoolean(int idx);
    bool IsNone(int idx);
    void PushGlobalTable();
    void GetUserValue(int idx);
    void SetUserValue(int idx);
    string ToString(int idx);
    void MCall(int args, int results);
    void Insert(int idx);
    void Remove(int idx);
    void Replace(int idx);
    int NewMetaTable(string name);
    int Next(int idx);
    void Concat(int n);
    void Len(int idx);
    void PushManagedFunction(CFunction fn);
    void PushManagedClosure(CFunction fn, byte n);
    LuaRef Ref(int t);
    void Unref(int t, LuaRef @ref);
    void PushObject(object? o);
    void PushArray(Array arr);
    void PushEnum(Enum e);
    void PushTable(Dictionary<string, object?> table);
    object? ToObject(int idx);
    object?[] ToArray(int idx);
    T?[] ToArray<T>(int idx);
    T ToEnum<T>(int idx) where T : Enum;
    LuaRef[] ToRefArray(int idx);
    Dictionary<string, object?> ToTable(int idx);
}