#ifndef NANOSSHARP_PLATFORM_LINUX_LUA_SYMBOLS_H_
#define NANOSSHARP_PLATFORM_LINUX_LUA_SYMBOLS_H_

#include <cstddef>
#include <cstdint>
#include <string>
#include <unordered_map>

#include "game/script_value.h"

namespace nanossharp::game {

struct LuaState;

class LuaRef {
 public:
  ~LuaRef();

 private:
  alignas(8) unsigned char storage_[32];
};

using LuaPushVisited = std::unordered_map<void*, LuaRef>;
using LuaPopVisited = std::unordered_map<void*, ScriptValue>;
using LuaCFunction = int (*)(LuaState*);

inline constexpr int kLuaRegistryIndex = -1001000;

constexpr int LuaUpvalueIndex(int index) { return kLuaRegistryIndex - index; }

struct LuaSymbols {
  int (*getglobal)(LuaState*, const char*) = nullptr;
  void (*setglobal)(LuaState*, const char*) = nullptr;
  int (*gettop)(LuaState*) = nullptr;
  void (*settop)(LuaState*, int) = nullptr;
  void (*createtable)(LuaState*, int, int) = nullptr;
  void (*rawset)(LuaState*, int) = nullptr;
  void (*rawseti)(LuaState*, int, int64_t) = nullptr;
  void (*setfield)(LuaState*, int, const char*) = nullptr;
  int (*setmetatable)(LuaState*, int) = nullptr;
  int (*newmetatable)(LuaState*, const char*) = nullptr;
  void* (*newuserdatauv)(LuaState*, size_t, int) = nullptr;
  void* (*touserdata)(LuaState*, int) = nullptr;
  void (*pushcclosure)(LuaState*, LuaCFunction, int) = nullptr;
  void (*pushlightuserdata)(LuaState*, void*) = nullptr;
  const char* (*pushlstring)(LuaState*, const char*, size_t) = nullptr;
  const char* (*tolstring)(LuaState*, int, size_t*) = nullptr;
  int (*checkstack)(LuaState*, int) = nullptr;
  int (*error)(LuaState*) = nullptr;
  int (*pcallk)(LuaState*, int, int, int, intptr_t, void*) = nullptr;
  int (*loadbufferx)(LuaState*, const char*, size_t, const char*, const char*) = nullptr;
  const char* (*setupvalue)(LuaState*, int, int) = nullptr;
  void (*pop_value)(LuaState*, int, ScriptValue&, LuaPopVisited&, bool) = nullptr;
  void (*push_value)(LuaState*, const ScriptValue&, LuaPushVisited&) = nullptr;
  void (*ref_destroy)(LuaRef*) = nullptr;
  void (*ref_push)(LuaRef*, LuaState*) = nullptr;

  bool Resolve(std::string& error);
};

LuaSymbols& GetLuaSymbols();

}

#endif
