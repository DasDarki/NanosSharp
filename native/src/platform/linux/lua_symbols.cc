#include "platform/linux/lua_symbols.h"

#include "platform/linux/symbols.h"

namespace nanossharp::game {

namespace {

constexpr char kPopValueSymbol[] =
    "_ZN2nw9scripting3lua8PopValueEP9lua_StateiRNS0_11ScriptValueERSt13unordered_mapIPvS4_"
    "St4hashIS7_ESt8equal_toIS7_ESaISt4pairIKS7_S4_EEEb";
constexpr char kPushValueSymbol[] =
    "_ZN2nw9scripting3lua9PushValueEP9lua_StateRKNS0_11ScriptValueERSt13unordered_mapIPvNS1_"
    "6LuaRefESt4hashIS8_ESt8equal_toIS8_ESaISt4pairIKS8_S9_EEE";
constexpr char kRefDestroySymbol[] = "_ZN2nw9scripting3lua6LuaRefD1Ev";
constexpr char kRefPushSymbol[] = "_ZN2nw9scripting3lua6LuaRef14PushToLuaStackEP9lua_State";

}

LuaRef::~LuaRef() { GetLuaSymbols().ref_destroy(this); }

bool LuaSymbols::Resolve(std::string& error) {
  return ResolveSymbol("lua_getglobal", getglobal, error) &&
         ResolveSymbol("lua_setglobal", setglobal, error) &&
         ResolveSymbol("lua_gettop", gettop, error) &&
         ResolveSymbol("lua_settop", settop, error) &&
         ResolveSymbol("lua_createtable", createtable, error) &&
         ResolveSymbol("lua_rawset", rawset, error) &&
         ResolveSymbol("lua_rawseti", rawseti, error) &&
         ResolveSymbol("lua_setfield", setfield, error) &&
         ResolveSymbol("lua_setmetatable", setmetatable, error) &&
         ResolveSymbol("luaL_newmetatable", newmetatable, error) &&
         ResolveSymbol("lua_newuserdatauv", newuserdatauv, error) &&
         ResolveSymbol("lua_touserdata", touserdata, error) &&
         ResolveSymbol("lua_pushcclosure", pushcclosure, error) &&
         ResolveSymbol("lua_pushlightuserdata", pushlightuserdata, error) &&
         ResolveSymbol("lua_pushlstring", pushlstring, error) &&
         ResolveSymbol("lua_tolstring", tolstring, error) &&
         ResolveSymbol("lua_checkstack", checkstack, error) &&
         ResolveSymbol("lua_error", this->error, error) &&
         ResolveSymbol("lua_pcallk", pcallk, error) &&
         ResolveSymbol("luaL_loadbufferx", loadbufferx, error) &&
         ResolveSymbol("lua_setupvalue", setupvalue, error) &&
         ResolveSymbol(kPopValueSymbol, pop_value, error) &&
         ResolveSymbol(kPushValueSymbol, push_value, error) &&
         ResolveSymbol(kRefDestroySymbol, ref_destroy, error) &&
         ResolveSymbol(kRefPushSymbol, ref_push, error);
}

LuaSymbols& GetLuaSymbols() {
  static LuaSymbols symbols;
  return symbols;
}

}
