#ifndef NANOSSHARP_GAME_LUA_H_
#define NANOSSHARP_GAME_LUA_H_

#include <string>

#include "game/lua_symbols.h"
#include "game/scripting.h"

namespace nanossharp::game {

void PushValueDetour(LuaState* state, const ScriptValue& value, LuaPushVisited& visited);
ScriptValue GetLuaGlobal(Engine& engine, Environment& environment, const std::string& name);
void SetLuaGlobal(Engine& engine, Environment& environment, const std::string& name,
                  const ScriptValue& value);

}

#endif
