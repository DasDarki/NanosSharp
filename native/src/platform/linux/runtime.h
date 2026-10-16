#ifndef NANOSSHARP_PLATFORM_LINUX_RUNTIME_H_
#define NANOSSHARP_PLATFORM_LINUX_RUNTIME_H_

#include <array>
#include <filesystem>
#include <memory>
#include <string>
#include <thread>

#include "game/scripting.h"
#include "platform/linux/symbols.h"
#include "platform/linux/hook_manager.h"
#include "host/dotnet_host.h"

namespace nanossharp {

class Runtime {
 public:
  static Runtime& Instance();

  void Load();
  void Log(game::LogType type, std::string message) const;

  game::Engine* engine() const { return engine_; }
  bool IsMainThread() const;

 private:
  Runtime() = default;

  static void OnEngineInitialize(bool is_server, void (*setup)(game::Engine&),
                                 void (*post_setup)(), void (*post_initialize)());
  static void OnEngineRelease();
  static void OnServerTick(void* server, float delta_time);
  static void OnPackageLoad(game::Package* package);
  static void OnPackageUnload(game::Package* package, bool is_reload, bool is_cleanup);
  static void OnEntityCreated(game::Engine* engine, const game::Entity& entity,
                              const std::shared_ptr<game::IScriptEntityRef>& ref,
                              uint64_t class_id);
  static void OnEntityDestroyed(game::Engine* engine, const game::Entity& entity,
                                uint64_t class_id);

  bool InstallHooks(std::string& error);
  bool StartManaged(std::string& error);
  void AttachEngine();
  void DetachEngine();
  void Disable(const std::string& reason);
  bool ValidatePackageLayout(const game::Package* package);

  using EntityCreatedFunction = decltype(&OnEntityCreated);
  using EntityDestroyedFunction = decltype(&OnEntityDestroyed);

  std::filesystem::path runtime_directory_;
  std::unique_ptr<hooks::HookManager> hooks_;
  std::unique_ptr<host::DotnetHost> host_;
  std::array<void*, game::kEngineVirtualCount + 2> engine_vtable_{};
  game::EngineInitializeFunction original_engine_initialize_ = nullptr;
  game::EngineReleaseFunction original_engine_release_ = nullptr;
  game::ServerTickFunction original_server_tick_ = nullptr;
  game::PackageLoadFunction original_package_load_ = nullptr;
  game::PackageUnloadFunction original_package_unload_ = nullptr;
  EntityCreatedFunction original_entity_created_ = nullptr;
  EntityDestroyedFunction original_entity_destroyed_ = nullptr;
  game::Engine* engine_ = nullptr;
  std::thread::id main_thread_;
  bool disabled_ = false;
  bool packages_validated_ = false;
  bool managed_started_ = false;
  bool managed_failed_ = false;
};

}

#endif
