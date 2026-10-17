#ifndef NANOSSHARP_PLATFORM_WINDOWS_RUNTIME_H_
#define NANOSSHARP_PLATFORM_WINDOWS_RUNTIME_H_

#include <windows.h>

#include <cstdint>
#include <cstdio>
#include <filesystem>
#include <memory>
#include <mutex>
#include <string>

#include "game/log_type.h"
#include "game/scripting.h"
#include "host/dotnet_host.h"

namespace nanossharp {

class Runtime {
 public:
  static Runtime& Instance();

  void Load(HMODULE module);
  void Unload();
  void Log(game::LogType type, const std::string& message);

  game::Engine* engine() const { return engine_; }
  const std::filesystem::path& base_directory() const { return base_directory_; }

 private:
  Runtime() = default;

  static DWORD WINAPI InstallHooks(void* parameter);
  static void OnEntityDestroyed(game::Engine* engine, const game::Entity& entity, uint64_t class_id);
  static void OnEntityCreated(game::Engine* engine, const game::Entity& entity,
                              const std::shared_ptr<game::IScriptEntityRef>& ref, uint64_t class_id);
  static void* OnEngineDestroyed(game::Engine* engine, uint32_t flags);

  bool IsGameProcess() const;
  void PatchSlot(void** vtable, int slot, void* detour, void** original);
  void AttachEngine(game::Engine* engine);
  void DetachEngine();
  bool StartManaged(std::string& error);

  using EntityDestroyedFunction = decltype(&OnEntityDestroyed);
  using EntityCreatedFunction = decltype(&OnEntityCreated);
  using EngineDestroyedFunction = decltype(&OnEngineDestroyed);

  std::filesystem::path base_directory_;
  std::unique_ptr<host::DotnetHost> host_;
  std::mutex log_mutex_;
  FILE* log_file_ = nullptr;
  EntityDestroyedFunction original_entity_destroyed_ = nullptr;
  EntityCreatedFunction original_entity_created_ = nullptr;
  EngineDestroyedFunction original_engine_destroyed_ = nullptr;
  game::Engine* engine_ = nullptr;
  DWORD main_thread_ = 0;
  bool disabled_ = false;
  bool managed_started_ = false;
  bool managed_failed_ = false;
};

}

#endif
