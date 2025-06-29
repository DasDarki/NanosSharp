#include "runtime.h"

#ifdef __cplusplus
#define EXTERN extern "C"
#else
#define EXTERN
#endif

#ifdef _WIN32
#define EXPORT EXTERN __declspec(dllexport)
#define IMPORT EXTERN __declspec(dllimport)
#else
#define EXPORT EXTERN __attribute__((visibility("default")))
#define IMPORT
#endif

EXTERN {
    #include "lua.h"
    #include "lauxlib.h"
}

int load_module_internally(lua_State *L) {
    const char *module_name = luaL_checkstring(L, 1);

    luaL_checktype(L, 2, LUA_TTABLE);                // Arg 2: _ENV

    // Get the env from the registry
    void* env = nullptr;

    // Lookup: registry["environments"][_ENV] → LuaEnvironment*
    lua_pushstring(L, "environments");
    lua_rawget(L, LUA_REGISTRYINDEX);     // [environments]
    lua_pushvalue(L, 2);                  // [_ENV]
    lua_rawget(L, -2);                    // [environments[_ENV]]

    if (!lua_isuserdata(L, -1)) {
        lua_pop(L, 2); // cleanup
        return luaL_error(L, "No LuaEnvironment associated with given _ENV");
    }

    env = lua_touserdata(L, -1);
    lua_pop(L, 2); // Pop env and environments


    Runtime::GetInstance().LoadModule(L, module_name, env);
    return 0;
}

EXTERN int luaopen_nanossharp_runtime (lua_State *L) {
    int result = Runtime::GetInstance().Start(L, "9.0.0", "NanosSharp", "NanosSharp.Runtime, NanosSharp");
    switch (result) {
        case RUNTIME_SUCCESS:
            break;
        case RUNTIME_ERROR_HOSTFXR_FAILED:
            return luaL_error(L, "Failed to load hostfxr.");
        case RUNTIME_ERROR_NO_RUNTIME_CONFIG:
            return luaL_error(L, "Failed to find NanosSharp runtime config.");
        case RUNTIME_ERROR_NO_RUNTIME_LIBRARY:
            return luaL_error(L, "Failed to find NanosSharp runtime library.");
        case RUNTIME_ERROR_ASSEMBLY_FAILED:
            return luaL_error(L, "Failed to load NanosSharp assembly.");
        case RUNTIME_ERROR_INIT_FAILED:
            return luaL_error(L, "Failed to initialize NanosSharp runtime.");
        default:
            return luaL_error(L, "Unknown error.");
    }

    struct luaL_Reg functions[] = {
            {"LoadModule", load_module_internally},
            {NULL, NULL},
    };

    luaL_newlib(L, functions);

    Runtime::GetInstance().LoadModule(L, "@autoload", nullptr);

    return 1;
}

// --------------------------- NanosSharp Exports --------------------------- \\

// region lua.h

EXPORT int Lua_AbsIndex(lua_State *L, int idx) {
    return lua_absindex(L, idx);
}

EXPORT int Lua_GetTop(lua_State *L) {
    return lua_gettop(L);
}

EXPORT void Lua_SetTop(lua_State *L, int idx) {
    lua_settop(L, idx);
}

EXPORT void Lua_PushValue(lua_State *L, int idx) {
    lua_pushvalue(L, idx);
}

EXPORT void Lua_Rotate(lua_State *L, int idx, int n) {
    lua_rotate(L, idx, n);
}

EXPORT void Lua_Copy(lua_State *L, int fromIdx, int toIdx) {
    lua_copy(L, fromIdx, toIdx);
}

EXPORT int Lua_CheckStack(lua_State *L, int n) {
    return lua_checkstack(L, n);
}

EXPORT void Lua_XMove(lua_State *from, lua_State *to, int n) {
    lua_xmove(from, to, n);
}

EXPORT int Lua_IsNumber(lua_State *L, int idx) {
    return lua_isnumber(L, idx);
}

EXPORT int Lua_IsString(lua_State *L, int idx) {
    return lua_isstring(L, idx);
}

EXPORT int Lua_IsCFunction(lua_State *L, int idx) {
    return lua_iscfunction(L, idx);
}

EXPORT int Lua_IsInteger(lua_State *L, int idx) {
    return lua_isinteger(L, idx);
}

EXPORT int Lua_IsUserData(lua_State *L, int idx) {
    return lua_isuserdata(L, idx);
}

EXPORT int Lua_Type(lua_State *L, int idx) {
    return lua_type(L, idx);
}

EXPORT const char *Lua_TypeName(lua_State *L, int tp, unsigned int *outLen) {
    auto str = lua_typename(L, tp);
    *outLen = (unsigned int) strlen(str);
    return str;
}

EXPORT double Lua_ToNumberX(lua_State *L, int idx, int *isNum) {
    return lua_tonumberx(L, idx, isNum);
}

EXPORT long long Lua_ToIntegerX(lua_State *L, int idx, int *isNum) {
    return lua_tointegerx(L, idx, isNum);
}

EXPORT int Lua_ToBoolean(lua_State *L, int idx) {
    return lua_toboolean(L, idx);
}

EXPORT const char *Lua_ToLString(lua_State *L, int idx, unsigned int *outLen) {
    size_t len;
    auto str = lua_tolstring(L, idx, &len);
    *outLen = (unsigned int) len;
    return str;
}

EXPORT unsigned long long Lua_RawLen(lua_State *L, int idx) {
    return lua_rawlen(L, idx);
}

EXPORT lua_CFunction Lua_ToCFunction(lua_State *L, int idx) {
    return lua_tocfunction(L, idx);
}

EXPORT void *Lua_ToUserData(lua_State *L, int idx) {
    return lua_touserdata(L, idx);
}

EXPORT const void *Lua_ToPointer(lua_State *L, int idx) {
    return lua_topointer(L, idx);
}

EXPORT void Lua_Arith(lua_State *L, int op) {
    lua_arith(L, op);
}

EXPORT int Lua_RawEqual(lua_State *L, int idx1, int idx2) {
    return lua_rawequal(L, idx1, idx2);
}

EXPORT int Lua_Compare(lua_State *L, int idx1, int idx2, int op) {
    return lua_compare(L, idx1, idx2, op);
}

EXPORT void Lua_PushNil(lua_State *L) {
    lua_pushnil(L);
}

EXPORT void Lua_PushNumber(lua_State *L, double n) {
    lua_pushnumber(L, n);
}

EXPORT void Lua_PushInteger(lua_State *L, long long n) {
    lua_pushinteger(L, n);
}

EXPORT void Lua_PushLString(lua_State *L, const char *s, unsigned int len) {
    lua_pushlstring(L, s, len);
}

EXPORT void Lua_PushString(lua_State *L, const char *s) {
    lua_pushstring(L, s);
}

EXPORT void Lua_PushCClosure(lua_State *L, lua_CFunction fn, int n) {
    lua_pushcclosure(L, fn, n);
}

EXPORT void Lua_PushBoolean(lua_State *L, int b) {
    lua_pushboolean(L, b);
}

EXPORT void Lua_PushLightUserData(lua_State *L, void *p) {
    lua_pushlightuserdata(L, p);
}

EXPORT int Lua_GetGlobal(lua_State *L, const char *name) {
    return lua_getglobal(L, name);
}

EXPORT int Lua_GetTable(lua_State *L, int idx) {
    return lua_gettable(L, idx);
}

EXPORT int Lua_GetField(lua_State *L, int idx, const char *k) {
    return lua_getfield(L, idx, k);
}

EXPORT int Lua_GetI(lua_State *L, int idx, long long n) {
    return lua_geti(L, idx, n);
}

EXPORT int Lua_RawGet(lua_State *L, int idx) {
    return lua_rawget(L, idx);
}

EXPORT int Lua_RawGetI(lua_State *L, int idx, long long n) {
    return lua_rawgeti(L, idx, n);
}

EXPORT void Lua_RawSetI(lua_State *L, int idx, long long n) {
    lua_rawseti(L, idx, n);
}

EXPORT int Lua_RawGetP(lua_State *L, int idx, void *p) {
    return lua_rawgetp(L, idx, p);
}

EXPORT void Lua_CreateTable(lua_State *L, int narr, int nrec) {
    lua_createtable(L, narr, nrec);
}

EXPORT void *Lua_NewUserData(lua_State *L, unsigned int sz) {
    return lua_newuserdata(L, sz);
}

EXPORT int Lua_GetMetaTable(lua_State *L, int objIndex) {
    return lua_getmetatable(L, objIndex);
}

EXPORT void Lua_SetGlobal(lua_State *L, const char *name) {
    lua_setglobal(L, name);
}

EXPORT void Lua_SetTable(lua_State *L, int idx) {
    lua_settable(L, idx);
}

EXPORT void Lua_SetField(lua_State *L, int idx, const char *k) {
    lua_setfield(L, idx, k);
}

EXPORT void Lua_RawSet(lua_State *L, int idx) {
    lua_rawset(L, idx);
}

EXPORT int Lua_SetMetaTable(lua_State *L, int objIndex) {
    return lua_setmetatable(L, objIndex);
}

EXPORT void Lua_Call(lua_State *L, int nargs, int nresults) {
    lua_call(L, nargs, nresults);
}

EXPORT int Lua_PCall(lua_State *L, int nargs, int nresults, int errfunc) {
    return lua_pcall(L, nargs, nresults, errfunc);
}

EXPORT int Lua_Yield(lua_State *L, int nresults) {
    return lua_yield(L, nresults);
}

EXPORT double Lua_ToNumber(lua_State *L, int idx) {
    return lua_tonumber(L, idx);
}

EXPORT long long Lua_ToInteger(lua_State *L, int idx) {
    return lua_tointeger(L, idx);
}

EXPORT void Lua_Pop(lua_State *L, int n) {
    lua_pop(L, n);
}

EXPORT void Lua_NewTable(lua_State *L) {
    lua_newtable(L);
}

EXPORT void Lua_PushCFunction(lua_State *L, lua_CFunction fn) {
    lua_pushcfunction(L, fn);
}

EXPORT int Lua_IsFunction(lua_State *L, int idx) {
    return lua_isfunction(L, idx);
}

EXPORT int Lua_IsTable(lua_State *L, int idx) {
    return lua_istable(L, idx);
}

EXPORT int Lua_IsLightUserData(lua_State *L, int idx) {
    return lua_islightuserdata(L, idx);
}

EXPORT int Lua_IsNil(lua_State *L, int idx) {
    return lua_isnil(L, idx);
}

EXPORT int Lua_IsBoolean(lua_State *L, int idx) {
    return lua_isboolean(L, idx);
}

EXPORT int Lua_IsNone(lua_State *L, int idx) {
    return lua_isnone(L, idx);
}

EXPORT void Lua_PushGlobalTable(lua_State *L) {
    lua_pushglobaltable(L);
}

EXPORT void Lua_GetUserValue(lua_State *L, int idx) {
    lua_getuservalue(L, idx);
}

EXPORT void Lua_SetUserValue(lua_State *L, int idx) {
    lua_setuservalue(L, idx);
}

EXPORT void Lua_Insert(lua_State *L, int idx) {
    lua_insert(L, idx);
}

EXPORT void Lua_Remove(lua_State *L, int idx) {
    lua_remove(L, idx);
}

EXPORT void Lua_Replace(lua_State *L, int idx) {
    lua_replace(L, idx);
}

EXPORT int Lua_NewMetaTable(lua_State *L, const char *name) {
    return luaL_newmetatable(L, name);
}

EXPORT int Lua_Next(lua_State *L, int idx) {
    return lua_next(L, idx);
}

EXPORT void Lua_Concat(lua_State *L, int n) {
    lua_concat(L, n);
}

EXPORT void Lua_Len(lua_State *L, int idx) {
    lua_len(L, idx);
}

EXPORT int ManagedFunctionWrapper(lua_State *L) {
    void *gc_handle = lua_touserdata(L, lua_upvalueindex(1));
    return call_managed_delegate(L, gc_handle);
}

// endregion

// region luaxlib.h
EXPORT int LuaL_Ref(lua_State *L, int t) {
    return luaL_ref(L, t);
}

EXPORT void LuaL_Unref(lua_State *L, int t, int ref) {
    luaL_unref(L, t, ref);
}

// endregion