#include "runtime.h"

#include <dlfcn.h>

#include <cstdio>
#include <cstring>
#include <utility>

#include "bridge/native_api.h"
#include "game/validation.h"

namespace nanossharp {

namespace {

constexpr char kLogPrefix[] = "[NanosSharp] ";
constexpr char kEntryType[] = "NanosSharp.Host.Entry";
constexpr char kEntryMethod[] = "Initialize";

using ManagedEntryPoint = int32_t (*)(const bridge::NativeApi* api,
                                      bridge::ManagedCallbacks* callbacks,
                                      const char* runtime_directory);

std::filesystem::path ResolveRuntimeDirectory() {
  Dl_info info{};
  if (dladdr(reinterpret_cast<void*>(&ResolveRuntimeDirectory), &info) == 0 ||
      info.dli_fname == nullptr) {
    return std::filesystem::current_path();
  }
  return std::filesystem::absolute(info.dli_fname).parent_path();
}

}

Runtime& Runtime::Instance() {
  static Runtime runtime;
  return runtime;
}

void Runtime::Load() {
  std::string error;
  if (!game::GetSymbols().Resolve(error)) {
    return;
  }
  unsetenv("LD_PRELOAD");

  runtime_directory_ = ResolveRuntimeDirectory();
  if (!InstallHooks(error)) {
    std::fprintf(stderr, "%sfailed to install hooks: %s\n", kLogPrefix, error.c_str());
  }
}

void Runtime::Log(game::LogType type, std::string message) const {
  auto enqueue_log = game::GetSymbols().enqueue_log;
  if (enqueue_log == nullptr) {
    std::fprintf(stderr, "%s%s\n", kLogPrefix, message.c_str());
    return;
  }
  enqueue_log(type, std::string(kLogPrefix) + message);
}

bool Runtime::IsMainThread() const {
  return std::this_thread::get_id() == main_thread_;
}

bool Runtime::InstallHooks(std::string& error) {
  game::Symbols& symbols = game::GetSymbols();
  hooks_ = std::make_unique<hooks::HookManager>();

  original_engine_initialize_ = symbols.engine_initialize;
  original_engine_release_ = symbols.engine_release;
  original_server_tick_ = symbols.server_tick;
  original_package_load_ = symbols.package_load;
  original_package_unload_ = symbols.package_unload;

  bool prepared =
      hooks_->Prepare(original_engine_initialize_, &Runtime::OnEngineInitialize) &&
      hooks_->Prepare(original_engine_release_, &Runtime::OnEngineRelease) &&
      hooks_->Prepare(original_server_tick_, &Runtime::OnServerTick) &&
      hooks_->Prepare(original_package_load_, &Runtime::OnPackageLoad) &&
      hooks_->Prepare(original_package_unload_, &Runtime::OnPackageUnload);
  if (!prepared || !hooks_->Install()) {
    error = hooks_->error();
    return false;
  }
  return true;
}

bool Runtime::StartManaged(std::string& error) {
  host_ = std::make_unique<host::DotnetHost>(runtime_directory_);
  if (!host_->Start(error)) {
    return false;
  }

  auto entry_point = reinterpret_cast<ManagedEntryPoint>(
      host_->GetEntryPoint(kEntryType, kEntryMethod, error));
  if (entry_point == nullptr) {
    return false;
  }

  bridge::ManagedCallbacks& callbacks = bridge::GetManagedCallbacks();
  int32_t result = entry_point(&bridge::GetNativeApi(), &callbacks,
                               runtime_directory_.c_str());
  if (result != 0) {
    error = "managed initialization failed with code " + std::to_string(result);
    callbacks = {};
    return false;
  }
  return true;
}

void Runtime::AttachEngine() {
  std::unique_ptr<game::Engine>* instance = game::GetSymbols().engine_instance;
  game::Engine* engine = instance != nullptr ? instance->get() : nullptr;
  if (disabled_ || engine == nullptr) {
    return;
  }

  std::string validation_error;
  if (!game::ValidateEngine(engine, validation_error)) {
    Disable(validation_error);
    return;
  }
  engine_ = engine;

  void** original_vtable = *reinterpret_cast<void***>(engine_) - 2;
  std::memcpy(engine_vtable_.data(), original_vtable,
              engine_vtable_.size() * sizeof(void*));
  original_entity_created_ = reinterpret_cast<EntityCreatedFunction>(
      engine_vtable_[2 + game::kEngineEntityCreatedSlot]);
  original_entity_destroyed_ = reinterpret_cast<EntityDestroyedFunction>(
      engine_vtable_[2 + game::kEngineEntityDestroyedSlot]);
  engine_vtable_[2 + game::kEngineEntityCreatedSlot] =
      reinterpret_cast<void*>(&Runtime::OnEntityCreated);
  engine_vtable_[2 + game::kEngineEntityDestroyedSlot] =
      reinterpret_cast<void*>(&Runtime::OnEntityDestroyed);
  *reinterpret_cast<void***>(engine_) = engine_vtable_.data() + 2;

  if (!managed_started_ && !managed_failed_) {
    std::string error;
    managed_started_ = StartManaged(error);
    if (!managed_started_) {
      managed_failed_ = true;
      Log(game::LogType::kError, "failed to start .NET: " + error);
      return;
    }
  }

  if (auto engine_attached = bridge::GetManagedCallbacks().engine_attached) {
    engine_attached();
  }
}

void Runtime::Disable(const std::string& reason) {
  disabled_ = true;
  engine_ = nullptr;
  Log(game::LogType::kError,
      "this server version is not supported, NanosSharp is disabled: " + reason);
}

bool Runtime::ValidatePackageLayout(const game::Package* package) {
  if (packages_validated_) {
    return true;
  }

  std::string validation_error;
  if (!game::ValidatePackage(package, validation_error)) {
    Disable(validation_error);
    return false;
  }
  packages_validated_ = true;
  return true;
}

void Runtime::DetachEngine() {
  if (engine_ == nullptr) {
    return;
  }
  if (auto engine_detaching = bridge::GetManagedCallbacks().engine_detaching) {
    engine_detaching();
  }
  engine_ = nullptr;
}

void Runtime::OnEngineInitialize(bool is_server, void (*setup)(game::Engine&),
                                 void (*post_setup)(), void (*post_initialize)()) {
  Runtime& runtime = Instance();
  runtime.main_thread_ = std::this_thread::get_id();
  runtime.original_engine_initialize_(is_server, setup, post_setup, post_initialize);
  runtime.AttachEngine();
}

void Runtime::OnEngineRelease() {
  Runtime& runtime = Instance();
  runtime.DetachEngine();
  runtime.original_engine_release_();
}

void Runtime::OnServerTick(void* server, float delta_time) {
  Runtime& runtime = Instance();
  if (runtime.engine_ != nullptr) {
    if (auto tick = bridge::GetManagedCallbacks().tick) {
      tick(delta_time);
    }
  }
  runtime.original_server_tick_(server, delta_time);
}

void Runtime::OnPackageLoad(game::Package* package) {
  Runtime& runtime = Instance();
  runtime.original_package_load_(package);
  auto package_loaded = bridge::GetManagedCallbacks().package_loaded;
  if (runtime.engine_ == nullptr || package_loaded == nullptr ||
      package->environment() == nullptr || !runtime.ValidatePackageLayout(package)) {
    return;
  }

  std::string directory = *game::GetSymbols().packages_path + "/" + package->name();
  package_loaded(package, package->name().c_str(), directory.c_str(), package->environment());
}

void Runtime::OnPackageUnload(game::Package* package, bool is_reload, bool is_cleanup) {
  Runtime& runtime = Instance();
  if (auto package_unloading = bridge::GetManagedCallbacks().package_unloading) {
    package_unloading(package);
  }
  runtime.original_package_unload_(package, is_reload, is_cleanup);
}

void Runtime::OnEntityCreated(game::Engine* engine, const game::Entity& entity,
                              const std::shared_ptr<game::IScriptEntityRef>& ref,
                              uint64_t class_id) {
  Runtime& runtime = Instance();
  runtime.original_entity_created_(engine, entity, ref, class_id);
  if (auto entity_created = bridge::GetManagedCallbacks().entity_created) {
    entity_created(entity.class_id, entity.instance);
  }
}

void Runtime::OnEntityDestroyed(game::Engine* engine, const game::Entity& entity,
                                uint64_t class_id) {
  Runtime& runtime = Instance();
  if (auto entity_destroyed = bridge::GetManagedCallbacks().entity_destroyed) {
    entity_destroyed(entity.class_id, entity.instance);
  }
  runtime.original_entity_destroyed_(engine, entity, class_id);
}

}
