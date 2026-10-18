#ifndef NANOSSHARP_PLATFORM_LINUX_SYMBOLS_H_
#define NANOSSHARP_PLATFORM_LINUX_SYMBOLS_H_

#include <dlfcn.h>

#include <functional>
#include <memory>
#include <string>

#include "game/log_type.h"
#include "platform/linux/package.h"
#include "game/scripting.h"

namespace nanossharp::game {


using EntityFetcher = std::function<ScriptValue(uint32_t)>;
using EngineInitializeFunction = void (*)(bool, void (*)(Engine&), void (*)(),
                                          void (*)());
using EngineReleaseFunction = void (*)();
using ServerTickFunction = void (*)(void*, float);
using EnqueueLogFunction = void (*)(LogType, std::string&&);
using PackageLoadFunction = void (*)(Package*);
using PackageUnloadFunction = void (*)(Package*, bool, bool);
using PackageReloadFunction = void (*)(Package*);
using PackageStoreClientFilesFunction = void (*)(Package*);

struct Symbols {
  std::unique_ptr<Engine>* engine_instance = nullptr;
  EngineInitializeFunction engine_initialize = nullptr;
  EngineReleaseFunction engine_release = nullptr;
  ServerTickFunction server_tick = nullptr;
  EnqueueLogFunction enqueue_log = nullptr;
  PackageLoadFunction package_load = nullptr;
  PackageUnloadFunction package_unload = nullptr;
  PackageReloadFunction package_reload = nullptr;
  PackageStoreClientFilesFunction package_store_client_files = nullptr;
  std::string* packages_path = nullptr;
  EntityFetcher* entity_fetcher = nullptr;

  bool Resolve(std::string& error);
};

Symbols& GetSymbols();

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

#endif
