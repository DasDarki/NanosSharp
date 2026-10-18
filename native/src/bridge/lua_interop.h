#ifndef NANOSSHARP_BRIDGE_LUA_INTEROP_H_
#define NANOSSHARP_BRIDGE_LUA_INTEROP_H_

#include <string>

#include "game/lua.h"
#include "game/scripting.h"

namespace nanossharp::bridge {

void PushValueDetour(game::LuaState* state, const game::ScriptValue& value, game::LuaPushVisited& visited);
game::ScriptValue GetLuaGlobal(game::Environment& environment, const std::string& name);
void SetLuaGlobal(game::Environment& environment, const std::string& name, const game::ScriptValue& value);

}

#endif
