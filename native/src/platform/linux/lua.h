#ifndef NANOSSHARP_PLATFORM_LINUX_LUA_H_
#define NANOSSHARP_PLATFORM_LINUX_LUA_H_

#include <string>

#include "platform/linux/lua_symbols.h"
#include "game/scripting.h"

namespace nanossharp::game {

void PushValueDetour(LuaState* state, const ScriptValue& value, LuaPushVisited& visited);
ScriptValue GetLuaGlobal(Engine& engine, Environment& environment, const std::string& name);
void SetLuaGlobal(Engine& engine, Environment& environment, const std::string& name,
                  const ScriptValue& value);

}

#endif
