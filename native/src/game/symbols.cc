#include "game/symbols.h"

#include <dlfcn.h>

namespace nanossharp::game {

namespace {

constexpr char kEngineInstanceSymbol[] =
    "_ZN2nw4core24ScriptingEngineSingleton11InstanceLuaE";
constexpr char kEngineInitializeSymbol[] =
    "_ZN2nw4core24ScriptingEngineSingleton10InitializeEbPFvRNS_9scripting6EngineEEPFvvES8_";
constexpr char kEngineReleaseSymbol[] =
    "_ZN2nw4core24ScriptingEngineSingleton7ReleaseEv";
constexpr char kServerTickSymbol[] = "_ZN2nw4core10CoreServer4TickEf";
constexpr char kEnqueueLogSymbol[] =
    "_ZN2nw4core3Log10EnqueueLogENS1_7LogTypeEOSs";
constexpr char kPackageLoadSymbol[] = "_ZN2nw4core7Package4LoadEv";
constexpr char kPackageUnloadSymbol[] = "_ZN2nw4core7Package6UnloadEbb";
constexpr char kPackageReloadSymbol[] = "_ZN2nw4core7Package6ReloadEv";
constexpr char kPackagesPathSymbol[] = "_ZN2nw4core7Package12PackagesPathE";

template <typename T>
bool ResolveSymbol(const char* name, T& target, std::string& error) {
  target = reinterpret_cast<T>(dlsym(RTLD_DEFAULT, name));
  if (target == nullptr) {
    error = std::string("unable to resolve symbol ") + name;
    return false;
  }
  return true;
}

}

bool Symbols::Resolve(std::string& error) {
  return ResolveSymbol(kEngineInstanceSymbol, engine_instance, error) &&
         ResolveSymbol(kEngineInitializeSymbol, engine_initialize, error) &&
         ResolveSymbol(kEngineReleaseSymbol, engine_release, error) &&
         ResolveSymbol(kServerTickSymbol, server_tick, error) &&
         ResolveSymbol(kEnqueueLogSymbol, enqueue_log, error) &&
         ResolveSymbol(kPackageLoadSymbol, package_load, error) &&
         ResolveSymbol(kPackageUnloadSymbol, package_unload, error) &&
         ResolveSymbol(kPackageReloadSymbol, package_reload, error) &&
         ResolveSymbol(kPackagesPathSymbol, packages_path, error);
}

Symbols& GetSymbols() {
  static Symbols symbols;
  return symbols;
}

}
