#include "platform/windows/runtime.h"

#include <cwchar>
#include <utility>

#include "bridge/native_api.h"
#include "platform/windows/allocator.h"
#include "platform/windows/lua_symbols.h"
#include "platform/windows/rtti.h"

namespace nanossharp {

namespace {

constexpr char kLuaEngineType[] = ".?AVLuaEngine@lua@scripting@nw@@";
constexpr char kLuaEnvironmentType[] = ".?AVLuaEnvironment@lua@scripting@nw@@";
constexpr char kHostEnvironmentName[] = "nanossharp";
constexpr wchar_t kLocalApplicationData[] = L"LOCALAPPDATA";
constexpr wchar_t kPackagesDirectory[] = L"NanosWorld\\Saved\\Packages";
constexpr char kClassTypePrefix[] = ".?AV?$Class@";
constexpr wchar_t kGameExecutablePrefix[] = L"NanosWorld";
constexpr wchar_t kHelperProcessArgument[] = L"--type=";
constexpr wchar_t kDirectoryName[] = L"NanosSharp";
constexpr wchar_t kRuntimeDirectoryName[] = L"runtime";
constexpr wchar_t kLogFileName[] = L"nanossharp.log";
constexpr wchar_t kErrorFileName[] = L"stderr.log";
constexpr char kEntryType[] = "NanosSharp.Host.Entry";
constexpr char kEntryMethod[] = "Initialize";
constexpr int kEngineDestructorSlot = 2;
constexpr int kEngineDestroySlot = 4;
constexpr int kEngineCreateEnvironmentSlot = 6;
constexpr int kEnvironmentDestructorSlot = 0;
constexpr ULONG kStackFrameCount = 24;
constexpr DWORD kCppException = 0xE06D7363;
constexpr DWORD kClrException = 0xE0434352;
constexpr DWORD kThreadNameException = 0x406D1388;
constexpr DWORD kWidePrintException = 0x4001000A;
constexpr size_t kMaximumClassCount = 512;

using ManagedEntryPoint = int32_t (*)(const bridge::NativeApi* api,
                                      bridge::ManagedCallbacks* callbacks,
                                      const char* runtime_directory);

const char* LogTypeName(game::LogType type) {
  switch (type) {
    case game::LogType::kWarning:
    case game::LogType::kScriptWarning:
      return "WARN";
    case game::LogType::kError:
    case game::LogType::kScriptError:
    case game::LogType::kFatal:
      return "ERROR";
    case game::LogType::kDebug:
    case game::LogType::kScriptDebug:
    case game::LogType::kVerbose:
      return "DEBUG";
    default:
      return "INFO";
  }
}

std::string ToUtf8(const std::filesystem::path& path) {
  std::u8string text = path.u8string();
  return std::string(text.begin(), text.end());
}

bool ValidateEngine(const game::Engine* engine, std::string& error) {
  if (!platform::rtti::HasTypeName(engine, kLuaEngineType)) {
    error = "the scripting engine is not the expected LuaEngine";
    return false;
  }
  const auto& classes = engine->classes();
  if (classes.empty() || classes.size() > kMaximumClassCount) {
    error = "the scripting class list has an unexpected size of " + std::to_string(classes.size());
    return false;
  }
  for (const game::IClass* scripting_class : classes) {
    if (!platform::rtti::HasTypePrefix(scripting_class, kClassTypePrefix)) {
      error = "the scripting class list contains an unexpected object";
      return false;
    }
  }
  return true;
}

}

Runtime& Runtime::Instance() {
  static Runtime runtime;
  return runtime;
}

void Runtime::Load(HMODULE module) {
  if (!IsGameProcess()) {
    return;
  }

  std::wstring module_path(MAX_PATH, L'\0');
  module_path.resize(GetModuleFileNameW(module, module_path.data(), MAX_PATH));
  base_directory_ = std::filesystem::path(module_path).parent_path() / kDirectoryName;
  std::filesystem::path log_path = base_directory_ / kLogFileName;
  _wfopen_s(&log_file_, log_path.c_str(), L"w");

  std::filesystem::path error_path = base_directory_ / kErrorFileName;
  HANDLE error_file = CreateFileW(error_path.c_str(), FILE_APPEND_DATA, FILE_SHARE_READ | FILE_SHARE_WRITE, nullptr,
                                  CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, nullptr);
  if (error_file != INVALID_HANDLE_VALUE) {
    SetStdHandle(STD_ERROR_HANDLE, error_file);
    SetStdHandle(STD_OUTPUT_HANDLE, error_file);
  }
  SetEnvironmentVariableW(L"DOTNET_EnableCrashReport", L"1");

  AddVectoredExceptionHandler(0, &Runtime::OnException);
  HANDLE thread = CreateThread(nullptr, 0, &Runtime::InstallHooks, this, 0, nullptr);
  if (thread != nullptr) {
    CloseHandle(thread);
  }
}

void Runtime::Unload() {
  if (log_file_ != nullptr) {
    std::fclose(log_file_);
    log_file_ = nullptr;
  }
}

void Runtime::Log(game::LogType type, const std::string& message) {
  std::lock_guard lock(log_mutex_);
  if (log_file_ == nullptr) {
    return;
  }
  SYSTEMTIME time;
  GetLocalTime(&time);
  std::fprintf(log_file_, "%04d-%02d-%02d %02d:%02d:%02d %5s  %s\n", time.wYear, time.wMonth, time.wDay,
               time.wHour, time.wMinute, time.wSecond, LogTypeName(type), message.c_str());
  std::fflush(log_file_);
}

bool Runtime::IsGameProcess() const {
  std::wstring path(MAX_PATH, L'\0');
  path.resize(GetModuleFileNameW(nullptr, path.data(), MAX_PATH));
  std::wstring name = std::filesystem::path(path).filename().wstring();
  bool is_game = name.rfind(kGameExecutablePrefix, 0) == 0;
  bool is_helper = std::wcsstr(GetCommandLineW(), kHelperProcessArgument) != nullptr;
  return is_game && !is_helper;
}

DWORD WINAPI Runtime::InstallHooks(void* parameter) {
  auto* runtime = static_cast<Runtime*>(parameter);
  void** vtable = platform::rtti::FindVtable(GetModuleHandleW(nullptr), kLuaEngineType);
  if (vtable == nullptr) {
    runtime->disabled_ = true;
    runtime->Log(game::LogType::kError, "this game version is not supported, the LuaEngine was not found");
    return 0;
  }

  void** environment_vtable = platform::rtti::FindVtable(GetModuleHandleW(nullptr), kLuaEnvironmentType);
  if (environment_vtable == nullptr ||
      !platform::allocator::Resolve(vtable[kEngineCreateEnvironmentSlot], environment_vtable[kEnvironmentDestructorSlot])) {
    runtime->disabled_ = true;
    runtime->Log(game::LogType::kError, "this game version is not supported, the game allocator was not found");
    return 0;
  }

  runtime->PatchSlot(vtable, game::kEngineEntityDestroyedSlot, reinterpret_cast<void*>(&OnEntityDestroyed),
                     reinterpret_cast<void**>(&runtime->original_entity_destroyed_));
  runtime->PatchSlot(vtable, game::kEngineEntityCreatedSlot, reinterpret_cast<void*>(&OnEntityCreated),
                     reinterpret_cast<void**>(&runtime->original_entity_created_));
  runtime->PatchSlot(vtable, kEngineDestructorSlot, reinterpret_cast<void*>(&OnEngineDestroyed),
                     reinterpret_cast<void**>(&runtime->original_engine_destroyed_));
  runtime->PatchSlot(vtable, kEngineDestroySlot, reinterpret_cast<void*>(&OnEngineClosing),
                     reinterpret_cast<void**>(&runtime->original_engine_closing_));
  runtime->PatchSlot(vtable, kEngineCreateEnvironmentSlot, reinterpret_cast<void*>(&OnEnvironmentCreated),
                     reinterpret_cast<void**>(&runtime->original_environment_created_));

  runtime->PatchSlot(environment_vtable, kEnvironmentDestructorSlot, reinterpret_cast<void*>(&OnEnvironmentDestroyed),
                     reinterpret_cast<void**>(&runtime->original_environment_destroyed_));
  std::string lua_error;
  if (platform::lua::Resolve(GetModuleHandleW(nullptr), lua_error)) {
    runtime->Log(game::LogType::kDebug, "Lua interop is available");
  } else {
    runtime->Log(game::LogType::kWarning, "Lua interop is disabled: " + lua_error);
  }
  runtime->Log(game::LogType::kInfo, "waiting for the scripting engine");
  return 0;
}

void Runtime::PatchSlot(void** vtable, int slot, void* detour, void** original) {
  DWORD protection = 0;
  VirtualProtect(&vtable[slot], sizeof(void*), PAGE_READWRITE, &protection);
  *original = vtable[slot];
  vtable[slot] = detour;
  VirtualProtect(&vtable[slot], sizeof(void*), protection, &protection);
}

void Runtime::AttachEngine(game::Engine* engine) {
  if (disabled_ || engine == engine_) {
    return;
  }
  platform::allocator::Activate();
  DetachEngine();

  Log(game::LogType::kDebug, "scripting engine attaching");
  std::string error;
  if (!ValidateEngine(engine, error)) {
    disabled_ = true;
    Log(game::LogType::kError, "this game version is not supported, NanosSharp is disabled: " + error);
    return;
  }
  engine_ = engine;
  main_thread_ = GetCurrentThreadId();

  if (!managed_started_ && !managed_failed_) {
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
  Log(game::LogType::kDebug, "scripting engine attached");
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

bool Runtime::StartManaged(std::string& error) {
  std::filesystem::path runtime_directory = base_directory_ / kRuntimeDirectoryName;
  host_ = std::make_unique<host::DotnetHost>(runtime_directory);
  if (!host_->Start(error)) {
    return false;
  }

  auto entry_point = reinterpret_cast<ManagedEntryPoint>(host_->GetEntryPoint(kEntryType, kEntryMethod, error));
  if (entry_point == nullptr) {
    return false;
  }

  bridge::ManagedCallbacks& callbacks = bridge::GetManagedCallbacks();
  std::string directory = ToUtf8(runtime_directory);
  int32_t result = entry_point(&bridge::GetNativeApi(), &callbacks, directory.c_str());
  if (result != 0) {
    error = "managed initialization failed with code " + std::to_string(result);
    callbacks = {};
    return false;
  }
  return true;
}

void Runtime::OnEntityDestroyed(game::Engine* engine, const game::Entity& entity, uint64_t class_id) {
  Runtime& runtime = Instance();
  runtime.AttachEngine(engine);
  if (runtime.engine_ == engine) {
    if (auto entity_destroyed = bridge::GetManagedCallbacks().entity_destroyed) {
      entity_destroyed(entity.class_id, entity.instance);
    }
  }
  runtime.original_entity_destroyed_(engine, entity, class_id);
}

void Runtime::OnEntityCreated(game::Engine* engine, const game::Entity& entity,
                              const std::shared_ptr<game::IScriptEntityRef>& ref, uint64_t class_id) {
  Runtime& runtime = Instance();
  runtime.original_entity_created_(engine, entity, ref, class_id);
  runtime.AttachEngine(engine);
  if (runtime.engine_ == engine) {
    if (auto entity_created = bridge::GetManagedCallbacks().entity_created) {
      entity_created(entity.class_id, entity.instance, entity.network_id);
    }
  }
}

void* Runtime::OnEngineDestroyed(game::Engine* engine, uint32_t flags) {
  Runtime& runtime = Instance();
  if (runtime.engine_ == engine) {
    runtime.DetachEngine();
  }
  return runtime.original_engine_destroyed_(engine, flags);
}

void Runtime::OnEngineClosing(game::Engine* engine) {
  Runtime& runtime = Instance();
  if (runtime.engine_ == engine) {
    runtime.Log(game::LogType::kDebug, "scripting engine closing");
    runtime.DetachEngine();
  }
  runtime.original_engine_closing_(engine);
}

game::Environment* Runtime::OnEnvironmentCreated(game::Engine* engine, const std::string& name,
                                                 game::CompatibilityVersion version) {
  Runtime& runtime = Instance();
  runtime.Log(game::LogType::kDebug, "environment creating: " + name);
  game::Environment* environment = runtime.original_environment_created_(engine, name, version);
  if (name == kHostEnvironmentName) {
    return environment;
  }

  runtime.AttachEngine(engine);
  if (runtime.engine_ == engine) {
    runtime.LoadPackage(environment, name);
  }
  return environment;
}

void* Runtime::OnEnvironmentDestroyed(game::Environment* environment, uint32_t flags) {
  Runtime& runtime = Instance();
  if (runtime.packages_.erase(environment) != 0) {
    runtime.Log(game::LogType::kDebug, "package unloading");
    if (auto package_unloading = bridge::GetManagedCallbacks().package_unloading) {
      package_unloading(reinterpret_cast<game::Package*>(environment));
    }
    runtime.Log(game::LogType::kDebug, "package unloaded");
  }
  return runtime.original_environment_destroyed_(environment, flags);
}

LONG CALLBACK Runtime::OnException(EXCEPTION_POINTERS* exception) {
  DWORD code = exception->ExceptionRecord->ExceptionCode;
  if (code == kCppException || code == kClrException || code == kThreadNameException ||
      code == DBG_PRINTEXCEPTION_C || code == kWidePrintException) {
    return EXCEPTION_CONTINUE_SEARCH;
  }

  auto address = reinterpret_cast<uintptr_t>(exception->ExceptionRecord->ExceptionAddress);
  HMODULE module = nullptr;
  GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
                     reinterpret_cast<LPCWSTR>(address), &module);
  std::wstring module_path(MAX_PATH, L'\0');
  module_path.resize(module != nullptr ? GetModuleFileNameW(module, module_path.data(), MAX_PATH) : 0);
  char text[256];
  std::snprintf(text, sizeof(text), "exception 0x%lx at %s+0x%llx reading 0x%llx", code,
                ToUtf8(std::filesystem::path(module_path).filename()).c_str(),
                static_cast<unsigned long long>(address - reinterpret_cast<uintptr_t>(module)),
                static_cast<unsigned long long>(exception->ExceptionRecord->ExceptionInformation[1]));
  Instance().Log(game::LogType::kError, text);

  void* frames[kStackFrameCount] = {};
  USHORT count = CaptureStackBackTrace(0, kStackFrameCount, frames, nullptr);
  for (USHORT index = 0; index < count; ++index) {
    HMODULE frame_module = nullptr;
    GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
                       static_cast<LPCWSTR>(frames[index]), &frame_module);
    std::wstring frame_path(MAX_PATH, L'\0');
    frame_path.resize(frame_module != nullptr ? GetModuleFileNameW(frame_module, frame_path.data(), MAX_PATH) : 0);
    std::snprintf(text, sizeof(text), "  at %s+0x%llx", ToUtf8(std::filesystem::path(frame_path).filename()).c_str(),
                  static_cast<unsigned long long>(reinterpret_cast<uintptr_t>(frames[index]) -
                                                  reinterpret_cast<uintptr_t>(frame_module)));
    Instance().Log(game::LogType::kError, text);
  }
  return EXCEPTION_CONTINUE_SEARCH;
}

void Runtime::LoadPackage(game::Environment* environment, const std::string& name) {
  auto package_loaded = bridge::GetManagedCallbacks().package_loaded;
  if (package_loaded == nullptr) {
    return;
  }

  std::wstring local_data(MAX_PATH, L'\0');
  local_data.resize(GetEnvironmentVariableW(kLocalApplicationData, local_data.data(), MAX_PATH));
  std::filesystem::path directory = std::filesystem::path(local_data) / kPackagesDirectory /
                                    std::filesystem::path(reinterpret_cast<const char8_t*>(name.c_str()));
  std::string utf8_directory = ToUtf8(directory);
  Log(game::LogType::kDebug, "package loaded: " + name + " in " + utf8_directory);
  packages_.insert(environment);
  package_loaded(reinterpret_cast<game::Package*>(environment), name.c_str(), utf8_directory.c_str(), environment);
}

}
