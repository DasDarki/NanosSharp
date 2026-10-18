#include "platform/windows/lua_symbols.h"

#include <atomic>

#include "bridge/lua_interop.h"
#include "game/lua.h"
#include "platform/windows/inline_hook.h"
#include "platform/windows/rtti.h"
#include "platform/windows/signature.h"

namespace nanossharp::platform::lua {

namespace {

constexpr char kLuaEnvironmentType[] = ".?AVLuaEnvironment@lua@scripting@nw@@";
constexpr size_t kCallLength = 5;
constexpr size_t kPushValuePrologue = 15;

struct Signature {
  const char* name;
  void** target;
  const char* pattern;
  bool is_call;
};

std::atomic<bool> resolved = false;
std::string failure = "the Lua functions were not resolved yet";
std::ptrdiff_t environment_table_offset = 0;

template <typename T>
void** Slot(T& function) {
  return reinterpret_cast<void**>(&function);
}

}

bool Resolve(HMODULE module, std::string& error) {
  game::LuaSymbols& lua = game::GetLuaSymbols();
  const Signature signatures[] = {
      {"lua_gettop", Slot(lua.gettop), "48 8B 41 20 48 8B 51 10 48 2B 10", false},
      {"lua_settop", Slot(lua.settop), "40 53 48 83 EC 20 48 63 C2 48 8B D9 48 FF C0", false},
      {"lua_setmetatable", Slot(lua.setmetatable),
       "48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 48 8B F9 E8 ?? ?? ?? ?? 48 8B 59 10", false},
      {"luaL_newmetatable", Slot(lua.newmetatable),
       "48 89 5C 24 08 57 48 83 EC 20 48 8B FA 4C 8B C2 BA D8 B9 F0 FF", false},
      {"lua_newuserdatauv", Slot(lua.newuserdatauv),
       "48 89 5C 24 08 57 48 83 EC 20 48 8B F9 E8 ?? ?? ?? ?? 48 8B 57 10", false},
      {"lua_pushcclosure", Slot(lua.pushcclosure),
       "48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 49 63 F8 48 8B F2 48 8B D9 45 85 C0", false},
      {"lua_pushlstring", Slot(lua.pushlstring), "48 89 5C 24 08 57 48 83 EC 20 48 8B D9 4D 85 C0", false},
      {"lua_tolstring", Slot(lua.tolstring), "48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 49 8B D8 8B F2", false},
      {"lua_error", Slot(lua.error), "48 83 EC 28 4C 8B 41 10 41 80 78 F8 44", false},
      {"lua_pcallk", Slot(lua.pcallk), "48 89 74 24 18 57 48 83 EC 40 33 F6 48 89 6C 24 58", false},
      {"luaL_loadbufferx", Slot(lua.loadbufferx),
       "48 83 EC 48 48 8B 44 24 70 48 89 54 24 30 48 8D 15 ?? ?? ?? ??", false},
      {"lua_setupvalue", Slot(lua.setupvalue), "48 89 5C 24 08 57 48 83 EC 30 33 C0 45 8B D0", false},
      {"PushValue", Slot(lua.push_value),
       "48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 40 48 8B DA 49 8B F0 BA 14 00 00 00", false},
      {"PopValue", Slot(lua.pop_value),
       "48 8B C4 55 53 56 57 41 54 41 55 41 56 41 57 48 8D A8 E8 FE FF FF 48 81 EC D8 01 00 00 0F 29 70 A8 0F 29 "
       "78 98 44 0F 29 40 88 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 85 90 00 00 00",
       false},
      {"LuaRef::~LuaRef", Slot(lua.ref_destroy),
       "40 53 48 83 EC 40 48 8B D9 48 8D 05 ?? ?? ?? ?? 48 89 01 48 8B 51 08", false},
      {"LuaRef::PushToLuaStack", Slot(lua.ref_push), "48 89 5C 24 08 57 48 83 EC 40 48 8B D9 41 B8 01 00 00 00",
       false},
      {"lua_setfield", Slot(lua.setfield),
       "48 89 5C 24 08 57 48 83 EC 20 48 8B FA 4C 8B C2 BA D8 B9 F0 FF 48 8B D9 E8 ?? ?? ?? ?? 85 C0 74 ?? 33 C0 48 "
       "8B 5C 24 30 48 83 C4 20 5F C3 BA FE FF FF FF 48 8B CB E8 ?? ?? ?? ?? 33 D2 41 B8 02 00 00 00 48 8B CB E8 ?? "
       "?? ?? ?? 48 8B D7 48 8B CB E8 ?? ?? ?? ?? 4C 8D 05 ?? ?? ?? ?? BA FE FF FF FF 48 8B CB E8 ?? ?? ?? ??",
       true},
      {"lua_setglobal", Slot(lua.setglobal),
       "48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 30 49 8B E8 48 8B FA 4C 8D 05 ?? ?? ?? ?? BA D8 B9 "
       "F0 FF 41 8B F1 48 8B D9 E8 ?? ?? ?? ?? 4C 8B C7 BA FF FF FF FF 48 8B CB E8 ?? ?? ?? ?? BA FF FF FF FF 48 8B "
       "CB E8 ?? ?? ?? ?? 85 C0 75 ?? BA FE FF FF FF 48 8B CB E8 ?? ?? ?? ?? 45 33 C0 48 8B D5 48 8B CB E8 ?? ?? ?? "
       "?? 48 8B D7 48 8B CB E8 ?? ?? ?? ?? BA 01 00 00 00 48 C7 44 24 20 00 00 00 00 44 8B C2 45 33 C9 48 8B CB E8 "
       "?? ?? ?? ?? BA FF FF FF FF 48 8B CB E8 ?? ?? ?? ?? 4C 8B C7 BA FD FF FF FF 48 8B CB E8 ?? ?? ?? ?? BA FE FF "
       "FF FF 41 B8 FF FF FF FF 48 8B CB E8 ?? ?? ?? ?? BA FE FF FF FF 48 8B CB E8 ?? ?? ?? ?? 85 F6 74 ?? BA FF FF "
       "FF FF 48 8B CB E8 ?? ?? ?? ?? 48 8B D7 48 8B CB E8 ?? ?? ?? ??",
       true},
      {"lua_getglobal", Slot(lua.getglobal),
       "48 89 5C 24 20 55 56 57 41 56 41 57 48 8D 6C 24 C9 48 81 EC D0 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 "
       "45 2F 4D 8B F0 48 8B DA 48 8B F9 45 33 C0 48 8B 91 88 00 00 00 48 8D 4D DF E8 ?? ?? ?? ?? 90 48 83 7B 18 0F "
       "76 ?? 48 8B 1B 48 8B D3 48 8B 8F 88 00 00 00 E8 ?? ?? ?? ??",
       true},
      {"lua_touserdata", Slot(lua.touserdata),
       "48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 20 49 8B E8 8B DA 48 8B F9 E8 ?? ?? ?? ??", true},
  };

  for (const Signature& signature : signatures) {
    size_t length = 0;
    uint8_t* match = signature::FindUnique(module, signature.pattern, length);
    uint8_t* address = signature.is_call && match != nullptr ? signature::CallTarget(match + length - kCallLength)
                                                             : match;
    if (address == nullptr) {
      failure = std::string("the function ") + signature.name + " was not found in this game version";
      error = failure;
      return false;
    }
    *signature.target = address;
  }

  uint32_t offset = 0;
  if (!rtti::FindBaseOffset(module, kLuaEnvironmentType, offset)) {
    failure = "the Lua environment table was not found in this game version";
    error = failure;
    return false;
  }
  environment_table_offset = offset;

  void* trampoline = inline_hook::Install(reinterpret_cast<void*>(lua.push_value),
                                          reinterpret_cast<void*>(&bridge::PushValueDetour), kPushValuePrologue);
  if (trampoline == nullptr) {
    failure = "the PushValue hook could not be installed";
    error = failure;
    return false;
  }
  lua.push_value = reinterpret_cast<decltype(lua.push_value)>(trampoline);
  resolved = true;
  return true;
}

bool IsResolved(std::string& reason) {
  if (!resolved) {
    reason = failure;
  }
  return resolved;
}

std::ptrdiff_t EnvironmentTableOffset() { return environment_table_offset; }

}
