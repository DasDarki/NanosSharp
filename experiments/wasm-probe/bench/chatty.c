#include <lua.h>
#include <lauxlib.h>

static int l_nop(lua_State *L)
{
    lua_Integer value = luaL_checkinteger(L, 1);
    lua_pushinteger(L, value + 1);
    return 1;
}

int luaopen_probe(lua_State *L)
{
    static const luaL_Reg functions[] = {{"nop", l_nop}, {NULL, NULL}};
    luaL_newlib(L, functions);
    return 1;
}
