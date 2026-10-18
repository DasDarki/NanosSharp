#include "platform/linux/lua_symbols.h"

#include "game/lua.h"
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

bool ResolveLuaSymbols(std::string& error) {
  LuaSymbols& symbols = GetLuaSymbols();
  return ResolveSymbol("lua_getglobal", symbols.getglobal, error) &&
         ResolveSymbol("lua_setglobal", symbols.setglobal, error) &&
         ResolveSymbol("lua_gettop", symbols.gettop, error) &&
         ResolveSymbol("lua_settop", symbols.settop, error) &&
         ResolveSymbol("lua_setfield", symbols.setfield, error) &&
         ResolveSymbol("lua_setmetatable", symbols.setmetatable, error) &&
         ResolveSymbol("luaL_newmetatable", symbols.newmetatable, error) &&
         ResolveSymbol("lua_newuserdatauv", symbols.newuserdatauv, error) &&
         ResolveSymbol("lua_touserdata", symbols.touserdata, error) &&
         ResolveSymbol("lua_pushcclosure", symbols.pushcclosure, error) &&
         ResolveSymbol("lua_pushlstring", symbols.pushlstring, error) &&
         ResolveSymbol("lua_tolstring", symbols.tolstring, error) &&
         ResolveSymbol("lua_error", symbols.error, error) &&
         ResolveSymbol("lua_pcallk", symbols.pcallk, error) &&
         ResolveSymbol("luaL_loadbufferx", symbols.loadbufferx, error) &&
         ResolveSymbol("lua_setupvalue", symbols.setupvalue, error) &&
         ResolveSymbol(kPopValueSymbol, symbols.pop_value, error) &&
         ResolveSymbol(kPushValueSymbol, symbols.push_value, error) &&
         ResolveSymbol(kRefDestroySymbol, symbols.ref_destroy, error) &&
         ResolveSymbol(kRefPushSymbol, symbols.ref_push, error);
}

}
