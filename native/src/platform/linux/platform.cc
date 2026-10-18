#include "platform/platform.h"

#include <cxxabi.h>
#include <dlfcn.h>
#include <nethost.h>

#include <array>
#include <cstdlib>
#include <cstring>
#include <stdexcept>
#include <typeinfo>

#include "platform/linux/isolated_libraries.h"
#include "platform/linux/package.h"
#include "platform/linux/runtime.h"
#include "platform/linux/symbols.h"
#include "platform/linux/validation.h"

namespace nanossharp::platform {

namespace {

constexpr std::ptrdiff_t kEngineLuaStateOffset = 0xa0;
constexpr std::ptrdiff_t kEnvironmentTableOffset = 0xc8;
constexpr char kLuaFunctionTypeName[] = "N2nw9scripting3lua14LuaFunctionRefE";

char* CopyString(const char* value) {
  size_t length = std::strlen(value);
  char* copy = static_cast<char*>(std::malloc(length + 1));
  std::memcpy(copy, value, length + 1);
  return copy;
}

game::Engine& RequireEngine() {
  game::Engine* engine = AttachedEngine();
  if (engine == nullptr) {
    throw std::runtime_error("the scripting engine is not attached");
  }
  return *engine;
}

}

bool LocateHostfxr(const std::filesystem::path&, HostfxrLocation& location, std::string& error) {
  std::array<char_t, 4096> path{};
  size_t size = path.size();
  int result = get_hostfxr_path(path.data(), &size, nullptr);
  if (result != 0) {
    error = "unable to locate hostfxr, is the .NET runtime installed?";
    return false;
  }
  location.library = path.data();
  return true;
}

void* OpenLibrary(const std::filesystem::path& path, std::string& error) {
  void* library = dlopen(path.c_str(), RTLD_NOW | RTLD_LOCAL);
  if (library == nullptr) {
    error = dlerror();
  }
  return library;
}

void* FindExport(void* library, const char* name) { return dlsym(library, name); }

game::Engine* AttachedEngine() { return Runtime::Instance().engine(); }

void Log(game::LogType type, const std::string& message) { Runtime::Instance().Log(type, message); }

char* DescribeType(const void* polymorphic_object) {
  auto vtable = *static_cast<const std::type_info* const* const*>(polymorphic_object);
  const std::type_info* type = vtable[-1];
  int status = 0;
  char* demangled = abi::__cxa_demangle(type->name(), nullptr, nullptr, &status);
  if (status != 0 || demangled == nullptr) {
    return CopyString(type->name());
  }
  return demangled;
}

void* LoadNativeLibrary(const char* path, std::string& error) {
  return host::LoadIsolatedLibrary(path, error);
}

bool ResolveDeferredEntity(uint32_t network_id, game::ScriptValue& value) {
  game::EntityFetcher* fetcher = game::GetSymbols().entity_fetcher;
  if (fetcher == nullptr || !*fetcher) {
    return false;
  }
  value = (*fetcher)(network_id);
  return true;
}

bool IsLuaInteropAvailable(std::string&) { return true; }

game::LuaState* GetLuaState() {
  auto* state = *reinterpret_cast<game::LuaState**>(reinterpret_cast<char*>(&RequireEngine()) +
                                                    kEngineLuaStateOffset);
  if (state == nullptr) {
    throw std::runtime_error("the Lua state is not available");
  }
  return state;
}

game::LuaRef* GetLuaEnvironmentTable(game::Environment& environment) {
  if (!game::IsLuaEnvironment(&environment)) {
    return nullptr;
  }
  return reinterpret_cast<game::LuaRef*>(reinterpret_cast<char*>(&environment) + kEnvironmentTableOffset);
}

bool IsLuaFunction(const game::IScriptFunctionRef* function) {
  const char* name = game::GetTypeName(function);
  return name != nullptr && std::strcmp(name, kLuaFunctionTypeName) == 0;
}

void ReloadPackage(game::Package* package) { game::GetSymbols().package_reload(package); }

void* GetPackageEntity(game::Package* package) { return package->scripting_entity(); }

}
