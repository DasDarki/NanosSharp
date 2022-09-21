﻿using System.Runtime.InteropServices;

namespace NanosSharp.API;

/// <summary>
/// The interface represents a native lua state through which one can interact with the luaVM of a nanos world package.
/// </summary>
public interface ILuaVM
{
    delegate int CFunction(ILuaVM state);
    
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
    int RawGet(int idx);
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
    void PushUserType(IntPtr p, int type);
    int NewMetaTable(string name);
    IntPtr ToUserType(int idx, int type);
    GCHandle PushManagedFunction(CFunction fn);
    GCHandle PushManagedClosure(CFunction fn, byte n);
}