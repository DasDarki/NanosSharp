#ifndef NANOSSHARP_GAME_SYMBOLS_H_
#define NANOSSHARP_GAME_SYMBOLS_H_

#include <memory>
#include <string>

#include "game/package.h"
#include "game/scripting.h"

namespace nanossharp::game {

enum class LogType : int32_t {
  kInfo = 0,
  kWarning = 1,
  kError = 2,
  kDebug = 3,
  kVerbose = 4,
  kSuccess = 5,
};

using EngineInitializeFunction = void (*)(bool, void (*)(Engine&), void (*)(),
                                          void (*)());
using EngineReleaseFunction = void (*)();
using ServerTickFunction = void (*)(void*, float);
using EnqueueLogFunction = void (*)(LogType, std::string&&);
using PackageLoadFunction = void (*)(Package*);
using PackageUnloadFunction = void (*)(Package*, bool, bool);
using PackageReloadFunction = void (*)(Package*);

struct Symbols {
  std::unique_ptr<Engine>* engine_instance = nullptr;
  EngineInitializeFunction engine_initialize = nullptr;
  EngineReleaseFunction engine_release = nullptr;
  ServerTickFunction server_tick = nullptr;
  EnqueueLogFunction enqueue_log = nullptr;
  PackageLoadFunction package_load = nullptr;
  PackageUnloadFunction package_unload = nullptr;
  PackageReloadFunction package_reload = nullptr;
  std::string* packages_path = nullptr;

  bool Resolve(std::string& error);
};

Symbols& GetSymbols();

}

#endif
